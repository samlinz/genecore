using System;
using System.Collections.Generic;
using System.Linq;
using GeneCore.Core;
using JetBrains.Annotations;

namespace GeneCore.SurvivorSelectors {
    public interface ISurvivorSelector {
        void AddOffspring<TFitness>(IPopulation<TFitness> population, IList<IIndividual> offspring);
    }

    public class ReplaceLeastFitSelector : ISurvivorSelector {
        public void AddOffspring<TFitness>([NotNull] IPopulation<TFitness> population, [NotNull] IList<IIndividual> offspring) {
            #region Validation

            if (population == null) throw new ArgumentNullException(nameof(population));
            if (offspring == null) throw new ArgumentNullException(nameof(offspring));

            #endregion

            IList<IndividualWithFitness<TFitness>> parentPopulation = population.GetPopulationByFitness();
            Int32 offspringCount = offspring.Count();

            List<IndividualWithFitness<TFitness>> leastFitIndividuals = parentPopulation.ToList().GetRange(0, offspringCount);
            for (Int32 i = 0; i < offspringCount; i++) {
                IIndividual replacedIndividual = leastFitIndividuals[i].GetIndividual();
                IIndividual replacingIndividual = offspring[i];
                
                population.ReplaceIndividual(replacedIndividual, replacingIndividual);
            }
        }
    }
}