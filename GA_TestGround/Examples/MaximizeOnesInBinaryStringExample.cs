using System;
using GeneCore.Core.ProcessComponents;

namespace GA_TestGround.Examples
{
    public class BinaryIndividual : AbstractIndividual<Boolean> { }

    public class MaximizeOnesInBinaryStringExample : IExampleSelectionProcess
    {
        public void Run()
        {
/*            var fitnessFunction = new IFitnessFunction();
            
            var process = new GeneticSelectionProcess<Int32, BinaryIndividual>.Builder()
                .UseDefaultConsoleLogger()
                .SetParentSelector(new FittestParentsSelector(1))
                .SetSurvivorSelector()
                .SetMatingFunction(new CrossoverMatingFunction())
                .SetFitnessFunction()
                .Build();*/
        }
    }
}