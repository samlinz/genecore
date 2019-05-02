using System;
using System.Threading;
using JetBrains.Annotations;

namespace GeneCore.Core {
    /// <summary>
    /// Represents the genotype of an individual.
    /// </summary>
    public interface IChromosome<out T> {
        T GetGeneAt(Int32 position);
        T[] GetGeneRange(Int32 start, Int32 stop);
    }

    /// <summary>
    /// Base class from basic chromosome in which the genome is represented internally
    /// as an array of arbitrary genes.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class AbstractChromosome<T> : IChromosome<T> {
        [NotNull]
        private readonly T[] _genome;
        [NotNull]
        private readonly ReaderWriterLockSlim _genomeLock = new ReaderWriterLockSlim(
            LockRecursionPolicy.NoRecursion); 

        protected AbstractChromosome(Int32 genomeLength) {
            _genome = new T[genomeLength];
        }

        /// <summary>
        /// Write an array of genes to chromosome.
        /// </summary>
        /// <param name="genes">Genes to write.</param>
        /// <param name="start">Start to write from the index in internal chromosome.</param>
        /// <exception cref="IndexOutOfRangeException">Tried to write outside the chromosome.</exception>
        public void SetChromosome([NotNull] T[] genes, Int32 start = 0) {
            Int32 genesLength = genes.Length;
            Int32 stopPoint = start + genesLength;

            #region Validate

            if (start < 0) {
                throw new IndexOutOfRangeException(OutOfRangeMessage(start));
            }
            if (stopPoint >= _genome.Length) {
                throw new IndexOutOfRangeException(OutOfRangeMessage(stopPoint));
            }

            #endregion

            _genomeLock.EnterWriteLock();

            try {
                // Copy the entire provided array to internal chromosome starting from the provided point.
                Array.Copy(genes, 0, _genome, start, genesLength);
            } finally {
                _genomeLock.ExitWriteLock();
            }
        }

        /// <summary>
        /// Return a single gene at given 0-base index.
        /// </summary>
        /// <param name="position">The index of the gene.</param>
        /// <returns>The gene at given point.</returns>
        /// <exception cref="IndexOutOfRangeException">Point was outside the chromosome.</exception>
        public T GetGeneAt(Int32 position) {
            if (position < 0 || position >= _genome.Length) {
                throw new IndexOutOfRangeException(OutOfRangeMessage(position));                
            }
            
            return _genome[position];
        }

        /// <summary>
        /// Return a range of genes.
        /// </summary>
        /// <param name="start">Start index.</param>
        /// <param name="length">Length or returned sequential genes.</param>
        /// <returns>Array of genes of type <typeparamref name="T"/>.</returns>
        /// <exception cref="IndexOutOfRangeException">Tried to read outside the chromosome.</exception>
        public T[] GetGeneRange(Int32 start, Int32 length) {
            if (start < 0) {
                throw new IndexOutOfRangeException(OutOfRangeMessage(start));                
            }

            if (start + length >= _genome.Length) {
                throw new IndexOutOfRangeException(OutOfRangeMessage(start + length));
            }
            
            T[] resultArray = new T[length];
            Array.Copy(_genome, start, resultArray, 0, length);
            return resultArray;
        }

        private String OutOfRangeMessage(Int32 index)
            => $"Position {index} out of genome range, genome length {_genome.Length}";
    }
}