using System;
using GeneCore.Core.ProcessComponents;

namespace GeneCore.TerminationConditions
{
    public interface ITerminationCondition<in TPopulation, TIndividual, TGene, in TProcessInformation>
        where TPopulation : IPopulation<TIndividual, TGene>
        where TIndividual : IIndividual<TGene>
        where TProcessInformation : IProcessInformation
    {
        Boolean ShouldTerminate(TPopulation population, TProcessInformation process);
    }
}