using GeneCore.Core;

namespace GeneCore.FitnessFunctions {
    /// <summary>
    /// Fitness function that represents the 'fitness' or score of the
    /// provided individual deriving from <typeparamref name="TIndividual"/>.
    /// This is the result of the 'objective function' that GA attempts to maximize.
    /// </summary>
    public interface IFitnessFunction {
        IFitness<T> CalculateFitness<T>(IIndividual individual);
    }
}