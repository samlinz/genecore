using System;
using System.Collections.Generic;
using System.Linq;
using GeneCore.Core;
using GeneCore.Core.ProcessComponents;
using GeneCore.MatingFunctions;
using GeneCore.ParentSelectors;
using GeneCore.PopulationInitializers;
using GeneCore.PopulationModifiers;
using GeneCore.SurvivorSelectors;
using GeneCore.TerminationConditions;
using Tests.Shared;
using Xunit;

namespace Tests.TestRuns
{
    public class TrivialMaximizeTruesProcess
    {
        [Fact]
        public void TrivialMaximizeTruesProcessShouldRun()
        {
            Random rng = new Random();

            Int32 genomeSize = 100;
            Int32 targetFitness = 90;
            Int32 populationSize = 100;
            Double mutationRate = 0.01;

            var matingFunctions = new List<IMatingFunction<TestIndividual<Boolean>, Chromosome<Boolean>, Boolean>>
            {
                new CrossOverMatingFunction<TestIndividual<Boolean>, Chromosome<Boolean>, Boolean>(1)
                , new CrossOverMatingFunction<TestIndividual<Boolean>, Chromosome<Boolean>, Boolean>(3)
                , new RandomCombinationMatingFunction<TestIndividual<Boolean>, Chromosome<Boolean>, Boolean>()
            };

            foreach (IMatingFunction<TestIndividual<Boolean>, Chromosome<Boolean>, Boolean> matingFunction in
                matingFunctions)
            {
                var builder =
                    new GeneticSelectionProcess<Population<TestIndividual<Boolean>, Boolean>, TestIndividual<Boolean>,
                        Chromosome<Boolean>, Boolean, ProcessInformation>.Builder();

                var process = builder.SetInitialPopulationSize(populationSize).UseDefaultConsoleLogger()
                    .SetProcessInformationComposer(
                        new ProcessInformationComposer<Population<TestIndividual<Boolean>, Boolean>,
                            TestIndividual<Boolean>, Boolean>()).SetMatingFunction(matingFunction)
                    .SetParentSelector(
                        new SingleGroupFittestParentsSelector<Population<TestIndividual<Boolean>, Boolean>,
                            TestIndividual<Boolean>, Boolean>()).SetFitnessFunction(new TestFitnessFunction())
                    .SetSurvivorSelector(
                        new ReplaceLeastFitSelector<Population<TestIndividual<Boolean>, Boolean>,
                            TestIndividual<Boolean>, Boolean>())
                    .SetPopulationInitializer(
                        new BooleanRandomPopulationInitializer<Population<TestIndividual<Boolean>, Boolean>,
                            TestIndividual<Boolean>, Chromosome<Boolean>>(genomeSize))
                    .AddTerminationCondition(
                        new FitnessTerminator<Population<TestIndividual<Boolean>, Boolean>, TestIndividual<Boolean>,
                            Boolean>(targetFitness))
                    .AddPopulationModifier(
                        new RandomMutationModifier<Population<TestIndividual<Boolean>, Boolean>, TestIndividual<Boolean>
                            , Boolean>(mutationRate, () => rng.Next(2) == 1)).Build();

                process.Initialize();
                process.RunUntilConvergence();

                IndividualWithFitness<TestIndividual<Boolean>, Boolean> fittest = process.GetPopulation().GetFittest();
                Double fittestFitness = fittest.GetFitness();
                Boolean[] fittestGenome = fittest.GetIndividual().GetChromosome().GetGenome();

                Double expectedFitness = fittestGenome.Select(b => b ? 1 : 0).Sum();
                
                Assert.True(fittestFitness >= 90);
                Assert.True(expectedFitness >= 90);
                Assert.True(expectedFitness.AreDoublesEqual(fittestFitness));
                
                IList<ProcessInformation> history = process.GetHistory();
                Assert.True(history.Count > 1);
                Assert.True(fittestFitness.AreDoublesEqual(history[history.Count - 1].FittestFitness));
            }
        }
    }

    internal class TestFitnessFunction : IFitnessFunction<TestIndividual<Boolean>, Boolean>
    {
        public Double CalculateFitness(TestIndividual<Boolean> individual)
        {
            return individual.GetChromosome().GetGenome().Select(b => b ? 1 : 0).Sum();
        }
    }

    internal class TestIndividual<Boolean> : AbstractIndividual<Boolean> { }
}