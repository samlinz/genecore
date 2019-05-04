using System;
using System.Collections.Generic;
using System.Linq;
using GeneCore.Common;
using GeneCore.Core.Logging;
using GeneCore.Core.ProcessComponents;
using GeneCore.TerminationConditions;
using JetBrains.Annotations;

namespace GeneCore.Core
{
    public partial class GeneticSelectionProcess<TPopulation, TIndividual, TChromosome, TGene, TProcessInformation>
        where TPopulation : class, IPopulation<TIndividual, TGene>
        where TIndividual : class, IIndividual<TGene>
        where TChromosome : class, IChromosome<TGene>
        where TProcessInformation : class, IProcessInformation
    {
        /// <summary>
        ///     Builder to create optimization process objects.
        /// </summary>
        public class Builder
        {
            [NotNull]
            private readonly GeneticSelectionProcess<TPopulation, TIndividual, TChromosome, TGene, TProcessInformation>
                _instance;

            public Builder()
            {
                _instance =
                    new GeneticSelectionProcess<TPopulation, TIndividual, TChromosome, TGene, TProcessInformation>
                    {
                        _terminationConditions =
                            new List<ITerminationCondition<TPopulation, TIndividual, TGene, TProcessInformation>>()
                        , _populationModifiers = new List<IPopulationModifier<TPopulation, TIndividual, TGene>>()
                        , _loggers = new List<ILogger>()
                    };
            }

            [NotNull]
            public Builder AddTerminationCondition(
                [NotNull]
                ITerminationCondition<TPopulation, TIndividual, TGene, TProcessInformation> terminationCondition)
            {
                if (terminationCondition == null) throw new ArgumentNullException(nameof(terminationCondition));
                _instance._terminationConditions.Add(terminationCondition);

                return this;
            }

            [NotNull]
            public Builder AddPopulationModifier(
                [NotNull] IPopulationModifier<TPopulation, TIndividual, TGene> populationModifier)
            {
                if (populationModifier == null) throw new ArgumentNullException(nameof(populationModifier));
                _instance._populationModifiers.Add(populationModifier);

                return this;
            }

            [NotNull]
            public Builder SetFitnessFunction([NotNull] IFitnessFunction<TIndividual, TGene> fitnessFunction)
            {
                if (fitnessFunction == null) throw new ArgumentNullException(nameof(fitnessFunction));
                if (_instance._fitnessFunction != null) throw new InvalidOperationException("Already set");

                _instance._fitnessFunction = fitnessFunction;

                return this;
            }

            [NotNull]
            public Builder SetPopulationInitializer(
                [NotNull] IPopulationInitializer<TPopulation, TIndividual, TGene> populationInitializer)
            {
                if (populationInitializer == null) throw new ArgumentNullException(nameof(populationInitializer));
                if (_instance._populationInitializer != null) throw new InvalidOperationException("Already set");

                _instance._populationInitializer = populationInitializer;

                return this;
            }

            [NotNull]
            public Builder SetInitialPopulationSize([NotNull] Int32 populationSize)
            {
                if (_instance._initialPopulationSize != null) throw new InvalidOperationException("Already set");

                _instance._initialPopulationSize = populationSize;

                return this;
            }

            [NotNull]
            public Builder SetParentSelector([NotNull] IParentSelector<TPopulation, TIndividual, TGene> parentSelector)
            {
                if (parentSelector == null) throw new ArgumentNullException(nameof(parentSelector));
                if (_instance._parentSelector != null) throw new InvalidOperationException("Already set");

                _instance._parentSelector = parentSelector;

                return this;
            }

            [NotNull]
            public Builder SetProcessInformationComposer(
                [NotNull]
                IProcessInformationComposer<TPopulation, TIndividual, TGene, TProcessInformation>
                    processInformationComposer)
            {
                if (processInformationComposer == null)
                    throw new ArgumentNullException(nameof(processInformationComposer));

                _instance._processInformationComposer = processInformationComposer;

                return this;
            }

            [NotNull]
            public Builder SetSurvivorSelector(
                [NotNull] ISurvivorSelector<TPopulation, TIndividual, TGene> survivorSelector)
            {
                if (survivorSelector == null) throw new ArgumentNullException(nameof(survivorSelector));

                _instance._survivorSelector = survivorSelector;

                return this;
            }

            [NotNull]
            public Builder SetMatingFunction([NotNull] IMatingFunction<TIndividual, TChromosome, TGene> matingFunction)
            {
                if (matingFunction == null) throw new ArgumentNullException(nameof(matingFunction));

                _instance._matingFunction = matingFunction;

                return this;
            }

            [NotNull]
            public Builder UseDefaultConsoleLogger(Boolean usePrefix = true)
            {
                _instance._loggers.Add(new SimpleConsoleLogger(usePrefix));

                return this;
            }

            [NotNull]
            // ReSharper disable always ConditionIsAlwaysTrueOrFalse
            public GeneticSelectionProcess<TPopulation, TIndividual, TChromosome, TGene, TProcessInformation> Build()
            {
                #region Validate built instance

                IList<String> missingProperties = new List<String>();

                if (_instance._terminationConditions == null)
                    throw new InvalidOperationException($"{nameof(_instance._terminationConditions)} was null");
                if (_instance._populationModifiers == null)
                    throw new InvalidOperationException($"{nameof(_instance._populationModifiers)} was null");

                if (!_instance._terminationConditions.Any()) missingProperties.Add("termination conditions");

                if (!_instance._populationModifiers.Any()) missingProperties.Add("population modifiers");

                if (_instance._fitnessFunction == null) missingProperties.Add("fitness Function");

                if (_instance._populationInitializer == null) missingProperties.Add("population initializer");

                if (!_instance._initialPopulationSize.HasValue) missingProperties.Add("initial population size");

                if (_instance._matingFunction == null) missingProperties.Add("mating function");

                if (_instance._survivorSelector == null) missingProperties.Add("survivor selector");

                if (missingProperties.Any())
                    throw new Exceptions.GeneticSelectionProcessNotCompleted(missingProperties.ToArray());

                #endregion

                // Return a deep copy of the built object.
                return _instance;
            }
        }
    }
}