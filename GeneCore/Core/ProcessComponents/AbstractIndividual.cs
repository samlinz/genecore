using System;
using JetBrains.Annotations;

namespace GeneCore.Core.ProcessComponents
{
    /// <summary>
    ///     Abstract base class for individuals. Encloses the chromosome and provides
    ///     accessor and setter methods.
    /// </summary>
    /// <typeparam name="TGene"></typeparam>
    public abstract class AbstractIndividual<TGene> : IIndividual<TGene>
    {
        // UUID which uniquely identifies this specific individual.
        private readonly Guid _guid;

        // Individuals' chromosome.
        private IChromosome<TGene> _chromosome;

        protected AbstractIndividual()
        {
            _guid = Guid.NewGuid();
        }

        public void SetChromosome([NotNull] IChromosome<TGene> chromosome)
        {
            #region Validation

            if (chromosome == null) throw new ArgumentNullException(nameof(chromosome));
            if (!chromosome.IsInitialized()) throw new InvalidOperationException(nameof(chromosome));
            if (_chromosome != null) throw new InvalidOperationException("Already set");

            #endregion

            _chromosome = chromosome;
        }

        public Guid GetGuid()
        {
            return _guid;
        }

        public String GetGuidStringShort()
        {
            return _guid.ToString().Substring(0, 5);
        }

        public IChromosome<TGene> GetChromosome()
        {
            return _chromosome;
        }
    }
}