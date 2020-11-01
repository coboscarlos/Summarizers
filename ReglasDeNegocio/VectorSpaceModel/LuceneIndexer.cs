using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Analysis.Tokenattributes;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Store;
using Version = Lucene.Net.Util.Version;
using BusinessRules.Utils;

namespace BusinessRules.VectorSpaceModel
{
    public class LuceneIndexer
    {
        public List<PhraseData> PhraseList = new List<PhraseData>();
        public List<string> TermsList = new List<string>();
        public string DocumentTitle;

        public RAMDirectory InMemory(string documentName)
        {
            PhraseList.Clear();

            // Indexes the documents in memory using Lucene.NET
            var ramDirectory = new RAMDirectory();
            var mfl = new IndexWriter.MaxFieldLength(IndexWriter.DEFAULT_MAX_FIELD_LENGTH);
            var indexWriter = new IndexWriter(ramDirectory, new WhitespaceAnalyzer(), true, mfl);
            
            try
            {
                var phrasesFromFile = XMLFilePreProcessing.GetPhrases(documentName);
                DocumentTitle = TextProcessing(XMLFilePreProcessing.GetTitle(documentName));
                foreach (var phrase in phrasesFromFile)
                    DocumentIndexing(indexWriter, phrase);
            }
            catch (Exception e1)
            {
                Debug.WriteLine("ERROR LuceneIndexer.InMemory : " + e1.Message);
            }

            indexWriter.Close();

            // Define the list of terms of the document and sort alphabetically
            var indexReader = IndexReader.Open(ramDirectory, true);
            var termEnum = indexReader.Terms();
            while (termEnum.Next())
                TermsList.Add(termEnum.Term().Text());
            termEnum.Close();
            indexReader.Close();

            TermsList.Sort();

            return ramDirectory;
        }

        private void DocumentIndexing(IndexWriter indexWriter, PhraseData myPhrasesData)
        {
            try
            {
                var newPhrase = new PhraseData(myPhrasesData);
                PhraseList.Add(newPhrase);
                var document = new Document();
                document.Add(new Field("content", myPhrasesData.ProcessedText, Field.Store.YES, Field.Index.ANALYZED, Field.TermVector.YES));
                indexWriter.AddDocument(document);
            }
            catch (Exception e2)
            {
                Debug.WriteLine("ERROR LuceneIndexer.DocumentIndexing : " + e2.Message);
            }
        }

        public static string TextProcessing(string content)
        {
            // StandardTokenizer: Splits words at punctuation characters, removing punctuation. However, 
            // a dot that's not followed by whitespace is considered part of a token.
            // Splits words at hyphens, unless there's a number in the token, in which case the whole token 
            // is interpreted as a product number and is not split.
            // Recognizes email addresses and internet hostnames as one token.
            TokenStream tokenStream = new StandardTokenizer(Version.LUCENE_29, new StringReader(content));

            // StandardFilter: Normalizes tokens extracted with StandardTokenizer. 
            // Removes 's from the end of words.
            // Removes dots from acronyms.
            tokenStream = new StandardFilter(tokenStream);

            //LowerCaseFilter: Normalizes token text to lower case.
            tokenStream = new LowerCaseFilter(tokenStream);

            //StopFilter: Removes stop words from a token stream.
            tokenStream = new StopFilter(true, tokenStream, StopWordList.EnglishStopWordsSet);

            //PorterStem: Transforms the token stream as per the Porter stemming algorithm. 
            tokenStream = new PorterStemFilter(tokenStream);

            return TokenToString(tokenStream);
        }

        private static string TokenToString(TokenStream myToken)
        {
            var localToken = myToken;
            var result = "";
            while (localToken.IncrementToken())
            {
                var termAttribute = (TermAttribute)localToken.GetAttribute(typeof(TermAttribute));
                result += termAttribute.Term() + " ";
            }
            return result.Trim();
        }
    }
}