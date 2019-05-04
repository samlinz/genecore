using System;
using System.Threading;
using JetBrains.Annotations;

namespace GeneCore.Core.ProcessComponents
{
    /// <summary>
    ///     Represents a chromosome that belongs to an individual.
    ///     A chromosome contains a string of genes which are represented by arbitrary type <typeparamref name="TGene" />.
    ///     Chromosome can be of arbitrary length and encodes the entire genotype of an individual.
    /// </summary>
    public interface IChromosome<TGene>
    {
        void InitializeChromosome(Int32 size);
        void SetChromosome(TGene[] genes, Int32 start = 0);
        Boolean IsInitialized();
        TGene GetGeneAt(Int32 position);
        void SetGeneAt(Int32 position, TGene gene);
        TGene[] GetGeneRange(Int32 start, Int32 stop);

        [NotNull]
        TGene[] GetGenome();
    }

    /// <inheritdoc />
    /// <summary>
    ///     Basic concrete implementation of a chromosome. Contains the chromosome of an individual
    ///     internally stored in an array. This array can be fetched and manipulated as a whole or
    ///     by individual genes.
    /// </summary>
    /// <typeparam name="TGene"></typeparam>
    public class Chromosome<TGene> : IChromosome<TGene>
    {
        private const String ChromosomeNotInitializedMessage = "Chromosome is not initialized";

        [NotNull]
        private readonly ReaderWriterLockSlim _genomeLock = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);

        private TGene[] _genome;

        public void InitializeChromosome(Int32 size)
        {
            #region Validation

            if (size == 0) throw new ArgumentOutOfRangeException(nameof(size));

            #endregion

            _genome = new TGene[size];
        }

        public Boolean IsInitialized()
        {
            return _genome != null;
        }

        /// <summary>
        ///     Write an array of genes to chromosome.
        /// </summary>
        /// <param name="genes">Genes to write.</param>
        /// <param name="start">Start to write from the index in internal chromosome.</param>
        /// <exception cref="IndexOutOfRangeException">Tried to write outside the chromosome.</exception>
        public void SetChromosome([NotNull] TGene[] genes, Int32 start = 0)
        {
            #region Validation

            if (_genome == null) throw new InvalidOperationException("Chromosome is not initialized");
            if (genes == null) throw new ArgumentNullException(nameof(genes));
            if (genes.Length == 0) throw new ArgumentException("Provided chromosome was empty");

            #endregion

            Int32 genesLength = genes.Length;
            Int32 stopPoint = start + genesLength;

            #region Validate

            if (start < 0) throw new IndexOutOfRangeException(OutOfRangeMessage(start));
            if (stopPoint > _genome.Length) throw new IndexOutOfRangeException(OutOfRangeMessage(stopPoint));

            #endregion

            _genomeLock.EnterWriteLock();

            try
            {
                // Copy the entire provided array to internal chromosome starting from the provided point.
                Array.Copy(genes, 0, _genome, start, genesLength);
            } finally
            {
                _genomeLock.ExitWriteLock();
            }
        }

        /// <summary>
        ///     Return a single gene at given 0-base index.
        /// </summary>
        /// <param name="position">The index of the gene.</param>
        /// <returns>The gene at given point.</returns>
        /// <exception cref="IndexOutOfRangeException">Point was outside the chromosome.</exception>
        public TGene GetGeneAt(Int32 position)
        {
            #region Validation

            if (_genome == null) throw new InvalidOperationException(ChromosomeNotInitializedMessage);

            #endregion

            if (position < 0 || position >= _genome.Length)
                throw new IndexOutOfRangeException(OutOfRangeMessage(position));

            return _genome[position];
        }

        public void SetGeneAt(Int32 position, TGene gene)
        {
            #region Validation

            if (_genome == null) throw new InvalidOperationException(ChromosomeNotInitializedMessage);
            if (position < 0 || position >= _genome.Length)
                throw new ArgumentOutOfRangeException(nameof(position));

            #endregion

            _genome[position] = gene;
        }

        /// <summary>
        ///     Return a range of genes.
        /// </summary>
        /// <param name="start">Start index.</param>
        /// <param name="length">Length or returned sequential genes.</param>
        /// <returns>Array of genes of type <typeparamref name="TGene" />.</returns>
        /// <exception cref="IndexOutOfRangeException">Tried to read outside the chromosome.</exception>
        [NotNull]
        public TGene[] GetGeneRange(Int32 start, Int32 length)
        {
            #region Validation

            if (_genome == null) throw new InvalidOperationException(ChromosomeNotInitializedMessage);

            if (start < 0) throw new IndexOutOfRangeException(OutOfRangeMessage(start));

            if (start + length > _genome.Length) throw new IndexOutOfRangeException(OutOfRangeMessage(start + length));

            #endregion

            var resultArray = new TGene[length];
            Array.Copy(_genome, start, resultArray, 0, length);
            return resultArray;
        }

        public TGene[] GetGenome()
        {
            #region Validation

            if (_genome == null) throw new InvalidOperationException(ChromosomeNotInitializedMessage);

            #endregion

            return GetGeneRange(0, _genome.Length);
        }

        private String OutOfRangeMessage(Int32 index)
        {
            return $"Position {index} out of genome range, genome length {_genome?.Length}";
        }
    }
}