using GeneCore.Core;

namespace GeneCore.PopulationInitializers {
    /// <summary>
    /// Function which implements the population initializer functionality.
    /// The function does what it does and returns the initial population.
    ///
    /// A thing to consider when doing your own custom initializer is that heuristics doesn't really
    /// outperform random initialization. The best thing to do is to maximize diversity.
    /// </summary>
    public interface IPopulationInitializer<T> where T : IIndividual {
        IPopulation<T> InitializePopulation();
    }
}