using System;
using System.Collections.Generic;
using System.Linq;
using GeneCore.Core.ProcessComponents;
using Tests.Shared;
using Xunit;

namespace Tests.UnitTests
{
    public class FitnessFunctionTests
    {
        [Fact]
        public void TestFitnessFunctionShouldWork()
        {
            Random rng = new Random();
            Int32 genomeLength = rng.Next(1, 1000);

            // Test fitness on boolean genome.
            for (Int32 i = 0; i < 5; i++)
            {
                Double expectedFitness = 0;

                var individual = new TestIndividual<Boolean>();
                var chromosome = new Chromosome<Boolean>();
                chromosome.InitializeChromosome(genomeLength);

                IList<Boolean> testChromosome = new List<Boolean>();
                for (Int32 j = 0; j < genomeLength; j++)
                {
                    Boolean gene = rng.Next(2) == 1;
                    if (gene) expectedFitness++;
                    testChromosome.Add(gene);
                }

                chromosome.SetChromosome(testChromosome.ToArray());

                individual.SetChromosome(chromosome);

                TestFitnessFunctionBoolean fitnessFunction = new TestFitnessFunctionBoolean();
                Double fitness = fitnessFunction.CalculateFitness(individual);

                Assert.True(fitness.AreDoublesEqual(expectedFitness));
            }

            // Test fitness on double genome.
            for (Int32 i = 0; i < 5; i++)
            {
                Double expectedFitness = 0;

                var individual = new TestIndividual<Double>();
                var chromosome = new Chromosome<Double>();
                chromosome.InitializeChromosome(genomeLength);

                IList<Double> testChromosome = new List<Double>();
                for (Int32 j = 0; j < genomeLength; j++)
                {
                    Double gene = rng.NextDouble() * 5;
                    expectedFitness += gene;
                    testChromosome.Add(gene);
                }

                chromosome.SetChromosome(testChromosome.ToArray());

                individual.SetChromosome(chromosome);

                TestFitnessFunctionDouble fitnessFunction = new TestFitnessFunctionDouble();
                Double fitness = fitnessFunction.CalculateFitness(individual);

                Assert.True(fitness.AreDoublesEqual(expectedFitness));
            }
        }
    }

    internal class TestFitnessFunctionBoolean : IFitnessFunction<TestIndividual<Boolean>, Boolean>
    {
        public Double CalculateFitness(TestIndividual<Boolean> individual)
        {
            Boolean[] genome = individual.GetChromosome().GetGenome();
            return genome.Select(b => b ? 1 : 0).Sum();
        }
    }

    internal class TestFitnessFunctionDouble : IFitnessFunction<TestIndividual<Double>, Double>
    {
        public Double CalculateFitness(TestIndividual<Double> individual)
        {
            Double[] genome = individual.GetChromosome().GetGenome();
            return genome.Sum();
        }
    }
}