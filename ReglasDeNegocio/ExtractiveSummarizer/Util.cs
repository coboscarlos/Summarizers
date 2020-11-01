using System.Collections.Generic;
using System.Diagnostics;
using BusinessRules.Utils;
using BusinessRules.VectorSpaceModel;

namespace BusinessRules.ExtractiveSummarizer
{
    public class Util
    {
        public static string SummarizeByCompressionRatio(TDM miTDM,
                                                List<KeyValuePair<int, double>> phrasesList,
                                                SummaryType summaryType,
                                                int summaryLength, out List<KeyValuePair<string, int>> summaryXML)
        {
            var genSummary = "";
            summaryXML = new List<KeyValuePair<string, int>>();
            var i = 0;

            if (summaryType == SummaryType.Words)
            {
                string[] words;
                do
                {
                    genSummary += miTDM.PhrasesList[phrasesList[i].Key].OriginalText + " ";
                    summaryXML.Add(new KeyValuePair<string, int>(miTDM.PhrasesList[phrasesList[i].Key].OriginalText, phrasesList[i].Key));
                    i++;
                    words = genSummary.TrimEnd(' ').Split(' ');
                } while (words.Length < summaryLength && i < phrasesList.Count);

                genSummary = "";
                for (i = 0; i < summaryLength; i++)
                {
                    if (i > words.Length - 1) continue;
                    genSummary += words[i] + " ";
                }

                var myLength = genSummary.TrimEnd(' ').Split(' ').Length;
                if (myLength < summaryLength)
                    Debug.WriteLine("**************OJO: short : " + myLength + " **************");
                return (genSummary.Trim());
            }

            if (summaryType == SummaryType.Sentences) {
                for (var f = 0; f < phrasesList.Count; f++)
                {
                    if (f >= summaryLength) break;
                    genSummary += miTDM.PhrasesList[phrasesList[f].Key].OriginalText + " ";
                }
                return (genSummary.Trim());
            }

            return (genSummary.Trim());
        }
    }
}