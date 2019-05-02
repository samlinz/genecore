using System;
using System.Collections.Generic;

namespace GeneCore.Core {
    public interface IProcessInformation {
        UInt64 GetGeneration();
        Double GetTotalFitness();
        UInt32 GetFitnessDelta();
    }

    public class ProcessInformation : IProcessInformation {
        private readonly UInt32 _generation;
        private readonly Double _totalFitness;
        private readonly UInt32 _fitnessDelta;

        public ProcessInformation(UInt32 generation, UInt32 totalFitness, UInt32 fitnessDelta) {
            _generation = generation;
            _totalFitness = totalFitness;
            _fitnessDelta = fitnessDelta;
        }

        public UInt64 GetGeneration() => _generation;

        public Double GetTotalFitness() => _totalFitness;

        public UInt32 GetFitnessDelta() => _fitnessDelta;
    }

/*    class ProcessInformationWithHistory : ProcessInformation {
        private readonly Int32 _historyEntries;

        // Process information history.
        public IList<ProcessInformationWithHistory> History { get; } = new List<ProcessInformationWithHistory>();

        public ProcessInformationWithHistory(Int32 historyEntries) {
            _historyEntries = historyEntries;
        }
    }*/
}