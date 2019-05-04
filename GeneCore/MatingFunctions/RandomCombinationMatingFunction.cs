using System;
using System.Collections.Generic;
using System.Linq;
using GeneCore.Core.ProcessComponents;
using JetBrains.Annotations;

namespace GeneCore.MatingFunctions
{
    public class
        RandomCombinationMatingFunction<TIndividual, TChromosome, TGene> : IMatingFunction<TIndividual, TChromosome,
            TGene> where TIndividual : IIndividual<TGene>, new() where TChromosome : IChromosome<TGene>, new()
    {
        [NotNull]
        private readonly Random _rng = new Random();

        private Int32? _offspringChromosomeLength;

        public TIndividual Mate([NotNull] IList<TIndividual> parents)
        {
            #region Validation

            if (parents == null) throw new ArgumentNullException(nameof(parents));
            if (parents.Count() < 2) throw new ArgumentOutOfRangeException(nameof(parents));

            #endregion

            TIndividual offspring = new TIndividual();
            TChromosome chromosome = new TChromosome();

            // Get parent genomes to 2d array.
            TGene[][] parentGenomes = parents.Select(p => p.GetChromosome().GetGenome()).ToArray();

            Int32 parentsCount = parentGenomes.Length;

            // If no chromosome length has been set, use the smallest length from parents.
            Int32 chromosomeLength = _offspringChromosomeLength ?? parentGenomes.Select(genes => genes.Length).Min();

            chromosome.InitializeChromosome(chromosomeLength);
            var offspringGenome = new TGene[chromosomeLength];

            for (Int32 i = 0; i < chromosomeLength; i++)
            {
                // Pick random parent and get their gene and add it to offspring genome.
                Int32 chosenParent = _rng.Next(0, parentsCount);
                TGene[] chosenGenome = parentGenomes[chosenParent];
                offspringGenome[i] = chosenGenome[i];
            }

            chromosome.SetChromosome(offspringGenome);
            offspring.SetChromosome(chromosome);

            return offspring;
        }

        public void SetChromosomeLength(Int32 chromosomeLength)
        {
            _offspringChromosomeLength = chromosomeLength;
        }
    }
}