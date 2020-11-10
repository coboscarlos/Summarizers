using System;
using BusinessRules.Utils;

namespace BusinessRules.ExtractiveSummarizer.Metaheuristics
{
    public abstract class BaseParameters: SummaryParameters
    {
        public Random RandomGenerator;

        public FitnessFunction TheFitnessFunction = FitnessFunction.MASDS;
        public FinalOrderOfSummary TheFinalOrderOfSummary = FinalOrderOfSummary.Position;
        public double Alfa = 0.55;
        public double Beta = 0.15;
        public double Gamma = 0.10;
        public double Delta = 0.10;
        public double Ro = 0.10;

        public int MaximumNumberOfFitnessFunctionEvaluations = 1600;

        public override string ToString()
        {
            var resultado = base.ToString() + "-" +
                            MaximumNumberOfFitnessFunctionEvaluations + "-" +
                            TheFitnessFunction + "-" +
                            Alfa.ToString("0.00") + "-" +
                            Beta.ToString("0.00") + "-" +
                            Gamma.ToString("0.00");
            
            if (TheFitnessFunction == FitnessFunction.MASDS)
                resultado += "-" + Delta.ToString("0.00") + "-" + Ro.ToString("0.00");

            return resultado;
        }
    }
}