using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using BusinessRules.Utils;

namespace BusinessRules.ExtractiveSummarizer.Metaheuristics
{
    public abstract class BaseSolucion : IEquatable<BaseSolucion>, IComparable<BaseSolucion>
    {
        public AlgorithmBase MyContainer;
        public double Fitness;
        public double PositionFactor;
        public double TitleSimilarityFactor;
        public double LengthFactor;
        public double CohesionFactor;
        public double CoverageFactor;

        public int SummaryLength;
        public List<int> ActivePhrases;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="myContainer"></param>
        protected BaseSolucion(AlgorithmBase myContainer)
        {
            MyContainer = myContainer;
            Fitness = 0;
            ActivePhrases = new List<int>();
        }

        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="copia"></param>
        protected BaseSolucion(BaseSolucion copia)
        {
            MyContainer = copia.MyContainer;
            Fitness = copia.Fitness;
            PositionFactor = copia.PositionFactor;
            TitleSimilarityFactor = copia.TitleSimilarityFactor;
            LengthFactor = copia.LengthFactor;
            CohesionFactor = copia.CohesionFactor;
            CoverageFactor = copia.CoverageFactor;
            SummaryLength = copia.SummaryLength;
            ActivePhrases = new List<int>();
            foreach (var fra in copia.ActivePhrases)
                ActivePhrases.Add(fra);
        }

        /// <summary>
        /// Copy current solution into another
        /// </summary>
        /// <param name="destino"></param>
        public void CopyTo(BaseSolucion destino)
        {
            destino.Fitness = Fitness;
            destino.PositionFactor = PositionFactor;
            destino.TitleSimilarityFactor = TitleSimilarityFactor;
            destino.LengthFactor = LengthFactor;
            destino.CohesionFactor = CohesionFactor;
            destino.CoverageFactor = CoverageFactor;
            destino.SummaryLength = SummaryLength;
            destino.ActivePhrases = new List<int>();
            foreach (var fra in ActivePhrases)
                destino.ActivePhrases.Add(fra);
        }

        /// <inheritdoc />
        /// <summary>
        /// Compare the genotype completely, if it is exactly the same, return true, otherwise return false
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(BaseSolucion other)
        {
            if (other is null) return false;
            var este = ActivePhrases;
            var otro = other.ActivePhrases;
            if (otro.Count != este.Count) return false;

            foreach (var posFrase in este)
                if (!otro.Contains(posFrase))
                    return false;
            return true;
        }

        /// <inheritdoc />
        /// <summary>
        /// Order from highest to lowest for the value of Fitness. It is used to
        /// have the highest values in the first places.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareTo(BaseSolucion other)
        {
            var minLon = MyContainer.MyParameters.MaximumLengthOfSummaryForRouge;
            var maxLon = MyContainer.MyParameters.MaximumSummaryLengthToEvolve;
            var soyValido = (SummaryLength >= minLon && SummaryLength <= maxLon);
            var otroEsValido = (other.SummaryLength >= minLon && other.SummaryLength <= maxLon);
            if (soyValido == otroEsValido) return -1 * Fitness.CompareTo(other.Fitness);
            if (soyValido) return -1;
            return 1;
        }

        public override string ToString()
        {
            var selectedPhrases = new StringBuilder("");
            selectedPhrases.Append("F:" + Fitness.ToString("#0.000") + " [");
            foreach (var pos in ActivePhrases)
                selectedPhrases.Append(pos + ",");
            selectedPhrases.Remove(selectedPhrases.Length - 1, 1);
            selectedPhrases.Append("] ");
            selectedPhrases.Append("L:" + SummaryLength.ToString("###") + "");
            return selectedPhrases.ToString();
        }

        /// <summary>
        /// Random initialization of a the agent, taking into account that the phrases are different from each other
        /// </summary>
        public void InicializarAleatorio()
        {
            SummaryLength = 0;
            AgregarFrasesValidas();
            ActivePhrases.Sort((x,y) => x.CompareTo(y));
            CalculateFitness();
        }

        public void AgregarFrasesValidas()
        {
            while (SummaryLength < MyContainer.MyParameters.MaximumLengthOfSummaryForRouge)
            {
                var falta = MyContainer.MyParameters.MaximumLengthOfSummaryForRouge - SummaryLength;
                var consulta = MyContainer.ViablePhrases.Where(
                        x => MyContainer.MyTDM.PhrasesList[x.Position].Length < falta);
                consulta = consulta.Where(x => !ActivePhrases.Contains(x.Position));
                var posicionValors = consulta as PositionValue[] ?? consulta.ToArray();
                if (!posicionValors.Any()) break;

                int pos;
                int longitudFrase;
                do
                {
                    pos = MyContainer.MyParameters.NumeroAleatorio.Next(posicionValors.Length);
                    pos = posicionValors[pos].Position;
                    longitudFrase = MyContainer.MyTDM.PhrasesList[pos].Length;
                } while (ActivePhrases.Contains(pos));

                ActivePhrases.Add(pos);
                SummaryLength += longitudFrase;
            }
        }

