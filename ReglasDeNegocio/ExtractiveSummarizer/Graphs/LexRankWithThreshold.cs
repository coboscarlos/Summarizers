using System;
using System.Collections.Generic;
using System.Diagnostics;
using BusinessRules.VectorSpaceModel;
using BusinessRules.Utils;

namespace BusinessRules.ExtractiveSummarizer.Graphs
{
    public class LexRankWithThreshold:SummarizerAlgorithm
    {
        protected LexRankWithThresholdParameters Mis;

        public override void Summarize(SummaryParameters mySummaryParameters, string newsDirectory, string cacheFileName)
        {
            Mis = (LexRankWithThresholdParameters)mySummaryParameters;

            Debug.WriteLine("Starting execution of LexRankWithThreshold.");
            var startTime = DateTime.Now;

            var myTDM = new TDM(newsDirectory, Mis.MyTDMParameters, cacheFileName);
            var normalized = ((LexRankWithThresholdParameters) mySummaryParameters).SimilarityNormalized;
            var mySimilarityMatrix = new SimilarityMatrix(myTDM, cacheFileName, normalized);

            var totalPhrases = myTDM.PhrasesList.Count;
            var myCosineSimilarities = mySimilarityMatrix.CosineSimilarityBetweenPhrases;

            // Calculate the transition probabilities on the same similarity matrix,
            // i.e., the values are passed to 1 or 0 if they exceed the threshold and
            // at the end it is normalized dividing by the number of ones that remained
            // ... the row vector is normalized |v| = 1.
            // With this, one property of markov models are met.
            for (var i = 0; i < totalPhrases; i++)
            {
                var sum = 0.0d;
                for (var j = 0; j < totalPhrases; j++)
                {
                    if (myCosineSimilarities[i][j] > Mis.Threshold)
                    {
                        myCosineSimilarities[i][j] = 1;
                        sum++;
                    }
                    else
                    {
                        myCosineSimilarities[i][j] = 0;
                    }
                }
                for (var j = 0; j < totalPhrases; j++)
                    myCosineSimilarities[i][j] /= sum;
            }

            // Here the other two properties of the markov matrices are met.
            // The neighborhood graph is constructed.
            for (var i = 0; i < totalPhrases; i++)
                for (var j = 0; j < totalPhrases; j++)
                {
                    myCosineSimilarities[i][j] = (Mis.DampingFactor / totalPhrases) +
                                  (1 - Mis.DampingFactor) * myCosineSimilarities[i][j];
                }

            var weights = UtilLexRank.PowerMethod(myCosineSimilarities, Mis.ErrorTolerance);

            var phrasesList = new List<KeyValuePair<int, double>>(); // Save candidate phrases with their weight (relevance)
            for (var i = 0; i < totalPhrases; i++)
                phrasesList.Add(new KeyValuePair<int, double>(i, weights[i]));

            //phrasesList.Sort((x,y) => -1*x.Value.CompareTo(y.Value)); // The phrases are ordered by their weight
            phrasesList.Sort(delegate(KeyValuePair<int, double> x, KeyValuePair<int, double> y)
            {
                if (Math.Abs(x.Value - y.Value) < 1e-07)
                    return myTDM.PhrasesList[x.Key].PositionInDocument.CompareTo(myTDM.PhrasesList[y.Key].PositionInDocument);
                return -1 * x.Value.CompareTo(y.Value);
            });

            TextSummary = Util.SummarizeByCompressionRatio(myTDM, phrasesList, mySummaryParameters.MySummaryType, 
                Mis.MaximumLengthOfSummaryForRouge, out SummaryByPhrases);

            var fin = DateTime.Now - startTime;
            Debug.WriteLine("Minutes of LexRankWithThreshold " + fin.TotalMinutes);
        }
    }
}