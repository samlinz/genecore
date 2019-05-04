using System;
using System.Collections.Generic;
using System.Linq;
using GeneCore.Core.ProcessComponents;
using JetBrains.Annotations;

namespace GeneCore.PopulationModifiers
{
    public class
        RandomMutationModifier<TPopulation, TIndividual, TGene> : IPopulationModifier<TPopulation, TIndividual, TGene>
        where TPopulation : IPopulation<TIndividual, TGene>, new() where TIndividual : IIndividual<TGene>
    {
        private readonly Double _mutationChangePerGene;

        [NotNull]
        private readonly Func<TGene> _newGeneProvider;

        public RandomMutationModifier(Double mutationChangePerGene, [NotNull] Func<TGene> newGeneProvider)
        {
            #region Validation

            if (mutationChangePerGene < 0 || mutationChangePerGene > 1)
                throw new ArgumentOutOfRangeException(nameof(mutationChangePerGene));
            if (newGeneProvider == null) throw new ArgumentNullException(nameof(newGeneProvider));

            #endregion

            _mutationChangePerGene = mutationChangePerGene;
            _newGeneProvider = newGeneProvider;
        }

        public TPopulation ModifyPopulation([NotNull] TPopulation originalPopulation)
        {
            #region Validation

            if (originalPopulation == null) throw new ArgumentNullException(nameof(originalPopulation));

            #endregion

            Random rng = new Random();

            IEnumerable<TIndividual> individuals = originalPopulation.GetIndividuals().ToList();

            foreach (TIndividual individual in individuals)
            {
                IChromosome<TGene> individualChromosome = individual.GetChromosome();
                Int32 individualGenomeLength = individualChromosome.GetGenome().Length;

                for (Int32 i = 0; i < individualGenomeLength; i++)
                {
                    Boolean mutate = rng.NextDouble() < _mutationChangePerGene;

                    if (!mutate) continue;

                    // Mutate a gene by replacing it with one provided by the injected function.
                    TGene newGene = _newGeneProvider();
                    individualChromosome.SetGeneAt(i, newGene);
                }
            }

            TPopulation newPopulation = new TPopulation();
            newPopulation.SetPopulation(individuals);

            return newPopulation;
        }
    }
}