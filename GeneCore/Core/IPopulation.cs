using System;
using System.Collections.Generic;
using System.Linq;
using GeneCore.FitnessFunctions;
using JetBrains.Annotations;

namespace GeneCore.Core {
    public interface IPopulation<TFitness> {
        void OrderByFitness(IFitnessFunction fitnessFunction);
        [NotNull]
        IList<IndividualWithFitness<TFitness>> GetPopulationByFitness();
        [NotNull]
        IList<IndividualWithFitness<TFitness>> GetNFittest(Int32 n);
        [NotNull]
        IndividualWithFitness<TFitness> GetFittest();
        void ReplaceIndividual(IIndividual replaced, IIndividual replacing);
    }

    public class Population<TFitness> : IPopulation<TFitness> {
        private const String ExceptionNotOrdered = "Population is not ordered";

        // The entire population mapped to UUIDs.
        [NotNull]
        private readonly IDictionary<Guid, IIndividual> _population;

        // Population ordered by fitness.
        private IList<IndividualWithFitness<TFitness>> _populationOrdered;

        // Flag indicating that the population has changed and ordering is not valid.
        private volatile Boolean _orderingDirty;

        public Population([NotNull] IDictionary<Guid, IIndividual> population, Boolean isOrdered) {
            _population = population ?? throw new ArgumentNullException(nameof(population));
            _orderingDirty = isOrdered;
        }

        public void OrderByFitness([NotNull] IFitnessFunction fitnessFunction) {
            if (fitnessFunction == null) throw new ArgumentNullException(nameof(fitnessFunction));

            // If the population is already ordered, return.
            if (!_orderingDirty) return;

            // Calculate fitness values to each individual in population.
            List<(IIndividual, IFitness<TFitness>)> populationWithFitnesses = _population.Values.Select(individual => (individual, fitnessFunction.CalculateFitness<TFitness>(individual))).ToList();

            // Sort list by fitness using fitness object's own comparer.
            populationWithFitnesses.Sort((individual1, individual2) => individual1.Item2.CompareTo(individual2.Item2));

            _populationOrdered = populationWithFitnesses.Select(tuple => new IndividualWithFitness<TFitness>(tuple.Item1, tuple.Item2)).ToList();

            // Unset flag to indicate the population is ordered.
            _orderingDirty = false;
        }

        public IList<IndividualWithFitness<TFitness>> GetPopulationByFitness() {
            if (_orderingDirty) throw new InvalidOperationException(ExceptionNotOrdered);
            if (_populationOrdered == null) throw new InvalidOperationException($"{nameof(_populationOrdered)} was null");

            return new List<IndividualWithFitness<TFitness>>(_populationOrdered);
        }

        public IList<IndividualWithFitness<TFitness>> GetNFittest(Int32 n) {
            #region Validation

            if (_orderingDirty) throw new InvalidOperationException(ExceptionNotOrdered);
            if (_populationOrdered == null) throw new InvalidOperationException($"{nameof(_populationOrdered)} was null");
            if (!_populationOrdered.Any()) throw new InvalidOperationException($"{nameof(_populationOrdered)} was empty");

            #endregion

            Int32 populationSize = _populationOrdered.Count;
            return _populationOrdered.ToList().GetRange(populationSize - n, n);
        }

        public IndividualWithFitness<TFitness> GetFittest() => GetNFittest(1)?.Single() ?? throw new InvalidOperationException($"Fittest individual not found");

        public void ReplaceIndividual([NotNull] IIndividual replaced, [NotNull] IIndividual replacing) {
            #region Validation

            if (replaced == null) throw new ArgumentNullException(nameof(replaced));
            if (replacing == null) throw new ArgumentNullException(nameof(replacing));

            #endregion

            Guid replacedUuid = replaced.GetGuid();
            Guid replacingUuid = replacing.GetGuid();

            if (_population.ContainsKey(replacingUuid))
                throw new InvalidOperationException($"Replacing UUID {replacedUuid} was already found in population");

            if (_population.Remove(replacedUuid))
                throw new InvalidOperationException($"Replaced UUID {replacedUuid} was already found in population");

            _population.Add(replacingUuid, replacing);

            // Invalidate ordering.
            _orderingDirty = true;
        }
    }
}