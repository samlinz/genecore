using System.Collections.Generic;

namespace GeneCore.Core.ProcessComponents
{
    /// <summary>
    ///     Function which selects single or more groups (usually of 2) individuals that act as parents
    ///     for the next generation of offpsring individuals.
    /// </summary>
    /// <typeparam name="TPopulation"></typeparam>
    /// <typeparam name="TIndividual"></typeparam>
    /// <typeparam name="TGene"></typeparam>
    public interface IParentSelector<in TPopulation, out TIndividual, TGene>
        where TPopulation : IPopulation<TIndividual, TGene> where TIndividual : IIndividual<TGene>
    {
        IEnumerable<IEnumerable<TIndividual>> GetParents(TPopulation population);
    }
}