using System;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using System.Threading;
using BusinessRules.VectorSpaceModel;

namespace BusinessRules.Utils
{
    public class XMLFilePreProcessing
    {
        public static List<PhraseData> GetPhrases(string myXmlFile)
        {
            var phrases = new List<PhraseData>();
            try
            {
                var completeText = ReadNews(myXmlFile);

                var idThread = Thread.CurrentThread.ManagedThreadId;
                var fileNameOriginal = "Text" + idThread + ".txt";
                var fileNamePhrases = "Phrases" + idThread + ".txt";
                var fileNameCommand = "Split" + idThread + ".bat";

                if (File.Exists(fileNameOriginal)) File.Delete(fileNameOriginal);
                if (File.Exists(fileNamePhrases)) File.Delete(fileNamePhrases);
                if (File.Exists(fileNameCommand)) File.Delete(fileNameCommand);

                File.AppendAllText(fileNameOriginal, completeText);
                var command = @"c:\python27\python splitta\sbd.py -m splitta\model_nb " + fileNameOriginal + " > " + fileNamePhrases;
                File.AppendAllText(fileNameCommand, command);

                var infoProcess = new ProcessStartInfo(fileNameCommand) { WindowStyle = ProcessWindowStyle.Hidden };
                var myProcess = new Process { StartInfo = infoProcess };
                myProcess.Start();
                myProcess.WaitForExit();

                var lines = File.ReadAllLines(fileNamePhrases);
                var position = 1;

                foreach (var line in lines)
                {
                    if (line.Length == 0) continue;
                    var processed = LuceneIndexer.TextProcessing(line);
                    if (processed.Length == 0) continue;
                    var newPhrase = new PhraseData(line, processed, position);
                    position++;
                    phrases.Add(newPhrase);
                }

                if (File.Exists(fileNameOriginal)) File.Delete(fileNameOriginal);
                if (File.Exists(fileNamePhrases)) File.Delete(fileNamePhrases);
                if (File.Exists(fileNameCommand)) File.Delete(fileNameCommand);
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR XMLFilePreProcessing.GetPhrases : " + e.Message);
            }
            return phrases;
        }

        /// <summary>
        /// this function allows you to read the DUC file and pass the read text to a txt file
        /// called text.txt saved in the execution path of the program
        /// </summary>
        /// <param name="myDocumentPath">Path where is the file to read</param>
        public static string ReadNews(string myDocumentPath)
        {
            var myContent = ValidateXmlTextInFile(myDocumentPath);
            myContent = myContent.ToLower();
            var start = myContent.IndexOf("<text>");
            var end = myContent.IndexOf("</text>");
            var myText = myContent.Substring(start + 6, end - start - 6);

            return myText;
        }
        
        public static string ValidateXmlTextInFile(string myDocumentFile)
        {
            var myContent = File.ReadAllText(myDocumentFile);
            myContent = HtmlToText.Preprocessing(myContent);
            return myContent;
        }

        public static string GetTitle(string myDocumentPath)
        {
            var myContent = File.ReadAllText(myDocumentPath);
            myContent = myContent.ToLower();
            var start = myContent.IndexOf("<headline>");
            var end = myContent.IndexOf("</headline>");
            var myTitle = myContent.Substring(start + 10, end - start - 10);

            return myTitle;
        }      
   }
}