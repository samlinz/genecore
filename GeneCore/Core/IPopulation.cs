using System.Collections.Generic;
using GeneCore.FitnessFunctions;

namespace GeneCore.Core {
    public interface IPopulation<T> where T : IIndividual {
        /// <summary>
        /// Sort individuals in the population according to provided fitness function and return a sorted
        /// list of fitness-individual pairs.
        /// </summary>
        /// <param name="fitnessFunction"></param>
        /// <returns></returns>
        SortedList<IFitness<TU>, T> SortByFitnessDescending<TU>(IFitnessFunction<T> fitnessFunction);
    }
}