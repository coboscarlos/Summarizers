using BusinessRules.VectorSpaceModel;

namespace BusinessRules.Utils
{
    public enum TFIDFWeight 
    {
        Simple, // Simple
        Complete, // Complete
        Best,  // Best
        BM25, 
        Doc2Vec
    }

    public enum DocumentRepresentation
    {
        Centroid,  // Centroid: The document for the cosine similarity calculus is the centroid of the phrase collection
        Vector   // Vector: The document for the cosine similarity calculus is represented as a query expressed in the TF-IDF space
    }

    public enum SummaryType
    {
        Sentences, // Number of Sentences
        Words // Number of Words
    }

    public class SummaryParameters
    {
        public bool DetailedReport = false;
        public SummaryType MySummaryType = SummaryType.Words;
        public int MaximumLengthOfSummaryForRouge = 100;
        public TDMParameters MyTDMParameters;

        public override string ToString()
        {
            var result = MySummaryType.ToString().Substring(0,1) + "-" + 
                            MaximumLengthOfSummaryForRouge + "-" +
                            MyTDMParameters;
            return result;
        }
    }
}