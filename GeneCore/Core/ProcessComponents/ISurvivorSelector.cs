using System.Collections.Generic;

namespace GeneCore.Core.ProcessComponents
{
    /// <summary>
    ///     Method which adds the generated new individuals (offspring) into the population and
    ///     usually removes some of the lesser fit individuals.
    /// </summary>
    /// <typeparam name="TPopulation"></typeparam>
    /// <typeparam name="TIndividual"></typeparam>
    /// <typeparam name="TGene"></typeparam>
    public interface ISurvivorSelector<in TPopulation, TIndividual, TGene>
        where TPopulation : IPopulation<TIndividual, TGene> where TIndividual : IIndividual<TGene>
    {
        /// <summary>
        ///     Provided with the current population and the new offspring,
        ///     creates and returns the population for the next generation.
        /// </summary>
        /// <param name="population"></param>
        /// <param name="offspring"></param>
        void AddOffspring(TPopulation population, IList<TIndividual> offspring);
    }
}