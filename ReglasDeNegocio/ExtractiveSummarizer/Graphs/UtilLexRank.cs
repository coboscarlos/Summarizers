using System;
using BusinessRules.Utils;

namespace BusinessRules.ExtractiveSummarizer.Graphs
{
    public class UtilLexRank
    {
        public static double[] PowerMethod(double[][] matrix, double errorTolerance)
        {
            matrix = Matrix.Transpose(matrix);

            var p = new double[matrix.Length];
            var uniformProb = 1 / (double)p.Length;
            for (var i = 0; i < p.Length; i++)
                p[i] = uniformProb;

            double diff;

            var squareError = Math.Pow(errorTolerance, 2);
            do
            {
                var pNew = Matrix.Multiply(matrix, p);
                diff = Matrix.VectorNorm(Matrix.Substract(pNew, p));
                p = pNew;
            } while (diff > squareError);

            return (p);
        }
    }
}