        /// <summary>
        /// Add sentences to the end of the solution to complete the size. Select phrases that have a
        /// lot of coverage.
        /// </summary>
        public void OptimizationComplete()
        {
            try
            {
                var frasesPosibles = MyContainer.ViablePhrases;
                var posicioncandidata = 0;
                while (SummaryLength < MyContainer.MyParameters.MaximumLengthOfSummaryForRouge)
                {
                    int idFraseCandidata;
                    do
                    {
                        idFraseCandidata = frasesPosibles[posicioncandidata].Position;
                        posicioncandidata++;
                    } while (ActivePhrases.Contains(idFraseCandidata));

                    SummaryLength += MyContainer.MyTDM.PhrasesList[idFraseCandidata].Length;
                    ActivePhrases.Add(idFraseCandidata);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("Index error caught in OptimizationComplete : " + e.Message);
            }
        }

        /// <summary>
        /// Calculate the summary length in words
        /// </summary>
        public void CalculateSummaryLengt()
        {
            var totalWords = 0;
            foreach (var posFrase in ActivePhrases)
                totalWords += MyContainer.MyTDM.PhrasesList[posFrase].Length;
            SummaryLength = totalWords;
        }

        /// <summary>
        /// Calculate the fitness of the solution. You MUST make sure that the Summary Length was
        /// calculated before.
        /// </summary>
        /// <param name="recalcular">If TRUE, only part of the fitness is recalculated</param>
        public virtual void CalculateFitness(bool recalcular = false)
        {
            var alfa = MyContainer.MyParameters.Alfa;
            var beta = MyContainer.MyParameters.Beta;
            var gamma = MyContainer.MyParameters.Gamma;
            var delta = MyContainer.MyParameters.Delta;
            var ro = MyContainer.MyParameters.Ro;

            if (recalcular)
            {
                RecalculateObjectiveFitnessFor_MA_SingleDocSum(alfa, beta, gamma, delta, ro);
                return;
            }

            MyContainer.CurrentFFEs++;

            {
                switch (MyContainer.MyParameters.TheFitnessFunction)
                {
                    case FitnessFunction.MCMR: CalculateMCMR();
                        break;
                    case FitnessFunction.CRP:
                        CRRSCNPos(alfa, beta, gamma);
                        break;
                    case FitnessFunction.MASDS:
                        ObjectiveFitnessFor_MA_SingleDocSum(alfa, beta, gamma, delta, ro);
                        break;
                }
            }
        }

        /// <summary>
        /// Obtain Solution Phrases Sorted By Coverage based on Cosine Similarity.
        /// </summary>
        /// <returns></returns>
        protected List<PositionValue> ObtainPhrasesSortedByCoverageCosine()
        {
            var selectedPhrases = new List<PositionValue>();
            var alfa = MyContainer.MyParameters.Alfa;
            var gamma = MyContainer.MyParameters.Gamma;
            var ro = MyContainer.MyParameters.Ro;

            foreach (var gen in ActivePhrases)
                selectedPhrases.Add(new PositionValue(gen,
                    (alfa * MyContainer.PhrasePositionRanking[gen] / 
                           MyContainer.PhrasePositionRanking[0]) +
                    (gamma * MyContainer.MyTDM.PhrasesList[gen].Length /
                           MyContainer.OrderedLengths[0])   +
                    ro * MyContainer.MyTDM.PhrasesList[gen].SimilarityToDocument));
            selectedPhrases.Sort((x, y) => -1 * x.Value.CompareTo(y.Value));
            return selectedPhrases;
        }

        public void CRRSCNPos(double alfa, double beta, double gamma)
        {
            var fs = ActivePhrases;
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
            var documentPosition = 0.0;
            foreach (var frase in fs)
                documentPosition += MyContainer.PhrasePositionRanking[frase];
            documentPosition /= fs.Count;
            documentPosition /= MyContainer.PhrasePositionRanking[0]; // To normalize in reason to the greater

            // calculate the total fitness
            Fitness = (alfa * coverage) - (beta * redundancy) + (gamma * documentPosition);
        }

        public void ObjectiveFitnessFor_MA_SingleDocSum(double alfa, double beta, double gama, double delta, double ro)
        {
            if (alfa > 0) PositionFactor = CalculatePositionFactor(ActivePhrases);
            if (beta > 0) TitleSimilarityFactor = CalculateTitleSimilarityFactor(ActivePhrases);
            if (gama > 0) LengthFactor = CalculateLengthFactor(ActivePhrases);
            if (delta > 0) CohesionFactor = CalculateCohesionFactor(ActivePhrases);
            if (ro > 0) CoverageFactor = CalculateCoverageFactor(ActivePhrases);

            Fitness = (alfa * PositionFactor) + (beta * TitleSimilarityFactor) + (gama * LengthFactor) +
                      (delta * (CohesionFactor - MyContainer.MinCohesion) / (MyContainer.MaxCohesion - MyContainer.MinCohesion)) +
                      (ro * (CoverageFactor - MyContainer.MinCoverage) / (MyContainer.MaxCoverage - MyContainer.MinCoverage));
        }

        public void RecalculateObjectiveFitnessFor_MA_SingleDocSum(double alfa, double beta, double gama, double delta, double ro)
        {
            Fitness = (alfa * PositionFactor) + (beta * TitleSimilarityFactor) + (gama * LengthFactor) +
                      (delta * (CohesionFactor - MyContainer.MinCohesion) / (MyContainer.MaxCohesion - MyContainer.MinCohesion)) +
                      (ro * (CoverageFactor - MyContainer.MinCoverage) / (MyContainer.MaxCoverage - MyContainer.MinCoverage));
        }

        /// <summary>
        /// Calculate the position factor of the summary. Phrases closest to the beginning of the document
        /// are supposed to be more important (applies great for news).
        /// </summary>
        /// <param name="selectedPhrases"></param>
        /// <returns></returns>
        public double CalculatePositionFactor(List<int> selectedPhrases)
        {
            var size = selectedPhrases.Count;
            var documentPosition = 0.0;
            foreach (var frase in selectedPhrases)
                documentPosition += MyContainer.PhrasePositionRanking[frase];
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

            var pf = (documentPosition - minpos)/ (maxpos - minpos); // Normalize based on range

            return pf;
        }

        /// <summary>
        /// Calculate the relationship with the subject, which implies the similarity with the title of the document
        /// </summary>
        /// <returns>A value of type double that indicates the weight of the Topic Relation Factor for the summary</returns>
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
        /// 
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
            
            var factor = (averageLength - minLon) / (maxLon - minLon);
            return factor;
        }

