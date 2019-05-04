using System;
using System.Collections.Generic;
using System.Linq;
using GeneCore.Core.ProcessComponents;
using GeneCore.PopulationInitializers;
using JetBrains.Annotations;
using Tests.UnitTests;

namespace Tests.Shared
{
    public static class TestUtils
    {
        [NotNull]
        public static Int32[] GetRandomArrayInt32(Int32 length, Int32 start = 0, Int32 stop = 10)
        {
            var result = new Int32[length];
            Random rng = new Random();
            for (Int32 i = 0; i < length; i++) result[i] = rng.Next(start, stop);

            return result;
        }

        public static Population<TestIndividual<TGene>, TGene> GetPopulation<TGene>(Int32 populationCount
            , [NotNull] Func<TestIndividual<TGene>> individualProvider)
        {
            if (individualProvider == null) throw new ArgumentNullException(nameof(individualProvider));

            var population = new Population<TestIndividual<TGene>, TGene>();

            IList<TestIndividual<TGene>> individuals = new List<TestIndividual<TGene>>();
            for (Int32 i = 0; i < populationCount; i++)
            {
                TestIndividual<TGene> individual = individualProvider();
                individuals.Add(individual);
            }

            population.SetPopulation(individuals);
            return population;
        }

        public static TestIndividual<Int32>[] GetRandomTestIndividuals(Int32 parentCount, Int32 chromosomeSize
            , Int32 geneMin = 0, Int32 geneMax = 10)
        {
            var individuals = new TestIndividual<Int32>[parentCount];
            Random rng = new Random();
            for (Int32 i = 0; i < parentCount; i++)
            {
                individuals[i] = GetRandomTestIndividual(chromosomeSize
                    , () => ChromosomeInitializers.GetRandomInt32(rng, geneMin, geneMax));
            }

            return individuals;
        }

        public static TestIndividual<TGene> GetRandomTestIndividual<TGene>(Int32 chromosomeSize
            , [NotNull] Func<TGene> randomProvider)
        {
            if (randomProvider == null) throw new ArgumentNullException(nameof(randomProvider));

            var testIndividual = new TestIndividual<TGene>();

            var chromosome = new Chromosome<TGene>();
            chromosome.InitializeChromosome(chromosomeSize);

            var genes = new TGene[chromosomeSize];
            for (Int32 i = 0; i < chromosomeSize; i++) genes[i] = randomProvider();

            chromosome.SetChromosome(genes);
            testIndividual.SetChromosome(chromosome);

            return testIndividual;
        }

        public static Boolean AreDoublesEqual(this Double first, Double second, Double tolerance = 0.001)
        {
            return Math.Abs(first - second) < tolerance;
        }

        [NotNull]
        public static Boolean[] GetRandomArrayBoolean(Int32 length)
        {
            return GetRandomArrayInt32(length, 0, 1).Select(i => i == 1).ToArray();
        }
    }
}