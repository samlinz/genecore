using System;
using System.Collections.Generic;
using GeneCore.Core.ProcessComponents;
using JetBrains.Annotations;

namespace GeneCore.PopulationInitializers
{
    public abstract class
        AbstractRandomPopulationInitializer<TPopulation, TIndividual, TChromosome, TGene> : IPopulationInitializer<
            TPopulation, TIndividual, TGene> where TPopulation : IPopulation<TIndividual, TGene>, new()
        where TIndividual : IIndividual<TGene>, new()
        where TChromosome : IChromosome<TGene>, new()
    {
        private readonly TGene _maxValue;
        private readonly TGene _minValue;

        protected readonly Int32 ChromosomeLength;

        protected AbstractRandomPopulationInitializer(Int32 chromosomeLength, TGene minValue, TGene maxValue)
        {
            ChromosomeLength = chromosomeLength;
            _minValue = minValue;
            _maxValue = maxValue;
        }

        public abstract TPopulation InitializePopulation(Int32 populationSize);

        protected TPopulation InitializePopulation(Int32 populationSize
            , [NotNull] Func<Random, TGene, TGene, TGene> randomFunc)
        {
            #region Validation

            if (randomFunc == null) throw new ArgumentNullException(nameof(randomFunc));

            #endregion

            IList<TIndividual> population = new List<TIndividual>();

            for (Int32 i = 0; i < populationSize; i++)
            {
                TChromosome newChromosome = ChromosomeInitializers.GetRandomChromosome<TChromosome, TGene>(
                    ChromosomeLength, ChromosomeInitializers.CreateRandomProvider(_minValue, _maxValue, randomFunc));

                TIndividual newIndividual = new TIndividual();
                newIndividual.SetChromosome(newChromosome);

                population.Add(newIndividual);
            }

            TPopulation newPopulation = new TPopulation();
            newPopulation.SetPopulation(population);
            return newPopulation;
        }
    }

    public class
        Int32RandomPopulationInitializer<TPopulation, TIndividual, TChromosome> : AbstractRandomPopulationInitializer<
            TPopulation, TIndividual, TChromosome, Int32> where TPopulation : IPopulation<TIndividual, Int32>, new()
        where TIndividual : IIndividual<Int32>, new()
        where TChromosome : IChromosome<Int32>, new()
    {
        public Int32RandomPopulationInitializer(Int32 chromosomeLength, Int32 minValue, Int32 maxValue) : base(
            chromosomeLength, minValue, maxValue)
        {
            if (minValue > maxValue)
                throw new ArgumentException($"Minvalue {minValue} is larger than maxvalue {maxValue}");
        }

        public override TPopulation InitializePopulation(Int32 populationSize)
        {
            return InitializePopulation(populationSize, ChromosomeInitializers.GetRandomInt32);
        }
    }

    public class
        DoubleRandomPopulationInitializer<TPopulation, TIndividual, TChromosome> : AbstractRandomPopulationInitializer<
            TPopulation, TIndividual, TChromosome, Double> where TPopulation : IPopulation<TIndividual, Double>, new()
        where TIndividual : IIndividual<Double>, new()
        where TChromosome : IChromosome<Double>, new()
    {
        public DoubleRandomPopulationInitializer(Int32 chromosomeLength, Double minValue, Double maxValue) : base(
            chromosomeLength, minValue, maxValue)
        {
            if (minValue > maxValue)
                throw new ArgumentException($"Minvalue {minValue} is larger than maxvalue {maxValue}");
        }

        public override TPopulation InitializePopulation(Int32 populationSize)
        {
            return InitializePopulation(populationSize, ChromosomeInitializers.GetRandomDouble);
        }
    }

    public class
        BooleanRandomPopulationInitializer<TPopulation, TIndividual, TChromosome> : AbstractRandomPopulationInitializer<
            TPopulation, TIndividual, TChromosome, Boolean> where TPopulation : IPopulation<TIndividual, Boolean>, new()
        where TIndividual : IIndividual<Boolean>, new()
        where TChromosome : IChromosome<Boolean>, new()
    {
        public BooleanRandomPopulationInitializer(Int32 chromosomeLength) : base(chromosomeLength, false, true) { }

        public override TPopulation InitializePopulation(Int32 populationSize)
        {
            return InitializePopulation(populationSize, ChromosomeInitializers.GetRandomBoolean);
        }
    }
}