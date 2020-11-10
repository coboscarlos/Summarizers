using System;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using System.Threading;
using System.Xml;
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
            var myText = "";
            try
            {
                var myContent = ValidateXmlTextInFile(myDocumentPath);
                var doc = new XmlDocument();
                doc.LoadXml(myContent);

                var s = "";
                foreach (XmlElement node in doc.GetElementsByTagName("TEXT"))
                    s += node.InnerText.Trim() + " ";
                myText = s.Trim();

                myText = myText.Replace("&amp;", "&");
                myText = myText.Replace("&#39;", "'");
                myText = myText.Replace("``", "''");
                myText = myText.Replace(".''\n", ".''.\n");
                myText = myText.Replace(".'' ", ".''. \n");
                myText = myText.Replace(".' ", ".'. \n");
                myText = myText.Replace(".;", ".\n");
                myText = myText.Replace("!;", "!\n");
                myText = myText.Replace("?;", "?\n");
                myText = myText.Replace("w+{ ", "w+ {");
                myText = myText.Replace("      ", " ");
                myText = myText.Replace("     ", " ");
                myText = myText.Replace("    ", " ");
                myText = myText.Replace("   ", " ");
                myText = myText.Replace("  ", " ");
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR XMLFilePreProcessing.ReadNews : " + myDocumentPath + " -->" + e.Message);
            }
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
            var myTitle = "";
            try
            {
                var xmlDoc = new XmlDocument();
                xmlDoc.Load(myDocumentPath);

                var nodosHead = xmlDoc.GetElementsByTagName("HEAD");
                var nodosHeadLine = xmlDoc.GetElementsByTagName("HEADLINE");
                var nodosHL = xmlDoc.GetElementsByTagName("HL");
                var nodosH3 = xmlDoc.GetElementsByTagName("H3");


                if (nodosHead.Count > 0)
                {
                    foreach (var nodo in nodosHead)
                    {
                        myTitle += (((XmlElement) nodo).InnerText).Replace("\r\n", " ");
                        myTitle += " ";
                    }
                }
                else if (nodosHeadLine.Count > 0)
                {
                    foreach (var nodo in nodosHeadLine)
                    {
                        var elementosP = ((XmlElement) nodo).GetElementsByTagName("P");

                        if (elementosP.Count > 0)
                        {
                            foreach (var p in elementosP)
                            {
                                myTitle += (((XmlElement) p).InnerText).Replace("\r\n", " ");
                                myTitle += " ";
                            }
                        }
                        else
                        {
                            myTitle += (((XmlElement) nodo).InnerText).Replace("\r\n", " ");
                            myTitle += " ";
                        }
                    }
                }
                else if (nodosHL.Count > 0)
                {
                    foreach (var nodo in nodosHL)
                    {
                        myTitle += (((XmlElement) nodo).InnerText).Replace("@", "");
                        myTitle += (((XmlElement) nodo).InnerText).Replace("\r\n", " ");
                        myTitle += " ";
                    }
                }
                else if (nodosH3.Count > 0)
                {
                    foreach (var nodo in nodosH3)
                    {
                        var elementosTi = ((XmlElement) nodo).GetElementsByTagName("TI");

                        foreach (var ti in elementosTi)
                        {
                            myTitle += (((XmlElement) ti).InnerText).Replace("\r\n", " ");
                            myTitle += " ";
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR XMLFilePreProcessing.GetTitle : " + myDocumentPath + " -->" + e.Message);
            }
            return myTitle;
        }      
   }
}