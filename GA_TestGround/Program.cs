/*
GA()
   initialize population
   find fitness of population
   
   while (termination criteria is reached) do
      parent selection
      crossover with probability pc
      mutation with probability pm
      decode and fitness calculation
      survivor selection
      find best
   return best
*/

/* todo
 * population initialization, random or custom heuristics, mix heuristics and random
 * maximize diversity in initial population
 * get the diversity of population
 * paralellization
 * logging
 *
 * incremental ga
 *     one or two offspring per generation, replace worst
 * generational
 *     replace entire population with len(population) offsprings
 *
 * fitness proportional selection
 *     chance of becoming parent propotional to fitness score
 *
 * parent selection stragegies
 *     roulette wheel selection
 *     sum fitnesses
 *     get random n
 *     start adding
 *
 * stocastic universal sampling
 *     multiple points at once
 *
 * tournanment selection
 *     select k from population, take fittest
 *     repeat
 *     works with negative 
 *
 * rankselection
 *     works with negative fitnesses
 *     when fitness values very close to each other
 *     rank all individuals, use rank as proportion
 *
 * random selection
 *
 * modifications
 * crossover
 *     probability, usually high
 *
 * mutation
 *     low probability
 *
 * survivor selection
 *     elitism, fittest always survives
 * kick randomly
 * kick after n generations
 * replace least fit
 *
 *
 * termination condidition
 *     fitness improved
 *     max time
 *
 * checkpoints!!!!!
 *     save best
 *     allow warm start from serialized
 *     every n generations
 *     absolute number of generations
 *     no improvement in fitness
 *     pre defined value for objective function
 *
 *
 * local search
 *     check solutions in neighborhood
 *
 * hybridize ga with local search
 *
 * constrained problems
 *     not all solutions feasible
 *
 * penalty for infeasible
 *     proportioinal to constraints violated
 *     distance from feasible region
 *
 * repair functions
 *     fix the infeasible solution
 *
 * not allow infeasible at all
 */

using System;
using System.Linq;
using GeneCore.Core;
using GeneCore.Core.ProcessComponents;
using GeneCore.MatingFunctions;
using GeneCore.ParentSelectors;
using GeneCore.PopulationInitializers;
using GeneCore.PopulationModifiers;
using GeneCore.SurvivorSelectors;
using GeneCore.TerminationConditions;

namespace GA_TestGround
{
    internal class Program
    {
        public static void Main(String[] args)
        {
            GeneticSelectionProcess<Population<TestIndividual<Boolean>, Boolean>, TestIndividual<Boolean>,
                Chromosome<Boolean>, Boolean, ProcessInformation>.Builder builder =
                new GeneticSelectionProcess<Population<TestIndividual<Boolean>, Boolean>, TestIndividual<Boolean>,
                    Chromosome<Boolean>, Boolean, ProcessInformation>.Builder();

            Random rng = new Random();

            Int32 genomeSize = 100;
            Int32 targetFitness = 90;
            Int32 fitnessDeltaTolerance = 10;

            Int32 populationSize = 1000;
            Double mutationRate = 0.01;

            GeneticSelectionProcess<Population<TestIndividual<Boolean>, Boolean>, TestIndividual<Boolean>,
                Chromosome<Boolean>, Boolean, ProcessInformation> process = builder
                .SetInitialPopulationSize(populationSize).UseDefaultConsoleLogger()
                .SetProcessInformationComposer(
                    new ProcessInformationComposer<Population<TestIndividual<Boolean>, Boolean>, TestIndividual<Boolean>
                        , Boolean>())
                .SetMatingFunction(
                    new CrossOverMatingFunction<TestIndividual<Boolean>, Chromosome<Boolean>, Boolean>(3))
//                    new RandomCombinationMatingFunction<
//                        TestIndividual<Boolean>
//                        , Chromosome<Boolean>
//                        , Boolean>())
                .SetParentSelector(
                    new SingleGroupFittestParentsSelector<Population<TestIndividual<Boolean>, Boolean>,
                        TestIndividual<Boolean>, Boolean>()).SetFitnessFunction(new TestFitnessFunction())
                .SetSurvivorSelector(
                    new ReplaceLeastFitSelector<Population<TestIndividual<Boolean>, Boolean>, TestIndividual<Boolean>,
                        Boolean>())
                .SetPopulationInitializer(
                    new BooleanRandomPopulationInitializer<Population<TestIndividual<Boolean>, Boolean>,
                        TestIndividual<Boolean>, Chromosome<Boolean>>(genomeSize))
                .AddTerminationCondition(
                    new FitnessTerminator<Population<TestIndividual<Boolean>, Boolean>, TestIndividual<Boolean>, Boolean
                    >(targetFitness))
                .AddPopulationModifier(
                    new RandomMutationModifier<Population<TestIndividual<Boolean>, Boolean>, TestIndividual<Boolean>,
                        Boolean>(mutationRate, () => rng.Next(2) == 1)).Build();

            process.Initialize();
            process.RunUntilConvergence();

            IndividualWithFitness<TestIndividual<Boolean>, Boolean> fittest = process.GetPopulation().GetFittest();
            Double fittestFitness = fittest.GetFitness();
            Boolean[] fittestGenome = fittest.GetIndividual().GetChromosome().GetGenome();

            Console.WriteLine($"Max fitness {fittestFitness}");
            Console.WriteLine($"Genome {string.Join("", fittestGenome.Select(b => b ? 1 : 0))}");
            Console.WriteLine("Done");
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