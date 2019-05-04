using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace GeneCore.Core.ProcessComponents
{
    /// <summary>
    ///     Concrete basic implementation of a population containing individuals of type <typeparamref name="TIndividual" />.
    ///     Works for the most cases as is, but can be inherited to provide additional functionality.
    /// </summary>
    /// <typeparam name="TIndividual"></typeparam>
    /// <typeparam name="TGene"></typeparam>
    public class Population<TIndividual, TGene> : IPopulation<TIndividual, TGene> where TIndividual : IIndividual<TGene>
    {
        private const String ExceptionNotOrdered = "Population is not ordered";

        // Flag indicating that the population has changed and ordering is not valid.
        private volatile Boolean _orderingDirty = true;

        // The entire population mapped to UUIDs.
        private IDictionary<Guid, TIndividual> _population;

        // Population ordered by fitness.
        private IList<IndividualWithFitness<TIndividual, TGene>> _populationOrdered;

        // Population with fitnesses.
        private IDictionary<Guid, IndividualWithFitness<TIndividual, TGene>> _populationWithFitnesses;

        public IEnumerable<TIndividual> GetIndividuals()
        {
            #region Validation

            if (_population == null) throw new InvalidOperationException($"{nameof(_population)} not set");

            #endregion

            return _population.Values.ToList();
        }

        public TIndividual GetIndividual(Guid uuid)
        {
            #region Validation

            if (_population == null) throw new InvalidOperationException($"{nameof(_population)} not set");
            if (!_population.ContainsKey(uuid))
                throw new InvalidOperationException($"{uuid} not found from population");

            #endregion

            return _population[uuid];
        }

        public IndividualWithFitness<TIndividual, TGene> GetIndividualWithFitness(Guid uuid)
        {
            #region Validation

            if (_orderingDirty) throw new InvalidOperationException(ExceptionNotOrdered);
            if (_population == null) throw new InvalidOperationException($"{nameof(_population)} not set");
            if (_populationWithFitnesses == null)
                throw new InvalidOperationException($"{nameof(_populationWithFitnesses)} not set");
            if (!_populationWithFitnesses.ContainsKey(uuid))
                throw new InvalidOperationException($"{uuid} not found from population");

            #endregion

            return _populationWithFitnesses[uuid];
        }

        public void SetPopulation(IEnumerable<TIndividual> population)
        {
            #region Validation

            if (_population != null) throw new InvalidOperationException("Already set");

            #endregion

            _population = population.Select(individual => (individual.GetGuid(), individual))
                .ToDictionary(tuple => tuple.Item1, tuple => tuple.Item2);

            // Ordering is off by default, invalidate.
            _orderingDirty = true;
        }

        public void OrderByFitness([NotNull] IFitnessFunction<TIndividual, TGene> fitnessFunction)
        {
            #region Validation

            if (_population == null) throw new InvalidOperationException($"{nameof(_population)} not set");
            if (fitnessFunction == null) throw new ArgumentNullException(nameof(fitnessFunction));

            #endregion

            // If the population is already ordered, return.
            if (!_orderingDirty) return;

            // Calculate fitness values to each individual in population.
            List<(TIndividual, Double)> populationWithFitnesses = _population.Values
                .Select(individual => (individual, fitnessFunction.CalculateFitness(individual))).ToList();

            // Sort list by fitness using fitness object's own comparer.
            populationWithFitnesses = populationWithFitnesses.OrderBy(tuple => tuple.Item2).ToList();

            _populationOrdered = populationWithFitnesses.Select(
                tuple => new IndividualWithFitness<TIndividual, TGene>(tuple.Item1, tuple.Item2)).ToList();

            // Save individuals with fitness to hash map for fast lookup.
            _populationWithFitnesses = _populationOrdered.ToDictionary(tuple => tuple.GetIndividual().GetGuid()
                , tuple => tuple);

            // Unset flag to indicate the population is ordered.
            _orderingDirty = false;
        }

        public IList<IndividualWithFitness<TIndividual, TGene>> GetPopulationByFitness()
        {
            #region Validation

            if (_orderingDirty) throw new InvalidOperationException(ExceptionNotOrdered);
            if (_populationOrdered == null)
                throw new InvalidOperationException($"{nameof(_populationOrdered)} was null");

            #endregion

            return new List<IndividualWithFitness<TIndividual, TGene>>(_populationOrdered);
        }

        public IList<IndividualWithFitness<TIndividual, TGene>> GetNFittest(Int32 n)
        {
            #region Validation

            if (_orderingDirty) throw new InvalidOperationException(ExceptionNotOrdered);
            if (n <= 0) throw new ArgumentOutOfRangeException($"{nameof(n)}: {n}");
            ;
            if (_populationOrdered == null)
                throw new InvalidOperationException($"{nameof(_populationOrdered)} was null");
            if (!_populationOrdered.Any())
                throw new InvalidOperationException($"{nameof(_populationOrdered)} was empty");

            #endregion

            Int32 populationSize = _populationOrdered.Count;
            return _populationOrdered.ToList().GetRange(populationSize - n, n);
        }

        public IndividualWithFitness<TIndividual, TGene> GetFittest()
        {
            return GetNFittest(1)?.Single() ?? throw new InvalidOperationException("Fittest individual not found");
        }

        public void ReplaceIndividual([NotNull] TIndividual replaced, [NotNull] TIndividual replacing)
        {
            #region Validation

            if (_population == null) throw new InvalidOperationException($"{nameof(_population)} not set");
            if (replaced == null) throw new ArgumentNullException(nameof(replaced));
            if (replacing == null) throw new ArgumentNullException(nameof(replacing));

            #endregion

            Guid replacedUuid = replaced.GetGuid();
            Guid replacingUuid = replacing.GetGuid();

            if (_population.ContainsKey(replacingUuid))
                throw new InvalidOperationException($"Replacing UUID {replacedUuid} was already found in population");

            if (!_population.Remove(replacedUuid))
                throw new InvalidOperationException($"Replaced UUID {replacedUuid} was not found in population");

            _population.Add(replacingUuid, replacing);

            // Invalidate ordering.
            _orderingDirty = true;
        }
    }
}