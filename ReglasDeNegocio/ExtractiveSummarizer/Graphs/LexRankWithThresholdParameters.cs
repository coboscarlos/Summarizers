using BusinessRules.Utils;

namespace BusinessRules.ExtractiveSummarizer.Graphs
{
    public class LexRankWithThresholdParameters: SummaryParameters
    {
        public double Threshold = 0.1;
        public double DampingFactor = 0.15;  // typical values between 0.1 y 0.2
        public double ErrorTolerance = 0.1;
        public bool SimilarityNormalized = false;

        public override string ToString()
        {
            var result = base.ToString() + "-LexRankWithThreshold-" +
                            Threshold + "-" +
                            DampingFactor + "-" +
                            ErrorTolerance + "-" +
                            SimilarityNormalized;
            return result;
        }
    }
}