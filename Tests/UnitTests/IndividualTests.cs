using System;
using GeneCore.Core.ProcessComponents;
using Xunit;

namespace Tests.UnitTests
{
    public class IndividualTests
    {
        [Fact]
        public void ShouldFetchChromosome()
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

            IChromosome<Int32> containedChromosome = individual.GetChromosome();
            Assert.Equal(chromosome.GetGenome(), containedChromosome.GetGenome());
        }

        [Fact]
        public void ShouldNotAllowUnInitializedChromosome()
        {
            var chromosome = new Chromosome<Int32>();
            var individual = new TestIndividual<Int32>();

            var chromosome2 = new Chromosome<Int32>();
            chromosome2.InitializeChromosome(3);
            chromosome2.SetChromosome(new[]
            {
                1
                , 2
                , 3
            });

            Assert.Throws<ArgumentNullException>(() => individual.SetChromosome(null));
            Assert.Throws<InvalidOperationException>(() => individual.SetChromosome(chromosome));

            individual.SetChromosome(chromosome2);
            Assert.Throws<InvalidOperationException>(() => individual.SetChromosome(chromosome2));
        }
    }

    public class TestIndividual<T> : AbstractIndividual<T> { }
}