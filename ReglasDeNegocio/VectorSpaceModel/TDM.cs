using System;
using System.Diagnostics;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using BusinessRules.Utils;
using System.Text;

namespace BusinessRules.VectorSpaceModel
{
    public partial class TDM
    {
        private const double K = 2;    // Used for OKAPI BM25 Weight
        private const double B = 0.75; // Used for OKAPI BM25 Weight

        protected readonly TDMParameters MyTDMParameters;
        private double[] _titleTFIDFVector;
        private double[] _documentTFIDFVector; // Vector TF-IDF that represents the document as Text in the multidimensional space
        private readonly Dictionary<string, int> _termsTable = new Dictionary<string, int>();
        private readonly Dictionary<int, string> _invertedTermsTable = new Dictionary<int, string>();
        private double[] _idfVector;
        private double _averageLengthOfPhrases;

        public double[][] TFIDFMatrix;       // Matrix TF-IDF that represents the sentences of the documents in the multidimensional space
        public List<PhraseData> PhrasesList = new List<PhraseData>();
        public List<double> SimilaritiesOrderedToTheTitle;

        public TDM(string documentFile, TDMParameters myTDMParameters, string cacheFileName)
        {
            var extension = myTDMParameters.TheTFIDFWeight == TFIDFWeight.Doc2Vec ? ".d2v" : ".tdm";
            MyTDMParameters = myTDMParameters;

            if (cacheFileName.Length != 0)
            {
                if (File.Exists(cacheFileName + extension))
                {
                    ReadFromFile(cacheFileName + extension);
                    return;
                }
            }

            // Sentences are indexed in RAM using Lucene
            var theIndexer = new LuceneIndexer();
            theIndexer.InMemory(documentFile);
            PhrasesList = theIndexer.PhraseList;
            var processedTitle = theIndexer.DocumentTitle;

            if (MyTDMParameters.TheTFIDFWeight == TFIDFWeight.Doc2Vec)
            {
                CalculateDoc2Vec(PhrasesList, theIndexer.DocumentTitle, 
                    cacheFileName,out TFIDFMatrix, out _documentTFIDFVector, 
                    out _titleTFIDFVector);

                SaveToFile(cacheFileName + extension);
                return;
            }

            // Organize the dictionary of direct and inverted terms after processing with LUCENE
            var counter = 0;
            foreach (var term in theIndexer.TermsList)
            {
                if (term == " ") continue;
                _termsTable.Add(term, counter);
                _invertedTermsTable.Add(counter, term);
                counter++;
            }

            Debug.WriteLine("Original TDM is: " + PhrasesList.Count + " x " + _termsTable.Count);

            CreateTDMMatrix(PhrasesList, _termsTable, myTDMParameters,
                           out TFIDFMatrix, out var termFrequencyInCollection,
                           out _, out _idfVector, out _documentTFIDFVector, out _averageLengthOfPhrases);

            // If you change the term dictionary (TermsTable) for terms that do not exceed
            // the threshold, all structures are recalculated
            if (RemoveTermsThatAreNotSignificant(termFrequencyInCollection, MyTDMParameters.MinimumFrequencyThresholdOfTermsForPhrase))
            {
                CreateTDMMatrix(PhrasesList, _termsTable, myTDMParameters,
                    out TFIDFMatrix, out termFrequencyInCollection,
                    out _, out _idfVector, out _documentTFIDFVector, out _averageLengthOfPhrases);
            }

            // If you change the list of phrases by sentences that do not exceed
            // the threshold, all structures are recalculated
            if (RemovePhrasesThatAreNotSignificant())
            {
                UpdateTermsTable(PhrasesList);
                CreateTDMMatrix(PhrasesList, _termsTable, myTDMParameters, out TFIDFMatrix,
                    out termFrequencyInCollection,
                    out _, out _idfVector, out _documentTFIDFVector, out _averageLengthOfPhrases);
            }
            Debug.WriteLine("TDM final de : " + PhrasesList.Count + " x " + _termsTable.Count);

            _titleTFIDFVector = CalculateTitleWeights(processedTitle, _termsTable, myTDMParameters, _averageLengthOfPhrases, _idfVector);

            SortSimilaritiesToTitle();

            SaveToFile(cacheFileName + extension);
        }

