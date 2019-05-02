using System;
using System.Net.Http.Headers;

namespace GeneCore.Core {
    public interface IIndividual {
        Guid GetGuid();
    }

    public class IndividualWithFitness<T> {
        private readonly IIndividual _individual;
        private readonly IFitness<T> _fitness;

        public IndividualWithFitness(IIndividual individual, IFitness<T> fitness) {
            _individual = individual;
            _fitness = fitness;
        }

        public IFitness<T> GetFitness() => _fitness;

        public IIndividual GetIndividual() => _individual;
    }

    public abstract class AbstractIndividual<TGene> : IIndividual {
        // UUID which uniquely identifies this specific individual.
        private readonly Guid _guid;
        // Individuals' chromosome.
        private readonly IChromosome<TGene> _chromosome;

        protected AbstractIndividual(IChromosome<TGene> chromosome) {
            _guid = Guid.NewGuid();
            _chromosome = chromosome;
        }

        public Guid GetGuid() {
            return _guid;
        }

        public IChromosome<TGene> GetChromosome() {
            return _chromosome;
        }
    }
}