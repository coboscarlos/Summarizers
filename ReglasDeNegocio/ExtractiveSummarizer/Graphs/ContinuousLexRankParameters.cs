using BusinessRules.Utils;

namespace BusinessRules.ExtractiveSummarizer.Graphs
{
    public class ContinuousLexRankParameters : SummaryParameters
    {
        public double DampingFactor = 0.15;  // typical values between 0.1 y 0.2
        public double ErrorTolerance = 0.1;
        public bool SimilarityNormalized = false;

        public override string ToString()
        {
            var result = base.ToString() + "-ContinuousLexRank-" +
                         DampingFactor + "-" +
                         ErrorTolerance + "-" +
                         SimilarityNormalized;
            return result;
        }
    }
}