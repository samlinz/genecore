using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace GeneCore.Core.ProcessComponents
{
    /// <summary>
    ///     Population is the entire groups of individuals, or solutions that currently exists.
    ///     Implementations provide methods to rank the individuals by their fitness score and access
    ///     them in unsorted or sorted manner.
    /// </summary>
    /// <typeparam name="TIndividual"></typeparam>
    /// <typeparam name="TGene"></typeparam>
    public interface IPopulation<TIndividual, TGene> where TIndividual : IIndividual<TGene>
    {
        void OrderByFitness(IFitnessFunction<TIndividual, TGene> fitnessFunction);

        [NotNull]
        IList<IndividualWithFitness<TIndividual, TGene>> GetPopulationByFitness();

        [NotNull]
        IList<IndividualWithFitness<TIndividual, TGene>> GetNFittest(Int32 n);

        [NotNull]
        IndividualWithFitness<TIndividual, TGene> GetFittest();

        void ReplaceIndividual(TIndividual replaced, TIndividual replacing);
        void SetPopulation([NotNull] IEnumerable<TIndividual> population);

        [NotNull]
        IEnumerable<TIndividual> GetIndividuals();

        TIndividual GetIndividual(Guid uuid);
        IndividualWithFitness<TIndividual, TGene> GetIndividualWithFitness(Guid uuid);
    }
}