using System;
using System.Collections.Generic;
using System.Linq;
using GeneCore.Core.Logging;
using GeneCore.Core.ProcessComponents;
using GeneCore.TerminationConditions;

namespace GeneCore.Core
{
    /// <summary>
    /// Encloses the state of the entire optimization process and provides methods for running the process and
    /// accessing its state.
    /// </summary>
    /// <typeparam name="TPopulation"></typeparam>
    /// <typeparam name="TIndividual"></typeparam>
    /// <typeparam name="TChromosome"></typeparam>
    /// <typeparam name="TGene"></typeparam>
    /// <typeparam name="TProcessInformation"></typeparam>
    public partial class GeneticSelectionProcess<TPopulation, TIndividual, TChromosome, TGene, TProcessInformation>
        where TPopulation : class, IPopulation<TIndividual, TGene>
        where TIndividual : class, IIndividual<TGene>
        where TChromosome : class, IChromosome<TGene>
        where TProcessInformation : class, IProcessInformation
    {
        private const Int32 MaxHistoryLength = 1_000_000;

        // Function which evaluates fitness for each individual.
        private IFitnessFunction<TIndividual, TGene> _fitnessFunction;

        // This boolean is set to true when any end condition is met.
        private volatile Boolean _hasConverged;

        private Int32? _initialPopulationSize;

        private volatile Boolean _isRunning;
        private volatile Boolean _isRunningLocked;

        // Logger objects.
        private IList<ILogger> _loggers;

        // Mating function which processes parent information into a new individual (offspring).
        private IMatingFunction<TIndividual, TChromosome, TGene> _matingFunction;

        // Function which uses some heuristic to select the parents for new offspring.
        private IParentSelector<TPopulation, TIndividual, TGene> _parentSelector;

        // Population.
        private TPopulation _population;

        // Function initializing the population.
        private IPopulationInitializer<TPopulation, TIndividual, TGene> _populationInitializer;

        // Population modifiers like random mutation.
        private IList<IPopulationModifier<TPopulation, TIndividual, TGene>> _populationModifiers;

        // The most recent process information object.
        private TProcessInformation _previousProcessInformation;

        // Process information composer which records information about the selection process.
        private IProcessInformationComposer<TPopulation, TIndividual, TGene, TProcessInformation>
            _processInformationComposer;

        // Stores the history of process as process information objects.
        private IList<TProcessInformation> _processInformationHistory;

        // Function which uses some heuristic to select the parents for new offspring.
        private ISurvivorSelector<TPopulation, TIndividual, TGene> _survivorSelector;

        // Search termination conditions.
        private IList<ITerminationCondition<TPopulation, TIndividual, TGene, TProcessInformation>>
            _terminationConditions;

        private GeneticSelectionProcess() { }

        public TPopulation GetPopulation()
        {
            #region Validation

            if (_population == null) throw new InvalidOperationException("Not initialized");
            if (_isRunning) throw new InvalidOperationException("Tried to get population while running");

            #endregion

            return _population;
        }

        /// <summary>
        ///     Initialize the process, by creating the initial population using the configured provider.
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        public void Initialize()
        {
            #region Validation

            if (_population != null) throw new InvalidOperationException("Already initialized");
            if (_populationInitializer == null)
                throw new InvalidOperationException($"{nameof(_populationInitializer)} not set");
            if (!_initialPopulationSize.HasValue)
                throw new InvalidOperationException($"{nameof(_initialPopulationSize)} not set");

            #endregion

            LogInfo("Initializing population");
            // ReSharper disable once PossibleNullReferenceException
            _population = _populationInitializer.InitializePopulation(_initialPopulationSize.Value);

            LogInfo("Initializing calculating initial fitnesses");
            _population.OrderByFitness(_fitnessFunction);
        }

        /// <summary>
        ///     Run a single generation of the process.
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        public void RunGeneration()
        {
            #region Validation

            if (_population == null)
                throw new InvalidOperationException($"{nameof(_population)} was null");
            if (_survivorSelector == null)
                throw new InvalidOperationException($"{nameof(_survivorSelector)} was null");
            if (_populationModifiers == null)
                throw new InvalidOperationException($"{nameof(_populationModifiers)} was null");
            if (_parentSelector == null)
                throw new InvalidOperationException($"{nameof(_parentSelector)} was null");
            if (_processInformationComposer == null)
                throw new InvalidOperationException($"{nameof(_processInformationComposer)} was null");

            #endregion

            _isRunning = true;

            UInt64 generationCount = _previousProcessInformation?.GetGeneration() ?? 0;
            LogDebug($"Starting generation {generationCount}");

            // Select parents to produce new generation of offspring.
            LogDebug("Selecting parents");
            
            IList<IEnumerable<TIndividual>> parentGroups = _parentSelector?.GetParents(_population)?.ToList();
            if (parentGroups == null) throw new InvalidOperationException($"{nameof(parentGroups)} was null");
            
            // Log parents and their fitness.
            Int32 parentGroupCount = 0;
            foreach (IEnumerable<TIndividual> parentGroup in parentGroups)
            {
                Double totalFitness = 0;
                IList<String> parentStrings = new List<String>();
                foreach (TIndividual parent in parentGroup)
                {
                    IndividualWithFitness<TIndividual, TGene> individualParent =
                        _population.GetIndividualWithFitness(parent.GetGuid());

                    Double parentFitness = individualParent.GetFitness();
                    parentStrings.Add($"P {parent.GetGuidStringShort()} F {parentFitness}");
                    totalFitness += parentFitness;
                }

                LogInfo($"PG {parentGroupCount}, TF {totalFitness}, " + $"{string.Join(", ", parentStrings)}");

                parentGroupCount++;
            }

            if (parentGroups == null || !parentGroups.Any())
                throw new InvalidOperationException("Failed to get parents");

            // Mate selected parents, produce offspring.
            LogDebug("Mating parents; creating offspring");
            IList<TIndividual> offspring =
                parentGroups.Select(parentGroup => _matingFunction.Mate(parentGroup.ToList())).ToList();

            // Log unmodified offspring fitness.
            foreach (TIndividual individualOffspring in offspring)
            {
                Double offspringFitness = _fitnessFunction.CalculateFitness(individualOffspring);
                LogInfo($"O {individualOffspring.GetGuidStringShort()} F {offspringFitness}");
            }

            // Add new offspring to population.
            LogDebug("Selecting survivors; adding offspring to population");
            _survivorSelector.AddOffspring(_population, offspring);

            // Apply population modifiers to population.
            LogDebug("Applying population modifier");
            foreach (IPopulationModifier<TPopulation, TIndividual, TGene> populationModifier in _populationModifiers)
                _population = populationModifier.ModifyPopulation(_population);

            // Order by fitness.
            LogDebug("Ordering population by fitness");
            _population.OrderByFitness(_fitnessFunction);

            // Create information object.
            TProcessInformation currentGenerationProcessInformation =
                _processInformationComposer.ComposeProcessInformation(_previousProcessInformation, _population);

            _processInformationHistory.Add(currentGenerationProcessInformation);

            LogInfo($"G{generationCount} TF {currentGenerationProcessInformation.GetTotalFitness()}");

            // Check if population has converged according to provided checks.
            if (!_hasConverged)
            {
                if (_terminationConditions.Any(terminationCondition =>
                    terminationCondition.ShouldTerminate(_population, currentGenerationProcessInformation)))
                {
                    _hasConverged = true;
                    LogInfo("Population has converged! Termination condition met!");
                }
            }

            // Compose new process information object.
            _previousProcessInformation = currentGenerationProcessInformation;

            if (!_isRunningLocked) _isRunning = false;
        }

        /// <summary>
        ///     Run the process until at least one termination condition is set.
        /// </summary>
        public void RunUntilConvergence()
        {
            _isRunning = true;
            _isRunningLocked = true;

            while (!HasConverged()) RunGeneration();

            _isRunning = false;
            _isRunningLocked = false;
        }

        /// <returns>True if the population has converged eg. a termination condition is met.</returns>
        public Boolean HasConverged()
        {
            return _hasConverged;
        }

        #region Logging methods

        private void LogDebug(String msg)
        {
            foreach (ILogger logger in _loggers) logger.Debug(msg);
        }

        private void LogInfo(String msg)
        {
            foreach (ILogger logger in _loggers) logger.Info(msg);
        }

        private void LogWarning(String msg)
        {
            foreach (ILogger logger in _loggers) logger.Warning(msg);
        }

        private void LogError(String msg, Exception exception = null)
        {
            foreach (ILogger logger in _loggers)
            {
                if (exception == null)
                    logger.Error(msg);
                else
                    logger.Exception(msg, exception);
            }
        }

        #endregion
    }
}