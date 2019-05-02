using System.Collections.Generic;
using GeneCore.Core;

namespace GeneCore.MatingFunctions {
    public interface IMatingFunction {
        IIndividual Mate(IEnumerable<IIndividual> parents);
    }
}