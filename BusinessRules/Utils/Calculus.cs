using System;

namespace BusinessRules.Utils
{
    public static class Calculus
    {
        /// <summary>
        /// Calculate the cosine similarity between two points with n-dimensions
        /// </summary>
        /// <param name="p1">First n-dimensional array</param>
        /// <param name="p2">Second n-dimensional array</param>
        /// <returns></returns>
        public static double CosineSimilarity(double[] p1, double[] p2)
        {
            double numerator = 0;
            double norm1 = 0;
            double norm2 = 0;

            for (var i = 0; i < p1.GetUpperBound(0) + 1; i++)
            {
                numerator += p1[i] * p2[i];
                norm1 += p1[i] * p1[i];
                norm2 += p2[i] * p2[i];
            }
            norm1 = Math.Sqrt(norm1);
            norm2 = Math.Sqrt(norm2);
            var total = numerator/norm1;
            total = total/norm2;
            if (double.IsNaN(total)) total = 0;
            return total;
        }
    }
}