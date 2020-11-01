using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace BusinessRules.Utils
{
    public class PhraseData : IEquatable<string>, IComparable<PhraseData>
    {
        public readonly string OriginalText;
        public string ProcessedText;
        public readonly int PositionInDocument;
        public readonly Dictionary<string, int> UniqueTermsInProcessedText = new Dictionary<string, int>();
        public int MaximumFrequency;
        public int Length;
        public int ProcessedLength;
        public double SimilarityToDocument;
        public double SimilarityToTitle;

        public string Id => PositionInDocument.ToString(CultureInfo.InvariantCulture);

        public PhraseData(string originalText, string processedText, int positionInDocument)
        {
            OriginalText = originalText;
            ProcessedText = processedText;
            PositionInDocument = positionInDocument;
            Length = OriginalText.Split(' ').Length;
            CalculateTermsWithFrequencies();
        }

        public PhraseData(PhraseData originalPhraseData)
        {
            OriginalText = originalPhraseData.OriginalText;
            ProcessedText = originalPhraseData.ProcessedText;
            PositionInDocument = originalPhraseData.PositionInDocument;
            foreach (var term in originalPhraseData.UniqueTermsInProcessedText)
                UniqueTermsInProcessedText.Add(term.Key, term.Value);
            MaximumFrequency = originalPhraseData.MaximumFrequency;
            Length = originalPhraseData.Length;
            ProcessedLength = originalPhraseData.ProcessedLength;
            SimilarityToDocument = originalPhraseData.SimilarityToDocument;
            SimilarityToTitle = originalPhraseData.SimilarityToTitle;
        }

        // Calculate all the terms of the original processed text with its frequencies
        private void CalculateTermsWithFrequencies()
        {
            UniqueTermsInProcessedText.Clear();
            ProcessedText = ProcessedText.Trim();
            var terms = ProcessedText.Split(' ');
            foreach (var term in terms)
            {
                if (term == "") continue; // blank space not valid

                if (UniqueTermsInProcessedText.ContainsKey(term))
                    UniqueTermsInProcessedText[term] += 1;
                else
                    UniqueTermsInProcessedText.Add(term, 1);
            }
            CalculateMaximumFrequency();
            CalculateProcessedLength();
        }

        // When a term is deleted from the data dictionary (TermsTable) it must be removed
        // from the phrases ... in the processed text and in the unique terms with its frequencies
        public void RemoveTerm(string term)
        {
            ProcessedText = ProcessedText.Replace(term, "");
            ProcessedText = ProcessedText.Replace("  ", " ");

            if (UniqueTermsInProcessedText.ContainsKey(term))
                UniqueTermsInProcessedText.Remove(term);
            CalculateMaximumFrequency();
            CalculateProcessedLength();
        }

        private void CalculateMaximumFrequency()
        {
            MaximumFrequency = UniqueTermsInProcessedText.Max(t => t.Value);
        }

        private void CalculateProcessedLength()
        {
            ProcessedLength = UniqueTermsInProcessedText.Sum(t => t.Value);
        }

        public override string ToString()
        {
            return PositionInDocument + ":" + OriginalText + "|:|" + ProcessedText;
        }

        public bool Equals(string other)
        {
            if (other == null) return false;
            return other == Id;
        }

        public int CompareTo(PhraseData other)
        {
            if (Math.Abs(SimilarityToDocument - other.SimilarityToDocument) < 1e-07)
                return PositionInDocument.CompareTo(other.PositionInDocument);

            return -1 * SimilarityToDocument.CompareTo(other.SimilarityToDocument);
        }
    }
}