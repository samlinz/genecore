using System;
using JetBrains.Annotations;

namespace GeneCore.Core.ProcessComponents
{
    /// <summary>
    ///     Represents an individual solution in the population. Individual has a single chromosome which represents
    ///     a solution to an arbitrary problem. Individuals are scored and ranked in the population and used to
    ///     generate new individuals or 'offsprings'.
    /// </summary>
    /// <typeparam name="TGene"></typeparam>
    public interface IIndividual<TGene>
    {
        String GetGuidStringShort();
        Guid GetGuid();

        [NotNull]
        IChromosome<TGene> GetChromosome();

        void SetChromosome(IChromosome<TGene> chromosome);
    }
}