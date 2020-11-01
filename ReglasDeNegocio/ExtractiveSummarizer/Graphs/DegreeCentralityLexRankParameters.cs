using BusinessRules.Utils;

namespace BusinessRules.ExtractiveSummarizer.Graphs
{
    public class DegreeCentralityLexRankParameters : SummaryParameters
    {
        public double DegreeCentrality = 0.1;
        public bool SimilarityNormalized = false;

        public override string ToString()
        {
            var result = base.ToString() + "-DegreeCentralityLexRank-" +
                         DegreeCentrality + "-" +
                         SimilarityNormalized;
            return result;
        }
    }
}