using System;
using System.Collections.Generic;
using System.Linq;
using GeneCore.Common;
using GeneCore.Core.ProcessComponents;
using JetBrains.Annotations;

namespace GeneCore.MatingFunctions
{
    public class
        CrossOverMatingFunction<TIndividual, TChromosome, TGene> : IMatingFunction<TIndividual, TChromosome, TGene>
        where TIndividual : class, IIndividual<TGene>, new() where TChromosome : IChromosome<TGene>, new()
    {
        private readonly Int32 _crossOverPoints;

        public CrossOverMatingFunction(Int32 crossOverPoints)
        {
            _crossOverPoints = crossOverPoints;
        }

        public TIndividual Mate([NotNull] IList<TIndividual> parents)
        {
            #region Validation

            if (parents == null) throw new ArgumentNullException(nameof(parents));

            // TODO: Is it valid case to have small number of points but large number of parents?
            // if (parents.Count > _crossOverPoints) throw new ArgumentNullException(nameof(parents));

            #endregion

            Random rng = new Random();

            TGene[][] parentGenomes = parents.Select(parent => parent.GetChromosome().GetGenome()).ToArray();

            Int32 smallestLength = parentGenomes.Select(genes => genes.Length).Min();

            // Select N unique crossover points.
            IList<Int32> crossOverPoints = new List<Int32>();
            while (crossOverPoints.Count < _crossOverPoints)
            {
                Int32 crossoverPointCandidate = rng.Next(smallestLength);
                if (crossOverPoints.Contains(crossoverPointCandidate))
                    continue;

                crossOverPoints.Add(crossoverPointCandidate);
            }

            // Order crossover points to ascending order
            crossOverPoints = crossOverPoints.OrderBy(i => i).ToList();

            // Construct new gene piece by piece.
            var newGene = new List<TGene>();
            Boolean continueConstructing = true;

            TGene[] previousIndividualChoice = null;
            Int32 previousCrossoverPoint = 0;

            while (continueConstructing)
            {
                TGene[] chosenParentGenome = parentGenomes.ChooseRandom(rng);
                if (chosenParentGenome == null)
                    throw new ArgumentNullException("parentGenomes.ChooseRandom(rng)");

                // Don't pick piece from same parent twice in a row.
                if (previousIndividualChoice == chosenParentGenome) continue;
                previousIndividualChoice = chosenParentGenome;

                // Take one or two crossover points to define the subsequence boundaries.
                Int32 nextCrossoverPoint = crossOverPoints.Count > 0 ? crossOverPoints[0] : chosenParentGenome.Length;

                // Pop the point from array.
                if (crossOverPoints.Count > 0) crossOverPoints.RemoveAt(0);
                else continueConstructing = false;

                Int32 pieceLength = nextCrossoverPoint - previousCrossoverPoint;

                // Get subsequence limited by points.
                TGene[] sequence = chosenParentGenome.ToList().GetRange(previousCrossoverPoint, pieceLength).ToArray();

                previousCrossoverPoint = nextCrossoverPoint;
                newGene.AddRange(sequence);
            }

            TChromosome newChromosome = new TChromosome();
            newChromosome.InitializeChromosome(newGene.Count);
            newChromosome.SetChromosome(newGene.ToArray());

            TIndividual offspring = new TIndividual();
            offspring.SetChromosome(newChromosome);

            return offspring;
        }
    }
}