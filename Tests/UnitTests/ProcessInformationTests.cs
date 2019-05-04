using System;
using System.Collections.Generic;
using System.Linq;
using GeneCore.Core;
using GeneCore.Core.ProcessComponents;
using GeneCore.PopulationInitializers;
using Tests.Shared;
using Xunit;

namespace Tests.UnitTests
{
    public class ProcessInformationTests
    {
        [Fact]
        public void ShouldComposeNewFirstProcessInformation()
        {
            Random rng = new Random();
            Population<TestIndividual<Int32>, Int32> population = TestUtils.GetPopulation(10
                , () => TestUtils.GetRandomTestIndividual(10, () => ChromosomeInitializers.GetRandomInt32(rng, 0, 10)));

            population.OrderByFitness(new PopulationTests.TestFitnessFunctionInt32());

            var composer =
                new ProcessInformationComposer<Population<TestIndividual<Int32>, Int32>, TestIndividual<Int32>, Int32
                >();

            ProcessInformation newProcessInformation = composer.ComposeProcessInformation(null, population);

            Assert.Equal((UInt64) 1, newProcessInformation.Generation);

            Double expectedTotalFitness = population
                .GetPopulationByFitness().Select(fitness => fitness.GetFitness()).Sum();

            Assert.Equal(expectedTotalFitness, newProcessInformation.TotalFitness);
        }

        [Fact]
        public void ShouldComposeNewProcessInformationForNewGeneration()
        {
            Random rng = new Random();
            for (Int32 i = 0; i < TestConstants.TestRepeats; i++)
            {
                Int32 populationSize = rng.Next(1, 1000);
                Int32 geneSize = rng.Next(1, 1000);

                Population<TestIndividual<Int32>, Int32> GetPopulation()
                {
                    var population = new Population<TestIndividual<Int32>, Int32>();
                    IList<TestIndividual<Int32>> individuals = new List<TestIndividual<Int32>>();

                    for (Int32 j = 0; j < populationSize; j++)
                    {
                        var newIndividual = new TestIndividual<Int32>();

                        Chromosome<Int32> newChromosome =
                            ChromosomeInitializers.GetRandomChromosome<Chromosome<Int32>, Int32>(geneSize
                                , rng_ => ChromosomeInitializers.GetRandomInt32(rng_, 0, 10));

                        newIndividual.SetChromosome(newChromosome);
                        individuals.Add(newIndividual);
                    }

                    population.SetPopulation(individuals);
                    population.OrderByFitness(new PopulationTests.TestFitnessFunctionInt32());

                    return population;
                }

                Population<TestIndividual<Int32>, Int32> population1 = GetPopulation();
                Population<TestIndividual<Int32>, Int32> population2 = GetPopulation();

                var composer =
                    new ProcessInformationComposer<Population<TestIndividual<Int32>, Int32>, TestIndividual<Int32>,
                        Int32>();

                ProcessInformation firstProcessInformation = composer.ComposeProcessInformation(null, population1);
                ProcessInformation secondProcessInformation =
                    composer.ComposeProcessInformation(firstProcessInformation, population2);

                Assert.Equal(firstProcessInformation.Generation, secondProcessInformation.Generation - 1);

                Assert.Equal(firstProcessInformation.TotalFitnessDelta, firstProcessInformation.TotalFitness);
                Assert.Equal(firstProcessInformation.FittestFitnessDelta, firstProcessInformation.FittestFitness);
                Assert.Equal(firstProcessInformation.FittestNFitnessDelta, firstProcessInformation.FittestNFitness);

                Assert.Equal(secondProcessInformation.TotalFitnessDelta
                    , secondProcessInformation.TotalFitness - firstProcessInformation.TotalFitness);
                Assert.Equal(secondProcessInformation.FittestFitnessDelta
                    , secondProcessInformation.FittestFitness - firstProcessInformation.FittestFitness);
                Assert.Equal(secondProcessInformation.FittestNFitnessDelta
                    , secondProcessInformation.FittestNFitness - firstProcessInformation.FittestNFitness);
            }
        }
    }
}