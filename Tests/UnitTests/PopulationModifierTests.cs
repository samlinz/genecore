using System;
using System.Linq;
using GeneCore.Common;
using GeneCore.Core.ProcessComponents;
using GeneCore.PopulationInitializers;
using GeneCore.PopulationModifiers;
using Tests.Shared;
using Xunit;

namespace Tests.UnitTests
{
    public class PopulationModifierTests
    {
        [Fact]
        public void MutatorModifierShouldMutateGenome()
        {
            Random rng = new Random();

            Int32 RandomGeneProvider()
            {
                return ChromosomeInitializers.GetRandomInt32(rng, 0, 10);
            }

            for (Int32 r = 0; r < TestConstants.TestRepeats; r++)
            {
                Double mutationRatio = rng.NextDouble();

                var mutator =
                    new RandomMutationModifier<Population<TestIndividual<Int32>, Int32>, TestIndividual<Int32>, Int32>(
                        mutationRatio, RandomGeneProvider);

                Int32 chromosomeSize = 10000;
                Int32 populationCount = 100;
                Int32 geneMin = 0;
                Int32 geneMax = 100;

                Population<TestIndividual<Int32>, Int32> population = TestUtils.GetPopulation(populationCount
                    , () => TestUtils.GetRandomTestIndividual(chromosomeSize
                        , () => ChromosomeInitializers.GetRandomInt32(rng, geneMin, geneMax)));

                Int32[][] originalGenes = population.GetIndividuals()
                    .Select(individual => individual.GetChromosome().GetGenome()).ToArray();
                originalGenes = Utils.DeepCopy(originalGenes);

                Population<TestIndividual<Int32>, Int32> modifiedPopulation = mutator.ModifyPopulation(population);

                Int32 modified = 0;
                Int32 unmodified = 0;

                TestIndividual<Int32>[] modifiedIndividuals = modifiedPopulation.GetIndividuals().ToArray();

                for (Int32 i = 0; i < modifiedIndividuals.Length; i++)
                {
                    Int32[] modifiedGenes = modifiedIndividuals[i].GetChromosome().GetGenome();

                    for (Int32 j = 0; j < originalGenes.Length; j++)
                    {
                        if (originalGenes[i][j] == modifiedGenes[j]) unmodified++;
                        else modified++;
                    }
                }

                // Verify that the ratio of mutated genes is about the same as specified to modifier.
                Double mutatedRatio = (Double) modified / (modified + unmodified);
                mutatedRatio.AreDoublesEqual(mutationRatio, 0.02);
            }
        }
    }
}