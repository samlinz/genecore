using JetBrains.Annotations;

namespace GeneCore.Core.ProcessComponents
{
    /// <summary>
    ///     A function which modifies the population at given intervals. This function is arbitrary but the most
    ///     common things to do is to do crossover, mutations etc. but it can be implemented to do anything you
    ///     want to try.
    /// </summary>
    public interface IPopulationModifier<TPopulation, TIndividual, TGene>
        where TPopulation : IPopulation<TIndividual, TGene> where TIndividual : IIndividual<TGene>
    {
        /// <summary>
        ///     The modifier function which takes in the unmodified population and returns the new, modified
        ///     one.
        /// </summary>
        /// <param name="originalPopulation">The original <see cref="IPopulation{T}" /> which is modified.</param>
        /// <returns>The modified <see cref="IPopulation{T}" /> that will be passed to next modifier.</returns>
        [NotNull]
        TPopulation ModifyPopulation(TPopulation originalPopulation);
    }
}