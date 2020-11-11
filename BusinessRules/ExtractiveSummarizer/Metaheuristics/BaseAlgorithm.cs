using System;
using System.Collections.Generic;
using BusinessRules.VectorSpaceModel;
using BusinessRules.Utils;

namespace BusinessRules.ExtractiveSummarizer.Metaheuristics
{
    public abstract class BaseAlgorithm : SummarizerAlgorithm
    {
        public BaseParameters MyParameters;

        public int CurrentFFEs;
        public int MaximumNumberOfFitnessFunctionEvaluations;
        public TDM MyTDM;
        public SimilarityMatrix MyExternalMDS;
        public int SolutionSize;

        public double[] PhrasePositionRanking;
        public List<int> OrderedLengths;

        protected List<PositionValue> SelectToCompleteSummary(List<BaseSolution> population,
            BaseSolution best)
        {
            var phrasesList = new List<PositionValue>();
            foreach (var t in population)
            {
                foreach (var phrase in t.SelectedPhrases)
                {
                    if (phrasesList.Exists(x => x.Position == phrase))
                    {
                        var currentPhrase = phrasesList.Find(x 
                            => x.Position == phrase);
                        currentPhrase.Value = currentPhrase.Value + 1;
                    }
                    else
                    {
                        var newPhrase = new PositionValue(phrase, 1);
                        phrasesList.Add(newPhrase);
                    }
                }
            }
            phrasesList.Sort((x, y) => -1 * 
                                       x.Value.CompareTo(y.Value));
            foreach (var phrase in best.SelectedPhrases)
            {
                if (phrasesList.Exists(x => x.Position == phrase))
                {
                    var currentPhrase = phrasesList.Find(x => 
                        x.Position == phrase);
                    phrasesList.Remove(currentPhrase);
                }
            }

            return phrasesList;
        }

        protected List<PositionValue> SelectPhrasesFromFinalSummary(
            List<int> selectedPhrases, List<PositionValue> additionalPhrase)
        {
            var phrasesList = new List<PositionValue>();
            if (additionalPhrase.Count > 0)
            {
                foreach (var phrase in additionalPhrase)
                    phrase.Value = 0;
            }

            switch (MyParameters.TheFinalOrderOfSummary)
            {
                case FinalOrderOfSummary.Position:
                    foreach (var frase in selectedPhrases){
                        phrasesList.Add(new PositionValue(frase, 
                            MyTDM.PhrasesList[frase].PositionInDocument));
                    }
                    phrasesList.Sort((x, y) => 
                        x.Value.CompareTo(y.Value));
                    phrasesList.AddRange(additionalPhrase);
                    break;

                case FinalOrderOfSummary.CRP:
                    foreach (var frase in selectedPhrases){
                        phrasesList.Add(new PositionValue(frase, 
                            MyTDM.PhrasesList[frase].SimilarityToDocument));
                    }
                    phrasesList.Sort((x, y) => -1 * 
                                               x.Value.CompareTo(y.Value));
                    phrasesList.AddRange(additionalPhrase);
                    break;

                case FinalOrderOfSummary.MASDS:
                    var size = selectedPhrases.Count;
                    var avgLength = 0.0;
                    for (var i = 0; i < size; i++)
                        avgLength += MyTDM.PhrasesList[selectedPhrases[i]].Length;
                    avgLength = avgLength / size;

                    var stdLength = 0.0;
                    for (var i = 0; i < size; i++)
                        stdLength += Math.Pow((MyTDM.PhrasesList[selectedPhrases[i]].Length 
                                               - avgLength), 2);
                    stdLength = Math.Sqrt(stdLength / size);

                    foreach (var thisPhrase in selectedPhrases)
                    {
                        var longitud = MyTDM.PhrasesList[thisPhrase].Length;
                        var complement = Math.Exp((-longitud - avgLength) / stdLength);

                        var cs = 0.0;
                        foreach (var otrafrase in selectedPhrases)
                            if (otrafrase != thisPhrase)
                                cs += MyExternalMDS.GetCosineSimilarityBetweenPhrases(
                                    thisPhrase, otrafrase);

                        var cov = 0.0;
                        foreach (var otrafrase in selectedPhrases)
                            if (otrafrase > thisPhrase)
                                cov += MyTDM.PhrasesList[thisPhrase].SimilarityToDocument +
                                        MyTDM.PhrasesList[otrafrase].SimilarityToDocument;

                        var f = Math.Sqrt(1.0 / (MyTDM.PhrasesList[thisPhrase].PositionInDocument));
                        f += MyTDM.PhrasesList[thisPhrase].SimilarityToTitle;
                        f += (1 - complement) / (1 + complement);
                        f += cs + cov;

                        phrasesList.Add(new PositionValue(thisPhrase, f));
                    }
                    phrasesList.Sort((x, y) => -1 * 
                                               x.Value.CompareTo(y.Value));
                    phrasesList.AddRange(additionalPhrase);
                    break;
            }
            return phrasesList;
        }

        public void CalculateRankingPhrasePosition()
        {
            var totalFrases = MyTDM.PhrasesList.Count;
            PhrasePositionRanking = new double[totalFrases];
            for (var i = 0; i < totalFrases; i++)
                PhrasePositionRanking[i] = (2.0 - (2.0 * (i - 1) / (totalFrases - 1))) / totalFrases;
        }

        public void SortLengths()
        {
            OrderedLengths = new List<int>();
            foreach (var t in MyTDM.PhrasesList)
                OrderedLengths.Add(t.Length);

            OrderedLengths.Sort((x,y) => -1 * x.CompareTo(y));
        }
    }
} 