using System.Collections.Generic;
using BusinessRules.Utils;

namespace BusinessRules.ExtractiveSummarizer
{
    public abstract class SummarizerAlgorithm
    {
        public string TextSummary;
        public List<KeyValuePair<string, int>> SummaryByPhrases;

        public abstract void Summarize(SummaryParameters mySummaryParameters, 
            string newsDirectory, string cacheFileName);
    }
}