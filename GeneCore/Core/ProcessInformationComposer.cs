using System;
using System.Collections.Generic;
using System.Linq;
using GeneCore.Common;
using GeneCore.Core.ProcessComponents;
using JetBrains.Annotations;

namespace GeneCore.Core
{
    public interface IProcessInformationComposer<in TPopulation, TIndividual, TGene, TProcessInformation>
        where TPopulation : IPopulation<TIndividual, TGene>
        where TIndividual : IIndividual<TGene>
        where TProcessInformation : IProcessInformation
    {
        TProcessInformation ComposeProcessInformation(TProcessInformation previousInformation, TPopulation population);
    }

    public class
        ProcessInformationComposer<TPopulation, TIndividual, TGene> : IProcessInformationComposer<TPopulation,
            TIndividual, TGene, ProcessInformation> where TPopulation : IPopulation<TIndividual, TGene>
        where TIndividual : IIndividual<TGene>
    {
        // Use N fittest individuals to calculate the mean highest fitness.
        private const Int32 IndividualsToUseForLastNFitnessCalculation = 3;

        public ProcessInformation ComposeProcessInformation([CanBeNull] ProcessInformation previousInformation
            , [NotNull] TPopulation population)
        {
            #region Validation

            if (population == null) throw new ArgumentNullException(nameof(population));

            #endregion

            UInt64 previousGeneration = previousInformation?.Generation ?? 0;

            IList<IndividualWithFitness<TIndividual, TGene>> currentPopulation = population.GetPopulationByFitness();

            UInt64 newGeneration = previousGeneration + 1;
            Double fittestFitness = population.GetFittest().GetFitness();
            Double totalFitness = currentPopulation.Select(i => i.GetFitness()).Sum();

            Int32 populationSize = currentPopulation.Count;
            Int32 getFittestIndividualsCount = Math.Min(populationSize, IndividualsToUseForLastNFitnessCalculation);
            Double fittestNFitness = currentPopulation.ToList()
                .GetRange(populationSize - getFittestIndividualsCount, getFittestIndividualsCount)
                .Select(i => i.GetFitness()).ToList().Mean();

            // Create information object for current generation.
            ProcessInformation newProcessInformation = new ProcessInformation
            {
                Generation = newGeneration
                , TotalFitness = totalFitness
                , FittestFitness = fittestFitness
                , FittestNFitness = fittestNFitness
                , TotalFitnessDelta = totalFitness - (previousInformation?.TotalFitness ?? 0)
                , FittestFitnessDelta = fittestFitness - (previousInformation?.FittestFitness ?? 0)
                , FittestNFitnessDelta = fittestNFitness - (previousInformation?.FittestNFitness ?? 0)
            };

            return newProcessInformation;
        }
    }
}