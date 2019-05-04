using System;
using System.Collections.Generic;
using System.Linq;
using GeneCore.Core.ProcessComponents;
using GeneCore.ParentSelectors;
using GeneCore.PopulationInitializers;
using Tests.Shared;
using Xunit;

namespace Tests.UnitTests
{
    public class ParentSelectorTests
    {
        [Fact]
        public void ShouldFetchFittestIndividualsAsParents()
        {
            Random rng = new Random();

            for (Int32 i = 0; i < TestConstants.TestRepeats; i++)
            {
                Int32 populationSize = rng.Next(2, 1000);

                Population<TestIndividual<Int32>, Int32> population = TestUtils.GetPopulation(populationSize
                    , () => TestUtils.GetRandomTestIndividual(10
                        , () => ChromosomeInitializers.GetRandomInt32(rng, 0, 10)));

                population.OrderByFitness(new PopulationTests.TestFitnessFunctionInt32());

                var selector =
                    new SingleGroupFittestParentsSelector<Population<TestIndividual<Int32>, Int32>,
                        TestIndividual<Int32>, Int32>(2);

                IList<IEnumerable<TestIndividual<Int32>>> selectedParents = selector.GetParents(population).ToList();
                IEnumerable<TestIndividual<Int32>> selectedParentPair = selectedParents.First();

                Assert.Equal(1, selectedParents.Count);

                IList<IndividualWithFitness<TestIndividual<Int32>, Int32>> twoFittest = population.GetNFittest(2);

                foreach (IndividualWithFitness<TestIndividual<Int32>, Int32> fitIndividual in twoFittest)
                    Assert.Contains(fitIndividual.GetIndividual(), selectedParentPair);
            }
        }
    }
}