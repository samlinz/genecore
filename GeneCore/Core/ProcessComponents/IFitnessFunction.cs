using System;

namespace GeneCore.Core.ProcessComponents
{
    /// <summary>
    ///     Fitness function that calculates the 'fitness' of an individual of type <typeparamref name="TIndividual" />.
    ///     Fitness is analogous to the score of the individual, or how well it solves the problem.
    ///     Fitness is of type double and generally higher is better. This score is used by parent selectors, termination
    ///     conditions etc. to evaluate individuals and the population as a whole.
    /// </summary>
    public interface IFitnessFunction<in TIndividual, TGene> where TIndividual : IIndividual<TGene>
    {
        Double CalculateFitness(TIndividual individual);
    }
}