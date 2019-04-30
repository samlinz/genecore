using System;
using GeneCore.Core;

namespace GeneCore.TerminationConditions {
    public interface ITerminationCondition {
        Boolean ShouldTerminate<T>(IPopulation<T> population, IProcessInformation process) where T : IIndividual;
    }
}