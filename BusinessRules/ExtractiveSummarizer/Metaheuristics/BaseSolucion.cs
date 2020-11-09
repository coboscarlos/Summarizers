using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BusinessRules.Utils;

namespace BusinessRules.ExtractiveSummarizer.Metaheuristics
{
    public abstract partial class BaseSolucion : IEquatable<BaseSolucion>, IComparable<BaseSolucion>
    {
        public BaseAlgorithm MyContainer;

        public double Fitness;
        public double PositionFactor;
        public double TitleSimilarityFactor;
        public double LengthFactor;
        public double CohesionFactor;
        public double CoverageFactor;

        private readonly int[] _phrases;
        public int Phrase(int i) { return _phrases[i]; }
        public List<int> SelectedPhrases { get; }
        public List<int> UnselectedPhrases { get; }
        public int SummaryLength { get; private set; }

        public void Activate(int positionPhrase)
        {
            if (_phrases[positionPhrase] == 1) return;
            _phrases[positionPhrase] = 1;
            SummaryLength += MyContainer.MyTDM.PhrasesList[positionPhrase].Length;
            SelectedPhrases.Add(positionPhrase);
            SelectedPhrases.Sort();
            UnselectedPhrases.Remove(positionPhrase);
            UnselectedPhrases.Sort();
        }

        public void InActivate(int positionPhrase)
        {
            if (_phrases[positionPhrase] == 0) return;
            _phrases[positionPhrase] = 0;
            SummaryLength -= MyContainer.MyTDM.PhrasesList[positionPhrase].Length;
            SelectedPhrases.Remove(positionPhrase);
            SelectedPhrases.Sort();
            UnselectedPhrases.Add(positionPhrase);
            UnselectedPhrases.Sort();
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="myContainer"></param>
        protected BaseSolucion(BaseAlgorithm myContainer)
        {
            MyContainer = myContainer;
            SummaryLength = 0;
            SelectedPhrases = new List<int>();
            UnselectedPhrases = new List<int>();
            _phrases = new int[myContainer.SolutionSize];
            for (var i = 0; i < MyContainer.MyTDM.PhrasesList.Count; i++)
            {
                _phrases[i] = 0;
                UnselectedPhrases.Add(i);
            }
            Fitness = 0;
        }

        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="origin"></param>
        protected BaseSolucion(BaseSolucion origin)
        {
            MyContainer = origin.MyContainer;
            Fitness = origin.Fitness;
            PositionFactor = origin.PositionFactor;
            TitleSimilarityFactor = origin.TitleSimilarityFactor;
            LengthFactor = origin.LengthFactor;
            CohesionFactor = origin.CohesionFactor;
            CoverageFactor = origin.CoverageFactor;
            SummaryLength = origin.SummaryLength;
            for (var i = 0; i < MyContainer.MyTDM.PhrasesList.Count; i++) _phrases[i] = origin._phrases[i];
            SelectedPhrases = new List<int>();
            SelectedPhrases.AddRange(origin.SelectedPhrases);
            UnselectedPhrases = new List<int>();
            UnselectedPhrases.AddRange(origin.UnselectedPhrases);
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
            var thisPhrases = SelectedPhrases;
            var otherPhrases = other.SelectedPhrases;
            return otherPhrases.Count == thisPhrases.Count && 
                   thisPhrases.All(posFrase => otherPhrases.Contains(posFrase));
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
            var thisLength = SummaryLength;
            var soyValido = (thisLength >= minLon && thisLength <= maxLon);
            var otherLength = other.SummaryLength;
            var otroEsValido = (otherLength >= minLon && otherLength <= maxLon);
            if (soyValido == otroEsValido) return -1 * Fitness.CompareTo(other.Fitness);
            if (soyValido) return -1;
            return 1;
        }

        public override string ToString()
        {
            var selectedPhrases = new StringBuilder("");
            selectedPhrases.Append("F:" + Fitness.ToString("#0.000") + " [");
            foreach (var pos in SelectedPhrases)
                selectedPhrases.Append(pos + ",");
            selectedPhrases.Remove(selectedPhrases.Length - 1, 1);
            selectedPhrases.Append("] ");
            selectedPhrases.Append("L:" + SummaryLength.ToString("###") + "");
            return selectedPhrases.ToString();
        }

        /// <summary>
        /// Random initialization of a the agent, taking into account that the phrases are different from each other
        /// </summary>
        public void RandomInitialization()
        {
            AddValidPhrases();
            CalculateFitness();
        }

        /// <summary>
        /// Include as many phrases as possible (randomly) while the summary is still feasible
        /// </summary>
        public void AddValidPhrases()
        {
            while (SummaryLength < MyContainer.MyParameters.MaximumLengthOfSummaryForRouge)
            {
                var positions = ValidPhrases();
                if (positions is null) break;

                int pos;
                do
                {
                    pos = MyContainer.MyParameters.RandomGenerator.Next(positions.Count);
                    pos = positions[pos];
                } while (SelectedPhrases.Contains(pos));
                Activate(pos);
            }
        }

        public List<int> ValidPhrases()
        {
            var availableSpace = MyContainer.MyParameters.MaximumLengthOfSummaryForRouge - SummaryLength;
            var search = SelectedPhrases.Where(
                x => MyContainer.MyTDM.PhrasesList[x].Length < availableSpace);
            return search.ToList();
        }

        /// <summary>
        /// Add sentences to the end of the solution to complete the size. Select phrases that have a
        /// lot of coverage.
        /// </summary>
        public void CompleteSummaryBasedOnCoverage()
        {
            while (SummaryLength < MyContainer.MyParameters.MaximumLengthOfSummaryForRouge)
            {
                var viable = ViablePhrasesOrderedByCoverage();
                if (viable is null) break;
                Activate(viable[0].Position);
            }
        }

        public List<PositionValue> ViablePhrasesOrderedByCoverage()
        {
            var positions = ValidPhrases();
            if (positions is null) return null;

            var viable = positions.Select(t => new
                PositionValue(t, MyContainer.MyTDM.PhrasesList[t].SimilarityToDocument)).ToList();
            viable.Sort((x, y) => -1 * x.Value.CompareTo(y.Value));
            return viable;
        }

        /// <summary>
        /// Obtain Solution _phrases Sorted By Coverage based on Cosine Similarity.
        /// </summary>
        /// <returns></returns>
        protected List<PositionValue> ObtainSelectedPhrasesSortedByCoverageCosine()
        {
            var sortedSelectedPhrases = new List<PositionValue>();
            var alfa = MyContainer.MyParameters.Alfa;
            var gamma = MyContainer.MyParameters.Gamma;
            var ro = MyContainer.MyParameters.Ro;

            foreach (var gen in SelectedPhrases)
                sortedSelectedPhrases.Add(new PositionValue(gen,
                    (alfa * MyContainer.PhrasePositionRanking[gen] /
                     MyContainer.PhrasePositionRanking[0]) +
                    (gamma * MyContainer.MyTDM.PhrasesList[gen].Length /
                     MyContainer.OrderedLengths[0]) +
                    ro * MyContainer.MyTDM.PhrasesList[gen].SimilarityToDocument));
            sortedSelectedPhrases.Sort((x, y) => -1 * x.Value.CompareTo(y.Value));
            return sortedSelectedPhrases;
        }
    }
}