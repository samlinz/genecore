using System;

namespace GeneCore.Core.ProcessComponents
{
    /// <summary>
    ///     <para>
    ///         Function which implements the population initializer functionality.
    ///         The function does what it does and returns the initial population.
    ///     </para>
    ///     <para>
    ///         A thing to consider when doing your own custom initializer is that heuristics doesn't really
    ///         outperform random initialization. The best thing to do is to maximize diversity.
    ///     </para>
    /// </summary>
    public interface IPopulationInitializer<out TPopulation, TIndividual, TGene>
        where TPopulation : IPopulation<TIndividual, TGene> where TIndividual : IIndividual<TGene>
    {
        TPopulation InitializePopulation(Int32 populationSize);
    }
}