        private void SortSimilaritiesToTitle()
        {
            // The similarity of each phrase to the document is calculated and ordered
            SimilaritiesOrderedToTheTitle = new List<double>();
            for (var posPhrase = 0; posPhrase < PhrasesList.Count; posPhrase++)
            {
                PhrasesList[posPhrase].SimilarityToDocument = Calculus.CosineSimilarity(TFIDFMatrix[posPhrase], _documentTFIDFVector);
                PhrasesList[posPhrase].SimilarityToTitle = Calculus.CosineSimilarity(TFIDFMatrix[posPhrase], _titleTFIDFVector);
                SimilaritiesOrderedToTheTitle.Add(PhrasesList[posPhrase].SimilarityToTitle);
            }
            SimilaritiesOrderedToTheTitle.Sort((x, y) => -1 * x.CompareTo(y));
        }

        private static void CreateTDMMatrix(List<PhraseData> phrasesList, Dictionary<string, int> termsTable,
            TDMParameters myTDMParameters, out double[][] matrixTFIDF, out int[] termFrequencyInCollection,
            out int[] observedIDFVector, out double[] vectorIDF, out double[] documentTFIDFVector, 
            out double averageLengthOfPhrases)
        {
            // The observed frequency of each term i is calculated in each phrase (TF or Fi),
            // and the frequency of each term is stored in the entire collection and the
            // number of sentences in which a term appears in the entire document
            matrixTFIDF = new double[phrasesList.Count][];
            termFrequencyInCollection = new int[termsTable.Count];
            observedIDFVector = new int[termsTable.Count];
            vectorIDF = new double[termsTable.Count];

            averageLengthOfPhrases = phrasesList.Aggregate(0.0, (current, phrase) => current + phrase.ProcessedLength);
            averageLengthOfPhrases /= phrasesList.Count;

            for (var posPhrase = 0; posPhrase < phrasesList.Count; posPhrase++)
            {
                matrixTFIDF[posPhrase] = new double[termsTable.Count];
                var termsToRemove = new List<string>();
                foreach (var term in phrasesList[posPhrase].UniqueTermsInProcessedText)
                {
                    if (termsTable.ContainsKey(term.Key))
                    {
                        var posTerm = termsTable[term.Key];
                        matrixTFIDF[posPhrase][posTerm] = term.Value;
                        termFrequencyInCollection[posTerm] += term.Value;
                        observedIDFVector[posTerm] += 1;
                    }
                    else
                    {
                        //The terms that should be eliminated in the sentence are marked because they are not in the collection
                        termsToRemove.Add(term.Key);
                    }
                }

                // The terms that are not included in the collection are eliminated
                foreach (var term in termsToRemove)
                    phrasesList[posPhrase].RemoveTerm(term);
            }

            // The IDFVector is calculated. It store the IDF value for each term in the corpus
            //    Simple Weight   IDF i = ln ( N / ni ) = LexRank
            //    Complete Weight IDF i = ln ( N / (ni+1) )
            //    BM25 Weight     IDF i = ln ( N / ni )
            //    Best Weight     IDF i = log10 ( N / ni )
            //    where N is the total number of phrases and ni is the number of phrases in which the term i appears
            for (var posTerm = 0; posTerm < termsTable.Count; posTerm++)
            {
                double result2 = observedIDFVector[posTerm];

                switch (myTDMParameters.TheTFIDFWeight )
                {
                    case TFIDFWeight.Simple:
                        vectorIDF[posTerm] = Math.Log(phrasesList.Count/result2);
                        break;
                    case TFIDFWeight.Complete:
                        vectorIDF[posTerm] = Math.Log(phrasesList.Count / (result2 + 1));
                        break;
                    case TFIDFWeight.BM25:                        
                        vectorIDF[posTerm] = Math.Log(phrasesList.Count / result2);
                        break;
                    case TFIDFWeight.Best:
                        vectorIDF[posTerm] = Math.Log10(phrasesList.Count / result2);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            // 4 Weight Matrix (W) - [TF-IDF] for each term y Phrase ... IDF value is stored in a separated vector
            for (var posPhrase = 0; posPhrase < phrasesList.Count; posPhrase++)
            {
                var lengthThisPhrase = phrasesList[posPhrase].ProcessedLength;
                foreach (var term in phrasesList[posPhrase].UniqueTermsInProcessedText)
                {
                    var posTerm = termsTable[term.Key];
                    matrixTFIDF[posPhrase][posTerm] = MultiplyTFbyIDFUsingWeights(matrixTFIDF[posPhrase][posTerm],
                        vectorIDF[posTerm], myTDMParameters.TheTFIDFWeight, phrasesList[posPhrase].MaximumFrequency, 
                        lengthThisPhrase, averageLengthOfPhrases);
                }
            }

            // 5. Calculate the centroid or query vector that represents the document
            documentTFIDFVector = null;
            switch (myTDMParameters.TheDocumentRepresentation)
            {
                case DocumentRepresentation.Centroid:
                    documentTFIDFVector = CalculateCentroidOfDocument(matrixTFIDF);
                    break;
                case DocumentRepresentation.Vector:
                    documentTFIDFVector = CalculateDocumentQueryVector(termFrequencyInCollection, myTDMParameters, averageLengthOfPhrases, vectorIDF);
                    break;
            }
        }

        /// <summary>
        /// With the phraseList is checked that the dictionary does not change, if so,
        /// the TermsTable and the InvertedTermsTable are recalculated
        /// </summary>
        /// <param name="phrasesList">List of phrases to get the dictionary</param>
        private void UpdateTermsTable(List<PhraseData> phrasesList)
        {
            var terms = new List<string>();
            foreach (var phrase in phrasesList)
            {
                foreach (var term in phrase.UniqueTermsInProcessedText)
                {
                    if (terms.Contains(term.Key)) continue;
                    terms.Add(term.Key);
                }
            }

            terms.Sort();
            if (terms.Count == _termsTable.Count) return; // These phrases have the same previous dictionary

            _termsTable.Clear();
            _invertedTermsTable.Clear();
            var counter = 0;
            foreach (var term in terms)
            {
                if (term == " ") continue;
                _termsTable.Add(term, counter);
                _invertedTermsTable.Add(counter, term);
                counter++;
            }
        }

        /// <summary>
        /// It eliminates the infrequent terms and updates the TermsTable and the
        /// InvertedTermsTable, therefore, the TFIDF matrix must be recalculated
        /// </summary>
        /// <param name="termsFrequencyInDocument">current frequency of each term in the collection</param>
        /// <param name="minimumFrequencyThresholdOfTermsForPhrase">minimum threshold to take into account the term in the dictionary</param>
        /// <returns>true if the tables changed, false if nothing changed</returns>
        private bool RemoveTermsThatAreNotSignificant(int[] termsFrequencyInDocument, int minimumFrequencyThresholdOfTermsForPhrase)
        {
            // I list the terms that should be deleted
            var termToRemove = new List<int>();
            for (var i = 0; i < _termsTable.Count; i++)
            {
                if (termsFrequencyInDocument[i] < minimumFrequencyThresholdOfTermsForPhrase)
                {
                    termToRemove.Add(i);
                    continue;
                }
                if (termsFrequencyInDocument[i] == 0) termToRemove.Add(i);
            }
            if (termToRemove.Count == 0) return false;

            termToRemove.Sort((x, y) => -1 * x.CompareTo(y)); // order from highest to lowest

            // I create the new dictionary of Terms - direct and inverted, eliminating those that are not frequent
            var newTermsTable = new Dictionary<string, int>();
            var newInvertedTermsTable = new Dictionary<int, string>();
            var newPosition = 0;
            for (var posTerm = 0; posTerm < _termsTable.Count; posTerm++)
            {
                if (termToRemove.Contains(posTerm)) 
                    continue;
                var key = _invertedTermsTable[posTerm];
                newTermsTable.Add(key, newPosition);
                newInvertedTermsTable.Add(newPosition, key);
                newPosition++;
            }

            _termsTable.Clear();
            foreach(var ter in newTermsTable)
                _termsTable.Add(ter.Key, ter.Value);

            _invertedTermsTable.Clear();
            foreach (var invertedTerm in newInvertedTermsTable)
                _invertedTermsTable.Add(invertedTerm.Key, invertedTerm.Value);

            Debug.WriteLine("Total of terms eliminated: " + termToRemove.Count);
            return true;
        }

        /// <summary>
        /// Remove phrases that are very different to the document.
        /// By default, it eliminates phrases that do not resemble at least 0.00001 and that do not remain with terms.
        /// If phrases are deleted the terms MUST be processed and all structures, including the TFIDF matrix. 
        /// </summary>
        /// <returns>true if phrases were deleted, false otherwise</returns>
        private bool RemovePhrasesThatAreNotSignificant()
        {
            var phrasesToRemove = new List<int>();
            for (var i = 0; i < PhrasesList.Count; i++)
            {
                if (PhrasesList[i].UniqueTermsInProcessedText.Count == 0)
                {
                    phrasesToRemove.Add(i);
                    continue;
                }
                if (Calculus.CosineSimilarity(TFIDFMatrix[i], _documentTFIDFVector) < MyTDMParameters.MinimumThresholdForTheAcceptanceOfThePhrase)
                {
                    phrasesToRemove.Add(i);
                    continue;
                }
                if (Calculus.CosineSimilarity(TFIDFMatrix[i], _documentTFIDFVector) < 0.00001)
                    phrasesToRemove.Add(i);   
            }
            if (phrasesToRemove.Count == 0) return false;

            phrasesToRemove.Sort((x,y) => -1 * x.CompareTo(y)); // order from highest to lowest

            // Remove phrases that did not pass the threshold
            foreach (var posEli in phrasesToRemove)
                PhrasesList.RemoveAt(posEli);

            Debug.WriteLine("Total of phrases eliminated: " + phrasesToRemove.Count);
            return true;
        }

        private static double[] CalculateCentroidOfDocument(double[][] matrixTFIDF)
        {
            var totalTerms = matrixTFIDF[0].GetUpperBound(0) + 1;
            var totalPhrases = matrixTFIDF.GetUpperBound(0) + 1;
            var documentTFIDFVector = new double[totalTerms];
            for (var i = 0; i < totalPhrases; i++)
                for (var j = 0; j < totalTerms; j++)
                    documentTFIDFVector[j] += matrixTFIDF[i][j];
            for (var j = 0; j < totalTerms; j++)
                documentTFIDFVector[j] = documentTFIDFVector[j] / totalPhrases;
            return documentTFIDFVector;
        }

        private static double[] CalculateDocumentQueryVector(int[] termFrequencyInDocument,
            TDMParameters myTDMParameters, double averageLengthOfPhrases, double[] vectorIDF)
        {
            var totalTerms = termFrequencyInDocument.Length;
            var maximumFrequency = 0.0;
            var documentLength = 0;
            for (var j = 0; j < totalTerms; j++)
            {
                documentLength += termFrequencyInDocument[j];
                if (termFrequencyInDocument[j] > maximumFrequency)
                    maximumFrequency = termFrequencyInDocument[j];
            }

            var documentTFIDFVector = new double[totalTerms];
            for (var j = 0; j < totalTerms; j++)
            {
                documentTFIDFVector[j] = MultiplyTFbyIDFUsingWeights(termFrequencyInDocument[j], 
                            vectorIDF[j], myTDMParameters.TheTFIDFWeight,
                            maximumFrequency, documentLength, averageLengthOfPhrases);
            }

            return documentTFIDFVector;
        }

        private static double[] CalculateTitleWeights(string processedTitle, 
            Dictionary<string, int> termsTable, TDMParameters myTDMParameters, 
            double averageLengthOfPhrases, double[] vectorIDF)
        {
            var titleTFVector = new double[termsTable.Count];
            if (processedTitle.Length == 0) return titleTFVector;
            var terms = processedTitle.Split(' ');
            var maximumFrequency = 0.0;
            var noRepeatedTerms = new List<string>();
            foreach (var term in terms)
            {
                if (!termsTable.ContainsKey(term)) continue;
                var posTerm = termsTable[term];
                titleTFVector[posTerm] = titleTFVector[posTerm] + 1;

                if (!noRepeatedTerms.Contains(term))
                    noRepeatedTerms.Add(term);

                if (titleTFVector[posTerm] > maximumFrequency)
                    maximumFrequency = titleTFVector[posTerm];
            }
            var titleLength = terms.Length;

            var titleTFIDFVector = new double[termsTable.Count];
            foreach (var term in noRepeatedTerms)
            {
                var posTerm = termsTable[term];
                titleTFIDFVector[posTerm] = MultiplyTFbyIDFUsingWeights(titleTFVector[posTerm], 
                            vectorIDF[posTerm], myTDMParameters.TheTFIDFWeight,
                            maximumFrequency, titleLength, averageLengthOfPhrases);
            }

            return titleTFIDFVector;
        }

        public double CosineSimilarityFromSummaryTextToDocument(List<int> selectedPhrases, 
            int maximumLengthOfSummaryForRouge)
        {
            double[] summaryTFIDFVector;

            // Calculate similarity based on Bag-Of-Words representation
            if (_termsTable.Count != 0)
            {
                var summary = new StringBuilder();
                var totalWords = 0;
                foreach (var pos in selectedPhrases)
                {
                    var words = PhrasesList[pos].OriginalText.Split(' ');
                    totalWords = totalWords + words.Length;
                    summary.Append(PhrasesList[pos].ProcessedText + " ");
                }

                // 1. Fill the vector with the frequencies of the terms in the summary
                var dividedText = summary.ToString().Trim().Split(' ');
                var summaryTFVector = new double[_termsTable.Count];

                foreach (var term in dividedText)
                {
                    if (_termsTable.ContainsKey(term))
                    {
                        var pos = _termsTable[term];
                        summaryTFVector[pos]++;
                    }
                }

                //2. Calculate the maximum frequency in the summary
                var maximumFrequency = summaryTFVector.Max();

                //3. Calculate the TF-IDF for each term of the summary
                summaryTFIDFVector = new double[_termsTable.Count];
                for (var posTerm = 0; posTerm < _termsTable.Count; posTerm++)
                {
                    summaryTFIDFVector[posTerm] = MultiplyTFbyIDFUsingWeights(summaryTFVector[posTerm],
                        _idfVector[posTerm], MyTDMParameters.TheTFIDFWeight,
                        maximumFrequency, totalWords, _averageLengthOfPhrases);
                }
            }
            else
            {
                // Calculate similarity based on average Word2Vec representation of selected phrases
                summaryTFIDFVector = new double[_documentTFIDFVector.Length];
                foreach (var pos in selectedPhrases)
                {
                    for (var term = 0; term < _documentTFIDFVector.Length; term++)
                        summaryTFIDFVector[term] += TFIDFMatrix[pos][term];
                }

                for (var term = 0; term < _documentTFIDFVector.Length; term++)
                    summaryTFIDFVector[term] /= selectedPhrases.Count;
            }

            //4. Calculate and return the similarity of the Summary to the Document
            var similarity = Calculus.CosineSimilarity(summaryTFIDFVector, _documentTFIDFVector);
            return similarity;
        }

        private static double MultiplyTFbyIDFUsingWeights(double tf, double idf,
            TFIDFWeight theTFIDFWeight, double maxTFi, int vectorLength,
            double averageLengthOfPhrases)
        {
            // Simple Weight   Wi,j = Fi * IDFi = LexRank
            // Complete Weight Wi,j = Fi / Max (Fj) * IDFi
            // BM25 Weight     Wi,j = ((k + 1) * Fi) / (k (1 - b + b (|phraseI| / AVG (Document))) + Fi) * IDFi
            // Best Weight     Wi,j = log10 (1 + Fi) * IDFi
            // where Fi is the observed frequency of term i y phrase j,
            // where N is the total number of phrases and ni is the number of phrases in which the term i appears,
            // Max (Fj) is the maximum observed frequency in phrase j.
            var tfidf = 0.0;
            switch (theTFIDFWeight)
            {
                case TFIDFWeight.Simple:
                    tfidf = tf * idf;
                    break;
                case TFIDFWeight.Complete:
                    if (maxTFi >= 1)
                        tfidf = (tf * idf) / maxTFi;
                    break;
                case TFIDFWeight.BM25:
                    var divisor = K * (1 - B + B * (vectorLength / averageLengthOfPhrases)) + tf;
                    tfidf = ((K + 1) * tf) / divisor;
                    tfidf *= idf;
                    break;
                case TFIDFWeight.Best:
                    tfidf = Math.Log10(1 + tf) * idf;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(theTFIDFWeight), theTFIDFWeight, null);
            }
            return tfidf;
        }

        private void CalculateDoc2Vec(IList<PhraseData> phraseList, string documenTitle,
            string cacheFileName, out double[][] matrixTFIDF, out double[] documentTFIDFVector, 
            out double[] titleTFIDFVector)
        {
            // If the split file does not exist ... create it.
            var cacheSplitta = cacheFileName + ".split";
            if (!File.Exists(cacheSplitta))
            {
                var phrasesByLine = new List<string>();
                var alldoc = "";
                var toRemove = new List<int>();
                for (var p = 0; p < phraseList.Count; p++)
                {
                    alldoc += phraseList[p].OriginalText + " ";
                    if (phraseList[p].Length < 3)
                    {
                        toRemove.Add(p);
                        continue;
                    }
                    phrasesByLine.Add(phraseList[p].OriginalText);
                }
                for (var i=toRemove.Count - 1; i >= 0; i--)
                    phraseList.RemoveAt(toRemove[i]);

                phrasesByLine.Add(alldoc.Trim()); // Include all document
                phrasesByLine.Add(documenTitle.Trim()); // Include the title

                File.AppendAllLines(cacheSplitta, phrasesByLine);
            }

            // If the doc2vec file does not exist ... create it.
            var cacheDoc2Vec = cacheFileName + ".doc2vec";
            if (!File.Exists(cacheDoc2Vec))
            {
                //https://www.youtube.com/watch?v=g1VWGdHRkHs for call python

                const string python = @"C:\Users\cobos\Desktop\doc2vec\venv\Scripts\python.exe";
                const string script = @"C:\Users\cobos\Desktop\doc2vec\doc2vec-splitted-documents.py";
                var inputfile = cacheSplitta;
                var outputfile = cacheDoc2Vec;
                var command = $"\"{python}\" \"{script}\" \"{inputfile}\" \"{outputfile}\"";

                var psi = new ProcessStartInfo
                {
                    FileName = command,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                };
                using (var process = Process.Start(psi))
                {
                    process?.WaitForExit();
                    //errors = process.StandardError.ReadToEnd();
                    //results = process.StandardOutput.ReadToEnd();
                }
            }

            var lines = File.ReadAllLines(cacheDoc2Vec);
            for (var i = 0; i < lines.Length; i++)
                lines[i] = lines[i].Replace(".", ",");
            var totDocs = lines.Length - 2;
            var latentTerms = lines[0].Split(' ');
            var totTerms = latentTerms.Length;

            matrixTFIDF = new double[totDocs][];
            documentTFIDFVector = new double[totTerms];
            titleTFIDFVector = new double[totTerms];

            int pos;
            for (pos = 0; pos < totDocs; pos++)
            {
                matrixTFIDF[pos] = new double[totTerms];
                var dat = lines[pos].Split(' ');
                for (var j = 0; j < totTerms; j++)
                    matrixTFIDF[pos][j] = double.Parse(dat[j]);
            }

            var dat2 = lines[pos++].Split(' ');
            for (var j = 0; j < totTerms; j++)
                documentTFIDFVector[j] = double.Parse(dat2[j]);

            dat2 = lines[pos].Split(' ');
            for (var j = 0; j < totTerms; j++)
                titleTFIDFVector[j] = double.Parse(dat2[j]);

            SortSimilaritiesToTitle();
        }
    }
}