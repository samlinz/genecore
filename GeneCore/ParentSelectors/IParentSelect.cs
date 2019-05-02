using System;
using System.Collections.Generic;
using System.Linq;
using GeneCore.Core;
using JetBrains.Annotations;

namespace GeneCore.ParentSelectors {
    public interface IParentSelector {
        IEnumerable<IEnumerable<IIndividual>> GetParents<TFitness>(IPopulation<TFitness> population);
    }

    public class FittestParentsSelector : IParentSelector {
        private readonly Int32 _pairCount;
        private readonly Int32 _parentsPerChild;

        public FittestParentsSelector(Int32 pairCount, Int32 parentsPerChild = 2) {
            this._pairCount = pairCount;
            this._parentsPerChild = parentsPerChild;
        }

        public IEnumerable<IEnumerable<IIndividual>> GetParents<TFitness>([NotNull] IPopulation<TFitness> population) {
            if (population == null) throw new ArgumentNullException(nameof(population));

            Int32 chosenPairs = 0;
            IList<IndividualWithFitness<TFitness>> notChosenParents = population.GetPopulationByFitness();
            IList<IEnumerable<IIndividual>> chosenParents = new List<IEnumerable<IIndividual>>();

            Int32 populationSize = notChosenParents.Count;
            if (_parentsPerChild > populationSize)
                throw new InvalidOperationException($"Number of expected parents {_parentsPerChild} is greater than population size {populationSize}");

            while (notChosenParents.Count >= _parentsPerChild && chosenPairs < _pairCount) {
                // Select a pair of parents by their fitness.
                Int32 chosenParentsLength = chosenParents.Count;
                IList<IIndividual> parents = notChosenParents
                    .ToList()
                    .GetRange(chosenParentsLength - _parentsPerChild, _parentsPerChild)
                    .Select(individual => individual.GetIndividual())
                    .ToList();

                // Add the newly selected parents to result.
                chosenParents.Add(parents);

                // Pop selected parents from original collection.
                for (Int32 i = 0; i < _parentsPerChild; i++) {
                    notChosenParents.RemoveAt(notChosenParents.Count - 1);
                }
            }

            return chosenParents;
        }
    }
}