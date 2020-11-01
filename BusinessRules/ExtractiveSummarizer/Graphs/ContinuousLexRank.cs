using System;
using System.Collections.Generic;
using System.Diagnostics;
using BusinessRules.VectorSpaceModel;
using BusinessRules.Utils;

namespace BusinessRules.ExtractiveSummarizer.Graphs
{
    public class ContinuousLexRank : SummarizerAlgorithm
    {
        protected ContinuousLexRankParameters Mis;

        public override void Summarize(SummaryParameters mySummaryParameters, string newsDirectory, string cacheFileName)
        {
            Mis = (ContinuousLexRankParameters)mySummaryParameters;

            Debug.WriteLine("Starting execution of ContinuousLexRank.");
            var startTime = DateTime.Now;

            var myTDM = new TDM(newsDirectory, Mis.MyTDMParameters, cacheFileName);
            var normalized = ((ContinuousLexRankParameters)mySummaryParameters).SimilarityNormalized;
            var mySimilarityMatrix = new SimilarityMatrix(myTDM, cacheFileName, normalized);

            var totalPhrases = myTDM.PhrasesList.Count;
            var myCosineSimilarities = mySimilarityMatrix.CosineSimilarityBetweenPhrases;

            // Calculate the transition probabilities over the same similarity matrix
            // the row vector is normalized |v| = 1
            for (var i = 0; i < totalPhrases; i++)
            {
                var sum = 0.0d;
                for (var j = 0; j < totalPhrases; j++)
                    sum += myCosineSimilarities[i][j];

                // The row must add 1 to be a matrix representing a markov chain where the
                // transition probabilities of state i to state j are stored.
                // It must add 1 because all the probabilities of change when accumulating must add 1.
                for (var j = 0; j < totalPhrases; j++)
                    myCosineSimilarities[i][j] /= sum;
            }

            // The neighborhood graph is constructed - Matrix stochastic, irreducible and aperiodic.
            // It is irreducible if a state (node) can be reached from any other state (node).
            // It is aperiodic if the period = 1, i.e. mcd {n: P(n) x,x > 0}
            for (var i = 0; i < totalPhrases; i++)
                for (var j = 0; j < totalPhrases; j++)
                {
                    var valor = myCosineSimilarities[i][j];
                    valor = (Mis.DampingFactor / totalPhrases) +
                        (1 - Mis.DampingFactor) * valor;
                    myCosineSimilarities[i][j] = valor;
                }

            // Based on the Perron-Frobenius theorem, an irreducible and aperiodic Markov chain
            // always converges to a single stationary distribution.
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

            var endTime = DateTime.Now - startTime;
            Debug.WriteLine("Minutes of ContinuousLexRank: " + endTime.TotalMinutes);
        }
    }
}