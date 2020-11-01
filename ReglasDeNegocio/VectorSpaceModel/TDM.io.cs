using System.IO;
using System.Collections.Generic;
using BusinessRules.Utils;
using System.Text;

namespace BusinessRules.VectorSpaceModel
{
    public partial class TDM
    {
        private void ReadFromFile(string cacheFileName)
        {
            var lines = File.ReadAllLines(cacheFileName);
            var totDocs = int.Parse(lines[0]);
            var totTerms = int.Parse(lines[1]);
            _averageLengthOfPhrases = double.Parse(lines[2]);
            var posLine = 3;

            TFIDFMatrix = new double[totDocs][];
            for (var i = 0; i < totDocs; i++)
            {
                TFIDFMatrix[i] = new double[totTerms];
                var dat = lines[posLine++].Split('\t');
                for (var j = 0; j < totTerms; j++)
                    TFIDFMatrix[i][j] = double.Parse(dat[j]);
            }

            _idfVector = new double[totTerms];
            if (MyTDMParameters.TheTFIDFWeight != TFIDFWeight.Doc2Vec)
            {
                for (var i = 0; i < totTerms; i++)
                    _idfVector[i] = double.Parse(lines[posLine++]);
            }
            else
            {
                for (var i = 0; i < totTerms; i++)
                    _idfVector[i] = 1.0;
            }

            _documentTFIDFVector = new double[totTerms];
            for (var i = 0; i < totTerms; i++)
                _documentTFIDFVector[i] = double.Parse(lines[posLine++]);

            _titleTFIDFVector = new double[totTerms];
            for (var i = 0; i < totTerms; i++)
                _titleTFIDFVector[i] = double.Parse(lines[posLine++]);

            _termsTable.Clear();
            _invertedTermsTable.Clear();
            if (MyTDMParameters.TheTFIDFWeight != TFIDFWeight.Doc2Vec)
            {
                for (var i = 0; i < totTerms; i++)
                {
                    var dat = lines[posLine++].Split('\t');
                    _termsTable.Add(dat[0], int.Parse(dat[1]));
                }

                for (var i = 0; i < totTerms; i++)
                {
                    var dat = lines[posLine++].Split('\t');
                    _invertedTermsTable.Add(int.Parse(dat[0]), dat[1]);
                }
            }

            PhrasesList.Clear();
            for (var i = 0; i < totDocs; i++)
            {
                var newPhraseData = new PhraseData(lines[posLine++],
                    lines[posLine++], int.Parse(lines[posLine++]))
                {
                    SimilarityToDocument = double.Parse(lines[posLine++]),
                    SimilarityToTitle = double.Parse(lines[posLine++])
                };
                PhrasesList.Add(newPhraseData);
            }

            SimilaritiesOrderedToTheTitle = new List<double>();
            for (var i = 0; i < totDocs; i++)
            {
                var value = double.Parse(lines[posLine++]);
                SimilaritiesOrderedToTheTitle.Add(value);
            }
        }

        private void SaveToFile(string fileName)
        {
            if (File.Exists(fileName))
                File.Delete(fileName);

            var result = new StringBuilder();

            var totDocs = TFIDFMatrix.GetUpperBound(0) + 1;
            result.Append(totDocs + Characters.LineBreak);

            var totalTerms = TFIDFMatrix[0].GetUpperBound(0) + 1;
            result.Append(totalTerms + Characters.LineBreak);

            result.Append(_averageLengthOfPhrases.ToString("G17") + Characters.LineBreak);

            for (var i = 0; i < totDocs; i++)
            {
                for (var j = 0; j < totalTerms; j++)
                    result.Append(TFIDFMatrix[i][j].ToString("G17") + Characters.Tab);
                result.Append(Characters.LineBreak);
            }

            File.AppendAllText(fileName, result.ToString());
            result.Clear();

            if (MyTDMParameters.TheTFIDFWeight != TFIDFWeight.Doc2Vec)
                for (var i = 0; i < totalTerms; i++)
                    result.Append(_idfVector[i].ToString("G17") + Characters.LineBreak);

            for (var i = 0; i < totalTerms; i++)
                result.Append(_documentTFIDFVector[i].ToString("G17") + Characters.LineBreak);

            for (var i = 0; i < totalTerms; i++)
                result.Append(_titleTFIDFVector[i].ToString("G17") + Characters.LineBreak);

            if (MyTDMParameters.TheTFIDFWeight != TFIDFWeight.Doc2Vec)
            {
                foreach (var pair in _termsTable)
                    result.Append(pair.Key + Characters.Tab + pair.Value + Characters.LineBreak);

                foreach (var pair in _invertedTermsTable)
                    result.Append(pair.Key + Characters.Tab + pair.Value + Characters.LineBreak);
            }

            File.AppendAllText(fileName, result.ToString());
            result.Clear();

            for (var i = 0; i < totDocs; i++)
            {
                result.Append(PhrasesList[i].OriginalText + Characters.LineBreak);
                result.Append(PhrasesList[i].ProcessedText + Characters.LineBreak);
                result.Append(PhrasesList[i].PositionInDocument + Characters.LineBreak);
                result.Append(PhrasesList[i].SimilarityToDocument.ToString("G17") + Characters.LineBreak);
                result.Append(PhrasesList[i].SimilarityToTitle.ToString("G17") + Characters.LineBreak);
            }
            File.AppendAllText(fileName, result.ToString());

            result.Clear();

            foreach (var similarity in SimilaritiesOrderedToTheTitle)
                result.Append(similarity.ToString("G17") + Characters.LineBreak);
            File.AppendAllText(fileName, result.ToString());
        }
    }
}