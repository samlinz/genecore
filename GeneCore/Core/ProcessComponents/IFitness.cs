using System;
using JetBrains.Annotations;

namespace GeneCore.Core.ProcessComponents {
    public interface IFitness<T> : IComparable<IFitness<T>> {
        T GetFitness();
        Double GetFitnessDouble();
    }

    public abstract class AbstractFitness<T> : IFitness<T> {
        [NotNull]
        protected readonly T Fitness;

        protected AbstractFitness([NotNull] T fitness) {
            Fitness = fitness;
        }

        public T GetFitness() => Fitness;

        public abstract Double GetFitnessDouble();
        
        public abstract Int32 CompareTo(IFitness<T> other);
    }
    
    public class Int32Fitness : AbstractFitness<Int32> {
        public Int32Fitness(Int32 fitness) : base(fitness) {
        }

        public override Int32 CompareTo([NotNull] IFitness<Int32> other)
            => Fitness.CompareTo(other.GetFitness());

        public override Double GetFitnessDouble() => Convert.ToDouble(Fitness);
    }
    
    public class DoubleFitness : AbstractFitness<Double> {
        public DoubleFitness(Int32 fitness) : base(fitness) {
        }

        public override Int32 CompareTo([NotNull] IFitness<Double> other)
            => Fitness.CompareTo(other.GetFitness());
        
        public override Double GetFitnessDouble() => Fitness;
    }
}