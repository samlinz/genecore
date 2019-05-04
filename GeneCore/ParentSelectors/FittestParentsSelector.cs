using System;
using System.Collections.Generic;
using System.Linq;
using GeneCore.Core.ProcessComponents;
using JetBrains.Annotations;

namespace GeneCore.ParentSelectors
{
    public abstract class
        AbstractFittestParentsSelector<TPopulation, TIndividual, TGene> : IParentSelector<TPopulation, TIndividual,
            TGene> where TPopulation : IPopulation<TIndividual, TGene> where TIndividual : IIndividual<TGene>
    {
        /*  If the algorithm is unable to find unique combination of parents for N iterations,
            accept anything to avoid infinite loop (and calculating permutations).
            This is because either coincidence or invalid termination condition (should have converged already).
        */
        private const Int32 UniqueParentPatience = 2;
        protected readonly Int32 _pairCount;
        protected readonly Int32 _parentsPerChild;

        protected AbstractFittestParentsSelector(Int32 pairCount, Int32 parentsPerChild)
        {
            _pairCount = pairCount;
            _parentsPerChild = parentsPerChild;
        }

        public IEnumerable<IEnumerable<TIndividual>> GetParents([NotNull] TPopulation population)
        {
            if (population == null) throw new ArgumentNullException(nameof(population));

            // All parents available for selection.
            IList<IndividualWithFitness<TIndividual, TGene>> parentPool = population.GetPopulationByFitness();
            // Identifiers for selected pairs.
            ISet<String> chosenPairs = new HashSet<String>();

            if (parentPool.Count < _parentsPerChild)
            {
                throw new InvalidOperationException($"There are less individuals ({parentPool.Count}) " +
                                                    $"than the required parent count ({_parentsPerChild})");
            }

            // List of chosen parents.
            IList<IEnumerable<TIndividual>> chosenParents = new List<IEnumerable<TIndividual>>();

            Int32 populationSize = parentPool.Count;
            if (_parentsPerChild > populationSize)
            {
                throw new InvalidOperationException($"Number of expected parents {_parentsPerChild} is " +
                                                    $"greater than population size {populationSize}");
            }

            Int32 reattempts = 0;
            while (chosenParents.Count < _pairCount)
            {
                // Select group/groups of parents by their fitness.
                Boolean breakOutAfter = !SelectNewParents(parentPool, chosenParents, out IList<TIndividual> parents
                    , out parentPool);

                if (parents == null) throw new InvalidOperationException($"{nameof(parents)} was null");

                // Create string identifier for this specific combination and verify
                // that this combination hasn't already been created.
                String parentCombinationIdentifier = string.Join(","
                    , parents.OrderBy(i => i.GetGuid()).Select(individual => individual.GetGuid()));

                if (chosenPairs.Contains(parentCombinationIdentifier))
                {
                    if (reattempts++ < UniqueParentPatience) continue;
                }

                reattempts = 0;

                chosenPairs.Add(parentCombinationIdentifier);

                // Add the newly selected parents to result.
                chosenParents.Add(parents);

                if (breakOutAfter || StopPicking()) break;
            }

            return chosenParents;
        }

        /// <summary>
        ///     Override to provide additional end condition check after
        ///     new parents have been chosen.
        /// </summary>
        /// <returns>True to stop picking parents.</returns>
        protected virtual Boolean StopPicking()
        {
            return false;
        }

        /// <summary>
        ///     Implement to provide implementation specific state validation before picking
        ///     anything.
        /// </summary>
        /// <returns>True if valid state.</returns>
        public abstract Boolean ValidateState();

        /// <summary>
        ///     Implement to provide specific method of picking new parents.
        /// </summary>
        /// <param name="parentPool"></param>
        /// <param name="alreadyChosenParents"></param>
        /// <param name="selectedParents"></param>
        /// <param name="newParentPool"></param>
        /// <returns></returns>
        protected abstract Boolean SelectNewParents(IList<IndividualWithFitness<TIndividual, TGene>> parentPool
            , IList<IEnumerable<TIndividual>> alreadyChosenParents, out IList<TIndividual> selectedParents
            , out IList<IndividualWithFitness<TIndividual, TGene>> newParentPool);
    }

    /// <summary>
    ///     Parent selector which picks a single pair/group of parents based on fitness, selecting
    ///     the N fittest individuals as parents.
    /// </summary>
    /// <typeparam name="TPopulation"></typeparam>
    /// <typeparam name="TIndividual"></typeparam>
    /// <typeparam name="TGene"></typeparam>
    public class
        SingleGroupFittestParentsSelector<TPopulation, TIndividual, TGene> : AbstractFittestParentsSelector<TPopulation,
            TIndividual, TGene> where TPopulation : IPopulation<TIndividual, TGene>
        where TIndividual : IIndividual<TGene>
    {
        public SingleGroupFittestParentsSelector(Int32 parentsPerChild = 2) : base(1, parentsPerChild) { }

        public override Boolean ValidateState()
        {
            return true;
        }

        protected override Boolean SelectNewParents(
            [NotNull] IList<IndividualWithFitness<TIndividual, TGene>> parentPool
            , IList<IEnumerable<TIndividual>> alreadyChosenParents, out IList<TIndividual> selectedParents
            , out IList<IndividualWithFitness<TIndividual, TGene>> newParentPool)
        {
            #region Validation

            if (parentPool == null) throw new ArgumentNullException(nameof(parentPool));

            #endregion

            IList<TIndividual> parents = parentPool.ToList()
                .GetRange(parentPool.Count - _parentsPerChild, _parentsPerChild)
                .Select(individual => individual.GetIndividual()).ToList();

            selectedParents = parents;
            newParentPool = parentPool;

            // Don't continue picking.
            return false;
        }
    }
}