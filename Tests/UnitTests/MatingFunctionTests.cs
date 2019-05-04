using System;
using System.Collections.Generic;
using System.Linq;
using GeneCore.Core.ProcessComponents;
using GeneCore.MatingFunctions;
using Tests.Shared;
using Xunit;

namespace Tests.UnitTests
{
    public class MatingFunctionTests
    {
        [Fact]
        public void RandomMatingFunctionShouldWork()
        {
            var matingFunction = new RandomCombinationMatingFunction<TestIndividual<Int32>, Chromosome<Int32>, Int32>();

            Random rng = new Random();
            Int32 parentCount = rng.Next(2, 10);
            Int32 chromosomeSize = rng.Next(2, 10);

            matingFunction.SetChromosomeLength(chromosomeSize);

            TestIndividual<Int32>[] parents = TestUtils.GetRandomTestIndividuals(parentCount, chromosomeSize);
            List<IndividualWithFitness<TestIndividual<Int32>, Int32>> parentsWithFitnesses = parents
                .Select(testIndividual => new IndividualWithFitness<TestIndividual<Int32>, Int32>(testIndividual, 1))
                .ToList();

            TestIndividual<Int32> individual = matingFunction.Mate(parentsWithFitnesses);

            Assert.Equal(chromosomeSize, individual.GetChromosome().GetGenome().Length);
        }
    }
}