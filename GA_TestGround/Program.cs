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

namespace GA_TestGround
{
  internal class Program
  {
    public static void Main(string[] args)
    {
    }
  }
}