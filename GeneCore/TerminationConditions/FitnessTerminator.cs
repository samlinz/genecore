using System;
using GeneCore.Core.ProcessComponents;
using JetBrains.Annotations;

namespace GeneCore.TerminationConditions
{
    public class
        FitnessTerminator<TPopulation, TIndividual, TGene> : ITerminationCondition<TPopulation, TIndividual, TGene,
            ProcessInformation> where TPopulation : IPopulation<TIndividual, TGene>
        where TIndividual : IIndividual<TGene>
    {
        private readonly Double _fitnessMax;

        public FitnessTerminator(Double fitnessMax)
        {
            _fitnessMax = fitnessMax;
        }

        public Boolean ShouldTerminate([NotNull] TPopulation population, [NotNull] ProcessInformation process)
        {
            #region Validation

            if (population == null) throw new ArgumentNullException(nameof(population));
            if (process == null) throw new ArgumentNullException(nameof(process));

            #endregion

            return population.GetFittest().GetFitness() >= _fitnessMax;
        }
    }
}