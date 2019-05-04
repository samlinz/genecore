using System;
using System.Collections.Generic;
using System.Linq;
using GeneCore.Core.ProcessComponents;
using GeneCore.PopulationInitializers;
using Xunit;

namespace Tests.UnitTests
{
    public class PopulationInitializerTests
    {
        /// <summary>
        ///     Test that random chromosome initializer works with different types and ranges.
        /// </summary>
        [Fact]
        public void RandomChromosomeInitializerShouldWork()
        {
            Random rng = new Random();

            Int32 chromosomeLength = rng.Next(1, 1000_000);

            Int32 geneMin = rng.Next(1, 1000_000);
            Int32 geneMax = rng.Next(geneMin, 1000_000);

            // Int32
            Func<Random, Int32> randomProvider = ChromosomeInitializers.CreateRandomProvider(geneMin
                , geneMax, ChromosomeInitializers.GetRandomInt32);

            Chromosome<Int32> chromosome =
                ChromosomeInitializers.GetRandomChromosome<Chromosome<Int32>, Int32>(chromosomeLength, randomProvider);

            Int32[] fullRange = chromosome.GetGeneRange(0, chromosomeLength);

            Assert.Equal(chromosomeLength, fullRange.Length);

            foreach (Int32 gene in fullRange)
            {
                Assert.True(gene >= geneMin);
                Assert.True(gene <= geneMax);
            }

            // Boolean
            Func<Random, Boolean> randomProvider2 = ChromosomeInitializers.CreateRandomProvider(false
                , true, ChromosomeInitializers.GetRandomBoolean);

            Chromosome<Boolean> chromosome2 =
                ChromosomeInitializers.GetRandomChromosome<Chromosome<Boolean>, Boolean>(chromosomeLength
                    , randomProvider2);

            Boolean[] fullRange2 = chromosome2.GetGeneRange(0, chromosomeLength);

            Assert.Equal(chromosomeLength, fullRange2.Length);

            // Double
            Func<Random, Double> randomProvider3 = ChromosomeInitializers.CreateRandomProvider(Convert.ToDouble(geneMin)
                , Convert.ToDouble(geneMax), ChromosomeInitializers.GetRandomDouble);

            Chromosome<Double> chromosome3 =
                ChromosomeInitializers.GetRandomChromosome<Chromosome<Double>, Double>(chromosomeLength
                    , randomProvider3);

            Double[] fullRange3 = chromosome3.GetGeneRange(0, chromosomeLength);

            Assert.Equal(chromosomeLength, fullRange3.Length);
        }

        [Fact]
        public void RandomPopulationInitializerShouldWork()
        {
            Random rng = new Random();

            Int32 chromosomeLength = rng.Next(1, 1000);
            Int32 geneMin = rng.Next(0, 1000_000);
            Int32 geneMax = rng.Next(geneMin, 1000_000);

            Int32 populationSize = rng.Next(1, 1000);

            var initializer =
                new Int32RandomPopulationInitializer<Population<TestIndividual<Int32>, Int32>, TestIndividual<Int32>,
                    Chromosome<Int32>>(chromosomeLength, geneMin, geneMax);

            Population<TestIndividual<Int32>, Int32> population = initializer.InitializePopulation(populationSize);
            List<TestIndividual<Int32>> individuals = population.GetIndividuals().ToList();

            Assert.Equal(populationSize, individuals.Count());
        }
    }
}