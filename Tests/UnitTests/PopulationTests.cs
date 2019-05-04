using System;
using System.Collections.Generic;
using System.Linq;
using GeneCore.Core.ProcessComponents;
using GeneCore.PopulationInitializers;
using Tests.Shared;
using Xunit;

// ReSharper disable PossibleNullReferenceException

namespace Tests.UnitTests
{
    public class PopulationTests
    {
        internal class TestFitnessFunctionInt32 : IFitnessFunction<TestIndividual<Int32>, Int32>
        {
            public Double CalculateFitness(TestIndividual<Int32> individual)
            {
                Int32[] genome = individual.GetChromosome().GetGenome();
                return genome.Sum();
            }
        }

        [Fact]
        public void ShouldAllowAccessAfterInitialization()
        {
            IPopulation<TestIndividual<Int32>, Int32> population = new Population<TestIndividual<Int32>, Int32>();

            Random rng = new Random();
            Int32 populationSize = rng.Next(1, 1000);


            var individuals = new List<TestIndividual<Int32>>();
            for (Int32 i = 0; i < populationSize; i++)
            {
                var individual = new TestIndividual<Int32>();
                var chromosome = new Chromosome<Int32>();
                chromosome.InitializeChromosome(3);
                chromosome.SetChromosome(new[]
                {
                    1
                    , 2
                    , 3
                });
                individual.SetChromosome(chromosome);
                individuals.Add(individual);
            }

            population.SetPopulation(individuals);
            IEnumerable<TestIndividual<Int32>> containedPopulation = population.GetIndividuals();

            Assert.Equal(populationSize, containedPopulation.Count());
        }

        [Fact]
        public void ShouldNotAllowAccessBeforeInitialization()
        {
            IPopulation<TestIndividual<Int32>, Int32> population = new Population<TestIndividual<Int32>, Int32>();

            Assert.Throws<InvalidOperationException>(() => population.GetIndividuals());
        }

        [Fact]
        public void ShouldNotAllowAccessToDirtyOrdering()
        {
            Random rng = new Random();
            Population<TestIndividual<Int32>, Int32> population = TestUtils.GetPopulation(100
                , () => TestUtils.GetRandomTestIndividual(100
                    , () => ChromosomeInitializers.GetRandomInt32(rng, 0, 10)));

            Assert.Throws<InvalidOperationException>(() => population.GetFittest());
            Assert.Throws<InvalidOperationException>(() => population.GetNFittest(1));
            Assert.Throws<InvalidOperationException>(() => population.GetPopulationByFitness());
        }

        [Fact]
        public void ShouldOrderByFitness()
        {
            Random rng = new Random();

            for (Int32 i = 0; i < TestConstants.TestRepeats; i++)
            {
                Int32 populationSize = rng.Next(1, 1000);
                Int32 chromosomeSize = rng.Next(1, 1000);

                Population<TestIndividual<Double>, Double> population = TestUtils.GetPopulation(populationSize
                    , () => TestUtils.GetRandomTestIndividual(chromosomeSize
                        , () => ChromosomeInitializers.GetRandomDouble(rng, 0, 100)));

                List<(TestIndividual<Double> individual, Double)> individuals = population.GetIndividuals()
                    .Select(individual => (individual, individual.GetChromosome().GetGenome().Sum())).ToList();

                individuals = individuals.OrderBy(tuple => tuple.Item2).ToList();

                (TestIndividual<Double> individual, Double)[] expectedOrdering = individuals.ToArray();

                population.OrderByFitness(new TestFitnessFunctionDouble());

                TestIndividual<Double>[] orderedPopulation = population.GetPopulationByFitness()
                    .Select(individual => individual.GetIndividual()).ToArray();

                // Check that the ordering is identical.
                for (Int32 j = 0; j < expectedOrdering.Length; j++)
                    Assert.Equal(expectedOrdering[j].Item1.GetGuid(), orderedPopulation[j].GetGuid());

                // Check that fetches the fittest individual.
                Assert.Equal(expectedOrdering[expectedOrdering.Length - 1].Item1.GetGuid()
                    , population.GetFittest().GetIndividual().GetGuid());

                // Check that fetches N fittest individuals.
                IndividualWithFitness<TestIndividual<Double>, Double>[] twoFittest =
                    population.GetNFittest(2).ToArray();
                Assert.Equal(expectedOrdering[expectedOrdering.Length - 1].Item1.GetGuid()
                    , twoFittest[twoFittest.Length - 1].GetIndividual().GetGuid());
                Assert.Equal(expectedOrdering[expectedOrdering.Length - 2].Item1.GetGuid()
                    , twoFittest[twoFittest.Length - 2].GetIndividual().GetGuid());
            }
        }
    }
}