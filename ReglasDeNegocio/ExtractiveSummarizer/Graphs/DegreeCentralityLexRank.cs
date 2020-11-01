using System;
using System.Collections.Generic;
using System.Diagnostics;
using BusinessRules.VectorSpaceModel;
using BusinessRules.Utils;

namespace BusinessRules.ExtractiveSummarizer.Graphs
{
    public class DegreeCentralityLexRank: SummarizerAlgorithm
    {
        protected  DegreeCentralityLexRankParameters Mis;

        public override void Summarize(SummaryParameters mySummaryParameters, string newsDirectory, string cacheFileName)
        {
            Mis = (DegreeCentralityLexRankParameters)mySummaryParameters;

            Debug.WriteLine("Starting execution of DegreeCentralityLexRank.");
            var startTime = DateTime.Now;

            var myTDM = new TDM(newsDirectory, Mis.MyTDMParameters, cacheFileName);
            var normalized = ((DegreeCentralityLexRankParameters)mySummaryParameters).SimilarityNormalized;
            var mySimilarityMatrix = new SimilarityMatrix(myTDM, cacheFileName, normalized);

            var totalPhrases = myTDM.PhrasesList.Count;
            var myCosineSimilarities = mySimilarityMatrix.CosineSimilarityBetweenPhrases;

            var weights = new double[totalPhrases];
            for (var i = 0; i < totalPhrases; i++)
            {
                var sum = 0.0d;
                for (var j = 0; j < totalPhrases; j++)
                {
                    if (myCosineSimilarities[i][j] > Mis.DegreeCentrality)
                        sum++;
                }
                weights[i] = sum;
            }

            var phrasesList = new List<KeyValuePair<int, double>>(); // Save candidate phrases with their weight (relevance)
            for (var i = 0; i < totalPhrases; i++)
                phrasesList.Add(new KeyValuePair<int, double>(i, weights[i]));

            //phrasesList.Sort((x,y) => -1 * x.Value.CompareTo(y.Value)); // The phrases are ordered by their weight
            phrasesList.Sort(delegate(KeyValuePair<int, double> x, KeyValuePair<int, double> y)
            {
                if (Math.Abs(x.Value - y.Value) < 1e-07)
                    return myTDM.PhrasesList[x.Key].PositionInDocument.CompareTo(myTDM.PhrasesList[y.Key].PositionInDocument);
                return -1 * x.Value.CompareTo(y.Value);
            });

            TextSummary = Util.SummarizeByCompressionRatio(myTDM, phrasesList, mySummaryParameters.MySummaryType, 
                Mis.MaximumLengthOfSummaryForRouge, out SummaryByPhrases);

            var fin = DateTime.Now - startTime;
            Debug.WriteLine("Minutes of DegreeCentralityLexRank: " + fin.TotalMinutes);
        }
    }
}