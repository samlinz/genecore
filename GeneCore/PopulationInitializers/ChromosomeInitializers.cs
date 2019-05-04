using System;
using GeneCore.Core.ProcessComponents;
using JetBrains.Annotations;

namespace GeneCore.PopulationInitializers
{
    /// <summary>
    ///     Collection of static commonly used chromosome initializers.
    /// </summary>
    public static class ChromosomeInitializers
    {
        public static TChromosome GetRandomChromosome<TChromosome, TGene>(Int32 chromosomeLength
            , [NotNull] Func<Random, TGene> randomProvider) where TChromosome : IChromosome<TGene>, new()
        {
            #region Validation

            if (randomProvider == null) throw new ArgumentNullException(nameof(randomProvider));

            #endregion

            TChromosome chromosome = new TChromosome();
            chromosome.InitializeChromosome(chromosomeLength);

            Random rng = new Random();

            var randomizedChromosome = new TGene[chromosomeLength];
            for (Int32 i = 0; i < chromosomeLength; i++) randomizedChromosome[i] = randomProvider(rng);

            chromosome.SetChromosome(randomizedChromosome);

            return chromosome;
        }

        [NotNull]
        public static Func<Random, T> CreateRandomProvider<T>(T min, T max, [NotNull] Func<Random, T, T, T> provider)
        {
            return rng => provider.Invoke(rng, min, max);
        }

        [NotNull]
        public static Func<Random, T> CreateRandomProvider<T>([NotNull] Func<Random, T> provider)
        {
            return provider.Invoke;
        }

        public static Int32 GetRandomInt32([NotNull] Random rng, Int32 min, Int32 max)
        {
            return rng.Next(min, max);
        }

        public static Boolean GetRandomBoolean([NotNull] Random rng, Boolean min, Boolean max)
        {
            return rng.Next(0, 2) == 1;
        }

        public static Boolean GetRandomBoolean([NotNull] Random rng)
        {
            return rng.Next(0, 2) == 1;
        }

        public static Double GetRandomDouble([NotNull] Random rng, Double min, Double max)
        {
            return min + rng.NextDouble() * (max - min);
        }
    }
}