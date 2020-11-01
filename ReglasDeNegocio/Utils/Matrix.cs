using System;
using System.Linq;
using System.Text;

namespace BusinessRules.Utils
{
    public static class Matrix
    {
        public static double[][] Transpose(double[][] mat1)
        {
            var rows = mat1.Length;
            var columns = mat1[0].Length;
            var result = new double[columns][];

            for (var i = 0; i < columns; i++)
                result[i] = new double[rows];

            for (var i = 0; i < rows; i++)
            {
                for (var j = 0; j < columns; j++)
                {
                    result[j][i] = mat1[i][j];
                }
            }

            return result;
        }

        public static double[] Multiply(double[][] mat1, double[] mat2)
        {
            var result = new double[mat2.Length];

            for (var i = 0; i < result.Length; i++)
                result[i] = 0;

            for (var i = 0; i < mat1.Length; i++)
            {
                for (var j = 0; j < mat1.Length; j++)
                {
                    result[i] += mat1[i][j] * mat2[j];
                }
            }

            return (result);
        }

        public static double VectorNorm(double[] vector)
        {
            var norm = vector.Sum(t => Math.Abs(t));
            return norm;
        }

        public static double[] Substract(double[] mat1, double[] mat2)
        {
            var result = new double[mat1.Length];
            for (var i = 0; i < mat1.Length; i++)
                result[i] = Math.Pow(mat1[i] - mat2[i], 2);

            return (result);
        }

        public static string Print(double[][] matrix)
        {
            var result = new StringBuilder();

            var rows = matrix.GetUpperBound(0) + 1;
            result.Append(rows + Characters.LineBreak);

            var columns = matrix[0].GetUpperBound(0) + 1;
            result.Append(columns + Characters.LineBreak);

            for (var i = 0; i < rows; i++)
            {
                for (var j = 0; j < columns; j++)
                {
                    result.Append(matrix[i][j].ToString("G17") + Characters.Tab);
                }

                result.Append(Characters.LineBreak);
            }

            return result.ToString();
        }

        public static string PrintDiagonalInferior(double[][] matrix)
        {
            var result = new StringBuilder();

            var rows = matrix.GetUpperBound(0) + 1;
            result.Append(rows + Characters.LineBreak);

            for (var i = 0; i < rows; i++)
            {
                for (var j = 0; j < i+1; j++)
                {
                    result.Append(matrix[i][j].ToString("G17") + Characters.Tab);
                }

                result.Append(Characters.LineBreak);
            }

            return result.ToString();
        }
    }
}