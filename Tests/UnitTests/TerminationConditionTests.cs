using System;
using GeneCore.Core;
using GeneCore.Core.ProcessComponents;
using GeneCore.PopulationInitializers;
using GeneCore.TerminationConditions;
using Tests.Shared;
using Xunit;

namespace Tests.UnitTests
{
    public class TerminationConditionTests
    {
        [Fact]
        public void ShouldTerminateBasedOnGeneration()
        {
            Random rng = new Random();

            Population<TestIndividual<Int32>, Int32> population = TestUtils.GetPopulation(10
                , () => TestUtils.GetRandomTestIndividual(10, () => ChromosomeInitializers.GetRandomInt32(rng, -5, 5)));
            population.OrderByFitness(new PopulationTests.TestFitnessFunctionInt32());

            for (Int32 i = 0; i < TestConstants.TestRepeats; i++)
            {
                Int32 maxRun = 1000;
                Int32 terminationPoint = rng.Next(1, maxRun);

                var composer =
                    new ProcessInformationComposer<Population<TestIndividual<Int32>, Int32>, TestIndividual<Int32>,
                        Int32>();
                var terminator =
                    new GenerationsCountTerminator<Population<TestIndividual<Int32>, Int32>, TestIndividual<Int32>,
                        Int32>((UInt64) terminationPoint);

                ProcessInformation previousProcessInformation = null;

                for (Int32 j = 1; j < maxRun; j++)
                {
                    ProcessInformation newProcessInformation =
                        composer.ComposeProcessInformation(previousProcessInformation, population);
                    previousProcessInformation = newProcessInformation;

                    Boolean shouldTerminate = terminator.ShouldTerminate(population, newProcessInformation);
                    if (j >= terminationPoint) Assert.True(shouldTerminate);
                    else Assert.False(shouldTerminate);
                }
            }
        }
    }

    public class MockFitnessFunction<TGene> : IFitnessFunction<TestIndividual<TGene>, TGene>
    {
        private readonly Double _fixedFitness;

        public MockFitnessFunction(Double fixedFitness)
        {
            _fixedFitness = fixedFitness;
        }

        public Double CalculateFitness(TestIndividual<TGene> individual)
        {
            return _fixedFitness;
        }
    }
}