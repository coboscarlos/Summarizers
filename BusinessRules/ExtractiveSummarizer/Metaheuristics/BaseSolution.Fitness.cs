using System;
using System.Collections.Generic;
using System.Linq;

namespace BusinessRules.ExtractiveSummarizer.Metaheuristics
{
    public abstract partial class BaseSolution
    {
        /// <summary>
        /// Calculate the fitness of the solution. You MUST make sure that the Summary Length was
        /// calculated before.
        /// </summary>
        public virtual void CalculateFitness()
        {
            MyContainer.CurrentFFEs++;

            if (SummaryLength > MyContainer.MyParameters.MaximumLengthOfSummaryForRouge)
            {
                Fitness = 0;
                return;
            }

            var alfa = MyContainer.MyParameters.Alfa;
            var beta = MyContainer.MyParameters.Beta;
            var gamma = MyContainer.MyParameters.Gamma;
            var delta = MyContainer.MyParameters.Delta;
            var ro = MyContainer.MyParameters.Ro;

            switch (MyContainer.MyParameters.TheFitnessFunction)
            {
                case FitnessFunction.MCMR: CalculateMCMR();
                    break;
                case FitnessFunction.CRP:
                    CRRSCNPos(alfa, beta, gamma);
                    break;
                case FitnessFunction.MASDS:
                    FitnessFunctionsWithFiveFactors(alfa, beta, gamma, delta, ro);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// Fitness function based on CalculateMCMR (Maximum Coverage Minimum Redundancy)
        /// </summary>
        public void CalculateMCMR()
        {
            var fs = SelectedPhrases;
            Fitness = 0.0;
            for (var i = 0; i < fs.Count - 1; i++)
            for (var j = i + 1; j < fs.Count; j++)
            {
                var posI = fs[i];
                var posJ = fs[j];
                Fitness += MyContainer.MyTDM.PhrasesList[posI].SimilarityToDocument +
                           MyContainer.MyTDM.PhrasesList[posJ].SimilarityToDocument -
                           MyContainer.MyExternalMDS.CosineSimilarityBetweenPhrases[posI][posJ];
            }
        }

        public void CRRSCNPos(double alfa, double beta, double gamma)
        {
            var fs = SelectedPhrases;
            // Calculate coverage as a function of the similarity of the abstract text to the full
            // document multiplied by the number of phrases selected in the abstract
            var coverage = MyContainer.MyTDM.CosineSimilarityFromSummaryTextToDocument(fs, MyContainer.MyParameters.MaximumLengthOfSummaryForRouge);

            // calculate the average similarity of the summary sentences
            // if the redundancy is high it is bad because all the sentences of the summary resemble each other
            // if the redundancy is low it is good because it means that phrases are taken from different clusters
            var redundancy = 0.0;
            var tot = 0;
            for (var i = 0; i < fs.Count - 1; i++)
                for (var j = i + 1; j < fs.Count; j++)
                {
                    redundancy += MyContainer.MyExternalMDS.CosineSimilarityBetweenPhrases[fs[i]][fs[j]];
                    tot++;
                }
            if (tot == 0)
                redundancy = 0;
            else
                redundancy = redundancy / tot;

            // calculation of the average position of the sentences
            var documentPosition = fs.Sum(frase => MyContainer.PhrasePositionRanking[frase]);
            documentPosition /= fs.Count;
            documentPosition /= MyContainer.PhrasePositionRanking[0]; // To normalize in reason to the greater

            // calculate the total fitness
            Fitness = (alfa * coverage) - (beta * redundancy) + (gamma * documentPosition);
        }

        public void FitnessFunctionsWithFiveFactors(double alfa, double beta, double gama, double delta, double ro)
        {
            if (alfa > 0) PositionFactor = CalculatePositionFactor(SelectedPhrases);
            if (beta > 0) TitleSimilarityFactor = CalculateTitleSimilarityFactor(SelectedPhrases);
            if (gama > 0) LengthFactor = CalculateLengthFactor(SelectedPhrases);
            if (delta > 0) CohesionFactor = CalculateCohesionFactor(SelectedPhrases);
            if (ro > 0) CoverageFactor = CalculateCoverageFactor(SelectedPhrases);

            Fitness = (alfa * PositionFactor) + 
                      (beta * TitleSimilarityFactor) + 
                      (gama * LengthFactor) +
                      (delta * CohesionFactor) +
                      (ro * CoverageFactor);
        }

        /// <summary>
        /// Calculate the position factor of the summary. _phrases closest to the beginning of the document
        /// are supposed to be more important (applies great for news).
        /// </summary>
        /// <param name="selectedPhrases"></param>
        /// <returns></returns>
        public double CalculatePositionFactor(List<int> selectedPhrases)
        {
            var size = selectedPhrases.Count;
            var documentPosition = selectedPhrases.Sum(frase => MyContainer.PhrasePositionRanking[frase]);
            documentPosition /= selectedPhrases.Count;

            var maxpos = 0.0;
            var minpos = 0.0;
            var ultimaFrase = MyContainer.MyTDM.PhrasesList.Count - 1;
            for (var i = 0; i < size; i++)
            {
                maxpos += MyContainer.PhrasePositionRanking[i];
                minpos += MyContainer.PhrasePositionRanking[ultimaFrase - i];
            }
            maxpos = maxpos / size;
            minpos = minpos / size;

            return (documentPosition - minpos) / (maxpos - minpos); // Normalize based on range
        }

        /// <summary>
        /// Calculate the relationship with the subject, which implies the similarity with
        /// the title of the document
        /// </summary>
        /// <returns>A value of type double that indicates the weight of the Topic Relation
        /// Factor for the summary</returns>
        public double CalculateTitleSimilarityFactor(List<int> selectedPhrases)
        {
            var size = selectedPhrases.Count;

            var rt = 0.0;
            for (var i = 0; i < size; i++)
                rt += MyContainer.MyTDM.PhrasesList[selectedPhrases[i]].SimilarityToTitle;
            rt = rt / size;

            if (rt <= 0.0) return 1.0;

            var maxtit = 0.0;
            var mintit = 0.0;
            var lastPhrase = MyContainer.MyTDM.PhrasesList.Count - 1;
            for (var i = 0; i < size; i++)
            {
                maxtit += MyContainer.MyTDM.SimilaritiesOrderedToTheTitle[i];
                mintit += MyContainer.MyTDM.SimilaritiesOrderedToTheTitle[lastPhrase - i];
            }
            maxtit = maxtit / size;
            mintit = mintit/size;

            var factor = (rt - mintit)/ (maxtit - mintit);
            if (double.IsNaN(factor))
                factor = 1.0;
            return factor;
        }

        /// <summary>
        /// Calculate the Lenght Factor of a summary.
        /// </summary>
        /// <param name="selectedPhrases"></param>
        /// <returns></returns>
        public double CalculateLengthFactor(List<int> selectedPhrases)
        {
            if (selectedPhrases.Count == 0) return 0.0;

            var size = selectedPhrases.Count;
            var averageLength = 0.0;
            for (var i = 0; i < size; i++)
                averageLength += MyContainer.MyTDM.PhrasesList[selectedPhrases[i]].Length;
            averageLength = averageLength / size;

            var maxLon = 0.0;
            var minLon = 0.0;
            var lastPhrase = MyContainer.MyTDM.PhrasesList.Count - 1;
            for (var i = 0; i < size; i++)
            {
                maxLon += MyContainer.OrderedLengths[i];
                minLon += MyContainer.OrderedLengths[lastPhrase - i];
            }
            maxLon = maxLon / size;
            minLon = minLon / size;
            
            return (averageLength - minLon) / (maxLon - minLon);
        }

        /// <summary>
        /// Calculate the Cohesion Factor of a summary. Indicates the degree to which
        /// the summary sentences tell about the same information
        /// </summary>
        /// <param name="selectedPhrases">_phrases List that ...</param>
        /// <returns></returns>
        public double CalculateCohesionFactor(List<int> selectedPhrases)
        {
            var myMDS = MyContainer.MyExternalMDS;
            var size = selectedPhrases.Count;
            var cohesion = 0.0;
            var total = 0;
            for (var i = 0; i < size; i++)
                for (var j = i + 1; j < size; j++)
                {
                    cohesion += myMDS.GetCosineSimilarityBetweenPhrases(selectedPhrases[i], selectedPhrases[j]);
                    total++;
                }
            return cohesion/total;
        }

        /// <summary>
        /// Calculate the coverage factor
        /// </summary>
        /// <returns>A value of type double that indicates the Coverage Factor of the summary</returns>
        public double CalculateCoverageFactor(List<int> selectedPhrases)
        {
            var coverage = MyContainer.MyTDM.CosineSimilarityFromSummaryTextToDocument(selectedPhrases, 
                MyContainer.MyParameters.MaximumLengthOfSummaryForRouge);
            return coverage;
        }
    }
}