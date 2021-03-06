﻿namespace BusinessRules.ExtractiveSummarizer.Metaheuristics.GBHS
{
    public class GBHSParameters : BaseParameters
    {
        public int HMS = 10;
        public double HMCR = 0.85;
        public double ParMin = 0.01;
        public double ParMax = 0.99;

        public double OptimizacionProbability = 0.4;
        public int MaxNumberOfOptimizacions = 5;

        public override string ToString()
        {
            var resultado = base.ToString() + "-GBHS-" +
                            HMS + "-" +
                            HMCR + "-" +
                            ParMin + "-" +
                            ParMax + "-" +
                            OptimizacionProbability + "-" +
                            MaxNumberOfOptimizacions;
            return resultado;
        }
    }
}