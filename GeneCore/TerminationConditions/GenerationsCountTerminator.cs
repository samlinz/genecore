using System;
using GeneCore.Core.ProcessComponents;
using JetBrains.Annotations;

namespace GeneCore.TerminationConditions
{
    public class
        GenerationsCountTerminator<TPopulation, TIndividual, TGene> : ITerminationCondition<TPopulation, TIndividual,
            TGene, ProcessInformation> where TPopulation : IPopulation<TIndividual, TGene>
        where TIndividual : IIndividual<TGene>
    {
        private readonly UInt64 _generationsMax;

        public GenerationsCountTerminator(UInt64 generationsMax)
        {
            _generationsMax = generationsMax;
        }

        public Boolean ShouldTerminate([NotNull] TPopulation population, [NotNull] ProcessInformation process)
        {
            #region Validation

            if (population == null) throw new ArgumentNullException(nameof(population));
            if (process == null) throw new ArgumentNullException(nameof(process));

            #endregion

            return process.Generation >= _generationsMax;
        }
    }
}