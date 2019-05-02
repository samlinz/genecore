using GeneCore.Core;

namespace GeneCore.Checkpoints {
    /// <summary>
    /// Checkpoint is arbitrary method that runs every N generations.
    /// Can do anything really, for example serialize the search to disk.
    /// </summary>
    public interface ICheckpoint {
        void RunCheck<TFitness, TIndividual>(GeneticSelectionProcess<TFitness, TIndividual> process
            , IProcessInformation processInformation) where TIndividual : IIndividual;
    }

    // TODO
    /*public class SaveProcessToDisk : ICheckpoint {
    }*/
}