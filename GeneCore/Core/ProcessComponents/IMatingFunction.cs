using System.Collections.Generic;

namespace GeneCore.Core.ProcessComponents
{
    /// <summary>
    ///     Function which receives two or more individuals referred to as 'parents' picked by
    ///     <see cref="IParentSelector{TPopulation,TIndividual,TGene}" /> function and creates a single
    ///     new individual with chromosome based on the parents.
    /// </summary>
    /// <typeparam name="TIndividual"></typeparam>
    /// <typeparam name="TChromosome"></typeparam>
    /// <typeparam name="TGene"></typeparam>
    public interface IMatingFunction<TIndividual, TChromosome, TGene> where TIndividual : IIndividual<TGene>
        where TChromosome : IChromosome<TGene>
    {
        TIndividual Mate(IList<TIndividual> parents);
    }
}