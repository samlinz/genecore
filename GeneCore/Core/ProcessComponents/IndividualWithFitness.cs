using System;

namespace GeneCore.Core.ProcessComponents
{
    /// <summary>
    ///     Container for an individual combined with its fitness score.
    /// </summary>
    /// <typeparam name="TIndividual"></typeparam>
    /// <typeparam name="TGene"></typeparam>
    public class IndividualWithFitness<TIndividual, TGene> where TIndividual : IIndividual<TGene>
    {
        private readonly Double _fitness;
        private readonly TIndividual _individual;

        public IndividualWithFitness(TIndividual individual, Double fitness)
        {
            _individual = individual;
            _fitness = fitness;
        }

        public Double GetFitness()
        {
            return _fitness;
        }

        public TIndividual GetIndividual()
        {
            return _individual;
        }
    }
}