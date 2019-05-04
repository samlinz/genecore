using System;
using System.Collections.Generic;
using System.Linq;
using GeneCore.Core.ProcessComponents;
using JetBrains.Annotations;

namespace GeneCore.SurvivorSelectors
{
    public class
        ReplaceLeastFitSelector<TPopulation, TIndividual, TGene> : ISurvivorSelector<TPopulation, TIndividual, TGene>
        where TPopulation : class, IPopulation<TIndividual, TGene> where TIndividual : class, IIndividual<TGene>
    {
        public void AddOffspring([NotNull] TPopulation population, [NotNull] IList<TIndividual> offspring)
        {
            #region Validation

            if (population == null) throw new ArgumentNullException(nameof(population));
            if (offspring == null) throw new ArgumentNullException(nameof(offspring));

            #endregion

            IList<IndividualWithFitness<TIndividual, TGene>> parentPopulation = population.GetPopulationByFitness();
            Int32 offspringCount = offspring.Count();

            List<IndividualWithFitness<TIndividual, TGene>> leastFitIndividuals =
                parentPopulation.ToList().GetRange(0, offspringCount);

            for (Int32 i = 0; i < offspringCount; i++)
            {
                TIndividual replacedIndividual = leastFitIndividuals[i]?.GetIndividual() ??
                                                 throw new InvalidOperationException("Null individual to be replaced");
                TIndividual replacingIndividual = offspring[i];

                population.ReplaceIndividual(replacedIndividual, replacingIndividual);
            }
        }
    }
}