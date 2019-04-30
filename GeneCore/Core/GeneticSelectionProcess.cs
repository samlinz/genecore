using System;
using System.Collections.Generic;
using System.Linq;
using GeneCore.FitnessFunctions;
using GeneCore.PopulationInitializers;
using GeneCore.PopulationModifiers;
using GeneCore.TerminationConditions;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace GeneCore.Core {
    public class GeneticSelectionProcess<TFitness, TIndividual> where TIndividual : IIndividual {
        // Search termination conditions.
        [NotNull]
        private IList<ITerminationCondition> _terminationConditions;
        // Population modifiers like crossover etc.
        [NotNull]
        private IList<IPopulationModifier> _populationModifiers;
        // Function which evaluates fitness for each individual.
        [NotNull]
        private IFitnessFunction<TFitness> _fitnessFunction;

        private IProcessInformation _previousProcessInformation;
        // Population.
        [NotNull]
        private IPopulation<TIndividual> _population;

        private IPopulationInitializer<TIndividual> _populationInitializer;

        // Logger object. 
        [NotNull]
        private IList<ILogger> _loggers;
        private LogLevels _logLevels = LogLevels.Nothing;

        private GeneticSelectionProcess() {
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

            public void 
            
            public void UseDefaultConsoleLogger(Boolean usePrefix = true) {
                _instance._loggers.Add(new SimpleConsoleLogger(usePrefix));
            }
            
            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
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

                if (missingProperties.Any()) {
                    throw new GeneticSelectionProcessNotCompleted(missingProperties.ToArray());
                }

                #endregion
                
                // Return a deep copy of the built object.
                var serialized = JsonConvert.SerializeObject(_instance);
                var copy = JsonConvert.DeserializeObject<GeneticSelectionProcess<TFitness, TIndividual>>(serialized);
                return copy;
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