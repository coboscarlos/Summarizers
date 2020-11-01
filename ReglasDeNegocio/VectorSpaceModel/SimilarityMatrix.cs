using System.IO;
using BusinessRules.Utils;

namespace BusinessRules.VectorSpaceModel
{
    /// <summary>
    /// When it saves, it only stores a lower triangular matrix with cosine
    /// similarity between each of the sentences:
    ///        s1    s2    s3
    /// s1     1     
    /// s2     2-1   1
    /// s3     3-1   3-2   1
    /// </summary>
    public class SimilarityMatrix
    {
        public double[][] CosineSimilarityBetweenPhrases;
        private double _maximumSimilarity;

        /// <summary>
        /// </summary>
        /// <param name="miTDM"></param>
        /// <param name="cachefileName"> </param>
        /// <param name="normalized"></param>
        /// <returns></returns>
        public SimilarityMatrix(TDM miTDM, string cachefileName, bool normalized = true)
        {
            var extension = normalized ? ".msimn" : ".msim";
            cachefileName = cachefileName + extension;
            if (cachefileName.Length != 0)
                if (File.Exists(cachefileName))
                {
                    ReadFromFile(cachefileName);
                    return;
                }

            var phrases = miTDM.TFIDFMatrix.GetUpperBound(0) + 1;
            var maximum = 0.0;
            var minimum = 1.0;

            CosineSimilarityBetweenPhrases = new double[phrases][];
            for (var i = 0; i < phrases; i++)
            {
                CosineSimilarityBetweenPhrases[i] = new double[phrases];
                for (var j = 0; j < i + 1; j++)
                {
                    if (i == j)
                    {
                        CosineSimilarityBetweenPhrases[i][j] = 1;
                    }
                    else
                    {
                        var actual = Calculus.CosineSimilarity(miTDM.TFIDFMatrix[i], miTDM.TFIDFMatrix[j]);
                        CosineSimilarityBetweenPhrases[i][j] = actual;
                        CosineSimilarityBetweenPhrases[j][i] = actual;
                        if (actual > maximum) maximum = actual;
                        if (actual < minimum) minimum = actual;
                    }
                }
            }

            if (normalized)
            {
                var range = maximum - minimum;
                for (var i = 0; i < phrases; i++)
                    for (var j = 0; j < i + 1; j++)
                    {
                        if (i != j)
                        {
                            var actual = (CosineSimilarityBetweenPhrases[i][j] - minimum)/range;
                            CosineSimilarityBetweenPhrases[i][j] = actual;
                            CosineSimilarityBetweenPhrases[j][i] = actual;
                        }
                    }
            }

            _maximumSimilarity = 0.0;
            for (var i = 0; i < phrases; i++)
                for (var j = 0; j < i + 1; j++)
                    if (i != j)
                        if (CosineSimilarityBetweenPhrases[i][j] > _maximumSimilarity)
                            _maximumSimilarity = CosineSimilarityBetweenPhrases[i][j];

            SaveToFile(cachefileName); 
        }

        public void ReadFromFile(string cachefileName)
        {
            var lines = File.ReadAllLines(cachefileName);
            var totalPhrases = int.Parse(lines[0]);
            var posLine = 1; //ignore line 1 because it is the same. The matrix is square

            CosineSimilarityBetweenPhrases = new double[totalPhrases][];
            for (var i = 0; i < totalPhrases; i++)
            {
                CosineSimilarityBetweenPhrases[i] = new double[totalPhrases];
                var dat = lines[posLine++].Split('\t');
                for (var j = 0; j < i + 1; j++)
                {
                    var data = double.Parse(dat[j]);
                    CosineSimilarityBetweenPhrases[i][j] = data;
                    if (i == j) continue;
                    CosineSimilarityBetweenPhrases[j][i] = data;
                }
            }

            _maximumSimilarity = double.Parse(lines[posLine]);
        }

        /// <summary>
        /// Save in a file the similarity matrix for all sentences in front of the document
        /// </summary>
        /// <param name="fileName"></param>
        public void SaveToFile(string fileName)
        {
            if (File.Exists(fileName))
                File.Delete(fileName);

            var result = Matrix.PrintDiagonalInferior(CosineSimilarityBetweenPhrases);
            File.AppendAllText(fileName, result);

            result = _maximumSimilarity.ToString("G17") + Characters.LineBreak;
            File.AppendAllText(fileName, result);
        }

        public double GetCosineSimilarityBetweenPhrases(int phraseI, int phraseJ)
        {
            return CosineSimilarityBetweenPhrases[phraseI][phraseJ];
        }
    }
}