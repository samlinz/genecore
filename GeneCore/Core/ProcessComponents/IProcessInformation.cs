using System;

namespace GeneCore.Core.ProcessComponents
{
    /// <summary>
    ///     Object encoding information about the running optimization process.
    /// </summary>
    public interface IProcessInformation
    {
        UInt64 GetGeneration();
        Double GetTotalFitness();
    }

    /// <summary>
    ///     Implementation encoding the most basic information about a running process.
    /// </summary>
    public class ProcessInformation : IProcessInformation
    {
        public UInt64 Generation { get; set; }

        public Double TotalFitness { get; set; }
        public Double FittestFitness { get; set; }
        public Double FittestNFitness { get; set; }


        public Double TotalFitnessDelta { get; set; }
        public Double FittestFitnessDelta { get; set; }
        public Double FittestNFitnessDelta { get; set; }

        public UInt64 GetGeneration()
        {
            return Generation;
        }

        public Double GetTotalFitness()
        {
            return TotalFitness;
        }
    }
}