using GeneCore.Core;
using JetBrains.Annotations;

namespace GeneCore.PopulationModifiers {
    /// <summary>
    /// A function which modifies the population at given intervals. This function is arbitrary but the most
    /// common things to do is to do crossover, mutations etc. but it can be implemented to do anything you
    /// want to try.
    /// </summary>
    public interface IPopulationModifier {
        /// <summary>
        /// The modifier function which takes in the unmodified population and returns the new, modified
        /// one.
        /// </summary>
        /// <param name="originalPopulation">The original <see cref="IPopulation{T}"/> which is modified.</param>
        /// <returns>The modified <see cref="IPopulation{T}"/> that will be passed to next modifier.</returns>
        
        [NotNull]
        IPopulation<T> ModifyPopulation<T>(IPopulation<T> originalPopulation) where T : IIndividual;
    }
}