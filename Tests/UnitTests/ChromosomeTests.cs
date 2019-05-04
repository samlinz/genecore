using System;
using GeneCore.Core.ProcessComponents;
using Tests.Shared;
using Xunit;

namespace Tests.UnitTests
{
    public class ChromosomeTests
    {
        [Fact]
        public void ChromosomeShouldAllowSettingArbitraryRanges()
        {
            Random rng = new Random();
            Int32 geneLength = rng.Next(1, 1000);
            Int32 geneMax = 1000;

            for (Int32 i = 0; i < TestConstants.TestRepeats; i++)
            {
                var chromosome = new Chromosome<Int32>();
                chromosome.InitializeChromosome(geneLength);

                Int32[] genes = TestUtils.GetRandomArrayInt32(geneLength, 0, geneMax);
                chromosome.SetChromosome(genes);

                for (Int32 j = 0; j < 5; j++)
                {
                    Int32 subGeneLength = rng.Next(1, geneLength);
                    Int32[] subGenes = TestUtils.GetRandomArrayInt32(subGeneLength, 0, geneMax);

                    Int32 insertionPoint = rng.Next(0, geneLength - subGeneLength - 1);
                    chromosome.SetChromosome(subGenes, insertionPoint);

                    Int32[] updatedGenome = chromosome.GetGenome();

                    for (Int32 k = 0; k < subGeneLength; k++)
                        Assert.Equal(subGenes[k], updatedGenome[insertionPoint + k]);
                }
            }
        }

        [Fact]
        public void ChromosomeShouldInitializeAndGenesFetch()
        {
            Int32 geneLength = 100;

            for (Int32 test = 0; test < TestConstants.TestRepeats; test++)
            {
                var chromosome = new Chromosome<Int32>();
                chromosome.InitializeChromosome(geneLength);

                Int32[] originalGenes = TestUtils.GetRandomArrayInt32(geneLength);
                chromosome.SetChromosome(originalGenes, 0);

                Int32[] setGenes = chromosome.GetGeneRange(0, geneLength);

                Assert.NotSame(originalGenes, setGenes);

                // Test fetching single gene.
                for (Int32 i = 0; i < setGenes.Length; i++)
                {
                    Assert.Equal(originalGenes[i], setGenes[i]);
                    Assert.Equal(originalGenes[i], chromosome.GetGeneAt(i));
                }

                Random rng = new Random();

                // Test fetching random ranges.
                for (Int32 i = 0; i < 5; i++)
                {
                    Int32 rangeStart = rng.Next(0, geneLength - 1);
                    Int32 rangeStop = rng.Next(rangeStart, geneLength);
                    Int32 length = rangeStop - rangeStart;

                    Int32[] range = chromosome.GetGeneRange(rangeStart, length);
                    for (Int32 j = 0; j < length; j++) Assert.Equal(originalGenes[rangeStart + j], range[j]);
                }
            }
        }

        [Fact]
        public void ChromosomeShouldNotAllowInvalidArray()
        {
            var chromosome = new Chromosome<Int32>();
            chromosome.InitializeChromosome(5);

            Assert.True(chromosome.IsInitialized());

            Assert.Throws<ArgumentNullException>(() => chromosome.SetChromosome(null));
            Assert.Throws<ArgumentException>(() => chromosome.SetChromosome(new Int32[]
                { }));
        }

        [Fact]
        public void ChromosomeShouldThrowIfAccessedUninitialized()
        {
            var chromosome = new Chromosome<Boolean>();
            Assert.Throws<InvalidOperationException>(() => chromosome.GetGeneRange(0, 1));
            Assert.Throws<InvalidOperationException>(() => chromosome.GetGeneAt(0));
            Assert.Throws<InvalidOperationException>(() => chromosome.SetChromosome(new Boolean[5]));
        }
    }
}