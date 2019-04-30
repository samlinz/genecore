using System;
using System.Collections.Generic;

namespace GeneCore.Core {
    public interface IProcessInformation {
    }

    class ProcessInformation : IProcessInformation {
        public UInt32 Generation;
        public UInt32 TotalFitness;
        public UInt32 FitnessDelta;
    }

    class ProcessInformationWithHistory : ProcessInformation {
        private readonly Int32 _historyEntries;

        // Process information history.
        public IList<ProcessInformationWithHistory> History { get; } = new List<ProcessInformationWithHistory>();

        public ProcessInformationWithHistory(Int32 historyEntries) {
            _historyEntries = historyEntries;
        }
    }
}