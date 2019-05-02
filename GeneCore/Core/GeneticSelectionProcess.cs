using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GeneCore.FitnessFunctions;
using GeneCore.MatingFunctions;
using GeneCore.ParentSelectors;
using GeneCore.PopulationInitializers;
using GeneCore.PopulationModifiers;
using GeneCore.SurvivorSelectors;
using GeneCore.TerminationConditions;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace GeneCore.Core {
    public class GeneticSelectionProcess<TFitness, TIndividual> where TIndividual : IIndividual {
        // Search termination conditions.
        [NotNull]
        private IList<ITerminationCondition> _terminationConditions;
        // Population modifiers like random mutation.
        [NotNull]
        private IList<IPopulationModifier> _populationModifiers;
        // Mating function which processes parent information into a new individual (offspring).
        [NotNull]
        private IMatingFunction _matingFunction;
        // Function which uses some heuristic to select the parents for new offspring.
        [NotNull]
        private ISurvivorSelector _survivorSelector;
        // Function which uses some heuristic to select the parents for new offspring.
        [NotNull]
        private IParentSelector _parentSelector;
        // Function which evaluates fitness for each individual.
        [NotNull]
        private IFitnessFunction _fitnessFunction;
        // Population.
        [NotNull]
        private IPopulation<TIndividual> _population;
        // Function initializing the population.
        [NotNull]
        private IPopulationInitializer<TIndividual> _populationInitializer;
        // Process information composer which records information about the selection process.
        [NotNull]
        private IProcessInformationComposer _processInformationComposer;
        // The most recent process information object.
        private IProcessInformation _previousProcessInformation;
        // This boolean is set to true when any end condition is met.
        private Boolean _hasConverged;

        // Logger object. 
        [NotNull]
        private IList<ILogger> _loggers;
        private LogLevels _logLevels = LogLevels.Nothing;

        private GeneticSelectionProcess() {
        }

        public IPopulation<TIndividual> GetPopulation() {
            return Utils.DeepCopy(_population);
        }

        public async Task Initialize() {
            if (_population != null) throw new InvalidOperationException($"Already initialized");
            
            LogInfo("Initializing population");
            _population = await _populationInitializer.InitializePopulation();
        }

        public async Task RunGeneration() {
            UInt64 generationCount = _previousProcessInformation?.GetGeneration() ?? 1;
            LogInfo($"Starting generation {generationCount}");
            
            // Order by fitness.
            LogInfo($"Ordering population by fitness");
            _population.OrderByFitness(_fitnessFunction);

            // TODO: ORDER?
            
            // Apply population modifiers to population.
            LogInfo("Applying population modifier");
            foreach (IPopulationModifier populationModifier in _populationModifiers) {
                _population = populationModifier.ModifyPopulation(_population);
            }

            LogInfo("Selecting parents");
            IEnumerable<IEnumerable<IIndividual>> parents = _parentSelector.GetParents(_population);
            
            LogInfo("Mating parents; creating offspring");
            IList<IIndividual> offspring = parents
                .Select(parentGroup => _matingFunction.Mate(parentGroup))
                .ToList();
            
            LogInfo("Selecting survivors; adding offspring to population");
            _survivorSelector.AddOffspring(_population, offspring);

            // Compose new process information object.
            _previousProcessInformation = _processInformationComposer
                .ComposeProcessInformation(_previousProcessInformation, _population);
        }

        public async Task RunUntilConvergence() {
            while (!HasConverged()) {
                await RunGeneration();
            }
        }
        
        public Boolean HasConverged() {
            return _hasConverged;
        }

        private void LogInfo(String msg) {
            foreach (ILogger logger in _loggers) {
                logger.Info(msg);
            }
        }
        
        private void LogWarning(String msg) {
            foreach (ILogger logger in _loggers) {
                logger.Warning(msg);
            }
        }
        
        private void LogError(String msg, Exception exception = null) {
            foreach (ILogger logger in _loggers) {
                if (exception == null)
                    logger.Error(msg);
                else
                    logger.Exception(msg, exception);
            }
        }

        public class Builder {
            [NotNull]
            private readonly GeneticSelectionProcess<TFitness, TIndividual> _instance;

            public Builder() {
                _instance = new GeneticSelectionProcess<TFitness, TIndividual> {
                    _terminationConditions = new List<ITerminationCondition>(),
                    _populationModifiers = new List<IPopulationModifier>(),
                    _loggers = new List<ILogger>()
                };

            }

            public Builder AddTerminationCondition([NotNull] ITerminationCondition terminationCondition) {
                if (terminationCondition == null) throw new ArgumentNullException(nameof(terminationCondition));
                _instance._terminationConditions.Add(terminationCondition);
                
                return this;
            }
            
            public Builder AddPopulationModifier([NotNull] ITerminationCondition terminationCondition) {
                if (terminationCondition == null) throw new ArgumentNullException(nameof(terminationCondition));
                _instance._terminationConditions.Add(terminationCondition);

                return this;
            }
            
            public Builder SetFitnessFunction([NotNull] IFitnessFunction fitnessFunction) {
                if (fitnessFunction == null) throw new ArgumentNullException(nameof(fitnessFunction));
                if (_instance._fitnessFunction != null) throw new InvalidOperationException($"Already set");
                
                _instance._fitnessFunction = fitnessFunction;

                return this;
            }
            
            public Builder SetPopulationInitializer([NotNull] IPopulationInitializer<TIndividual> populationInitializer) {
                if (populationInitializer == null) throw new ArgumentNullException(nameof(populationInitializer));
                if (_instance._populationInitializer != null) throw new InvalidOperationException($"Already set");
                
                _instance._populationInitializer = populationInitializer;

                return this;
            }
            
            public Builder SetProcessInformationComposer([NotNull] IProcessInformationComposer processInformationComposer) {
                if (processInformationComposer == null) throw new ArgumentNullException(nameof(processInformationComposer));
                
                _instance._processInformationComposer = processInformationComposer;

                return this;
            }
            
            public Builder SetSurvivorSelector([NotNull] ISurvivorSelector survivorSelector) {
                if (survivorSelector == null) throw new ArgumentNullException(nameof(survivorSelector));
                
                _instance._survivorSelector = survivorSelector;

                return this;
            }
            
            public Builder SetMatingFunction([NotNull] IMatingFunction matingFunction) {
                if (matingFunction == null) throw new ArgumentNullException(nameof(matingFunction));
                
                _instance._matingFunction = matingFunction;

                return this;
            }
            
            public Builder UseDefaultConsoleLogger(Boolean usePrefix = true) {
                _instance._loggers.Add(new SimpleConsoleLogger(usePrefix));

                return this;
            }
            
            // ReSharper disable always ConditionIsAlwaysTrueOrFalse
            public GeneticSelectionProcess<TFitness, TIndividual> Build() {
                #region Validate built instance

                IList<String> missingProperties = new List<String>();
                
                if (!_instance._terminationConditions.Any()) {
                    missingProperties.Add("Termination conditions");
                }
                if (_instance._fitnessFunction == null) {
                    missingProperties.Add("Fitness Function");
                }
                if (_instance._populationInitializer == null) {
                    missingProperties.Add("Population initializer");
                }
                if (_instance._matingFunction == null) {
                    missingProperties.Add("Mating function");
                }
                if (_instance._survivorSelector == null) {
                    missingProperties.Add("Survivor selector");
                }

                if (missingProperties.Any()) {
                    throw new GeneticSelectionProcessNotCompleted(missingProperties.ToArray());
                }

                #endregion
                
                // Return a deep copy of the built object.
                return Utils.DeepCopy(_instance);
            }
        }
    }

    public class GeneticSelectionProcessNotCompleted : Exception {
        [NotNull]
        private readonly IList<String> _missingProperties;

        public GeneticSelectionProcessNotCompleted(params String[] missingProperties) {
            _missingProperties = new List<String>();
                
            if (missingProperties == null) return;
            
            foreach (String property in missingProperties) {
                _missingProperties.Add(property);
            }
        }

        public override String ToString() {
            String msg = "GeneticSelectionProcess being constructed was not completed before Build().";
            if (_missingProperties.Any()) {
                msg += $" Missing properties {String.Join(",", _missingProperties)}";
            }
            
            return msg;
        }
    }
}