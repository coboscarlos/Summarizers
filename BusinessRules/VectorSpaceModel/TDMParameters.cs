using BusinessRules.Utils;

namespace BusinessRules.VectorSpaceModel
{
    public class TDMParameters
    {
        public double MinimumThresholdForTheAcceptanceOfThePhrase = 0.0; // If a phrase does not exceed this threshold it is not selected in the summary
        public int MinimumFrequencyThresholdOfTermsForPhrase = 0; // If a phrase does not exceed this threshold it is removed from the TDM
        public DocumentRepresentation TheDocumentRepresentation = DocumentRepresentation.Centroid;
        public TFIDFWeight  TheTFIDFWeight  = TFIDFWeight.BM25;

        public override string ToString()
        {
            var result = MinimumThresholdForTheAcceptanceOfThePhrase + "-" +
                            MinimumFrequencyThresholdOfTermsForPhrase + "-" +
                            TheDocumentRepresentation.ToString().Substring(0,1) + "-" +
                            TheTFIDFWeight.ToString().Substring(0,1);
            return result;
        }
    }
}