        /// <summary>
        /// Calculate the Cohesion Factor of a summary. Indicates the degree to which
        /// the summary sentences tell about the same information
        /// </summary>
        /// <param name="selectedPhrases">Phrases List that ...</param>
        /// <returns></returns>
        public double CalculateCohesionFactor(List<int> selectedPhrases)
        {
            var myMDS = MyContainer.MyExternalMDS;
            var size = selectedPhrases.Count;
            var cs = 0.0;
            var total = 0;
            for (var i = 0; i < size; i++)
                for (var j = i + 1; j < size; j++)
                {
                    cs += myMDS.GetCosineSimilarityBetweenPhrases(selectedPhrases[i], selectedPhrases[j]);
                    total++;
                }
            cs = cs / total;

            if (cs > MyContainer.MaxCohesion)
            {
                MyContainer.MaxCohesion = cs;
                MyContainer.UpdateFitness = true;
            }
            else
            {
                if (!(cs < MyContainer.MinCohesion)) return cs;
                MyContainer.MinCohesion = cs;
                MyContainer.UpdateFitness = true;
            }
            return cs;
        }

        /// <summary>
        /// Calculate the coverage factor
        /// </summary>
        /// <returns>A value of type double that indicates the Coverage Factor of the summary</returns>
        public double CalculateCoverageFactor(List<int> selectedPhrases)
        {
            var coverage = MyContainer.MyTDM.CosineSimilarityFromSummaryTextToDocument(selectedPhrases, MyContainer.MyParameters.MaximumLengthOfSummaryForRouge);

            if (coverage > MyContainer.MaxCoverage)
            {
                MyContainer.MaxCoverage = coverage;
                MyContainer.UpdateFitness = true;
            }
            else
            {
                if (!(coverage < MyContainer.MinCoverage)) return coverage;
                MyContainer.MinCoverage = coverage;
                MyContainer.UpdateFitness = true;
            }
            return coverage;
        }

        /// <summary>
        /// Fitness function based on CalculateMCMR (Maximum Coverage Minimum Redundancy)
        /// </summary>
        public void CalculateMCMR()
        {
            var fs = ActivePhrases;

            var fitnessCoseno = 0.0;
            for (var i = 0; i < fs.Count - 1; i++)
                for (var j = i + 1; j < fs.Count; j++)
                {
                    var posI = fs[i];
                    var posJ = fs[j];
                    fitnessCoseno += MyContainer.MyTDM.PhrasesList[posI].SimilarityToDocument +
                                   MyContainer.MyTDM.PhrasesList[posJ].SimilarityToDocument -
                                    MyContainer.MyExternalMDS.CosineSimilarityBetweenPhrases[posI][posJ];
                }

            Fitness = fitnessCoseno;
        }
    }
}