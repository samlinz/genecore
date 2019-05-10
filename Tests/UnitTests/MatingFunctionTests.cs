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
            TestIndividual<Int32> individual = matingFunction.Mate(parents);

            Assert.Equal(chromosomeSize, individual.GetChromosome().GetGenome().Length);
        }
    }
}