namespace GeneCore.Core {
    public interface IProcessInformationComposer {
        IProcessInformation ComposeProcessInformation<T>(IProcessInformation previousInformation
            , IPopulation<T> population) where T : IIndividual;
    }
    
    public class ProcessInformationComposer : IProcessInformationComposer {
        public IProcessInformation ComposeProcessInformation<T>(IProcessInformation previousInformation
            , IPopulation<T> population) where T : IIndividual {
            
            // TODO
        }
    }
}