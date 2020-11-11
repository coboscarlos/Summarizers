using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using BusinessRules.ExtractiveSummarizer;
using BusinessRules.ExtractiveSummarizer.Graphs;
using BusinessRules.ExtractiveSummarizer.Metaheuristics.DiscreteFSP;
using BusinessRules.ExtractiveSummarizer.Metaheuristics.DiscreteSFLA;
using BusinessRules.ExtractiveSummarizer.Metaheuristics.GBHS;
using BusinessRules.Utils;

namespace Interface
{
    class TestSummarizers
    {
        public void Ejecutar(DUCDataSet midataset, SummaryParameters myParameters, 
            int idEjecution, int maxRepetitions, string theAlgorithm)
        {
            //Se establece el Identification de la ejecucion y se crea el directorio de salida
            var directorioDeSalida = midataset.RougeRootDirectory + @"experimentos\" + idEjecution.ToString("0000");
            Directory.CreateDirectory(directorioDeSalida);

            //Se define el nombre del archivo en el que se colocaran los resultados por medio de los parámetros de ejecución.
            var theDate = DateTime.Now.Year + "-" + DateTime.Now.Month.ToString("00") + "-" + DateTime.Now.Day.ToString("00") + "-" + DateTime.Now.Hour.ToString("00") + "-" + DateTime.Now.Minute.ToString("00");
            var outputFileName = theDate + "-Exp-" + maxRepetitions + "-" + myParameters + ".xlsx";
            outputFileName = outputFileName.Replace(",", ".");

            var directoryList = new List<string>();
            directoryList.AddRange(Directory.GetDirectories(midataset.RougeRootDirectory + "documents")); //Tiene la ruta de cada documento
            directoryList.Sort();

            var fileAccount = 0;

            var allFullNews = new List<string>();
            foreach (var fullDirectory in directoryList) //Recorre cada directorio de documentos y hace ...
            {
                var lasNoticiasFull = Directory.GetFiles(fullDirectory);
                allFullNews.AddRange(lasNoticiasFull);
            }

            foreach (var fullNews in allFullNews)
            {
                fileAccount++;
                var theFullNews = fullNews;
                var x = new FileInfo(theFullNews);
                var thisNews = x.Name; //Deja solo el nombre de la noticia
                Debug.WriteLine(thisNews + " " + fileAccount);

                var nombreArchivosCache = midataset.MatricesRootDirectory + thisNews + "-" +
                                            myParameters.MyTDMParameters.MinimumFrequencyThresholdOfTermsForPhrase + "-" +
                                            myParameters.MyTDMParameters.MinimumThresholdForTheAcceptanceOfThePhrase + "-" +
                                            myParameters.MyTDMParameters.TheTFIDFWeight  + "-" +
                                            myParameters.MyTDMParameters.TheDocumentRepresentation;
                nombreArchivosCache = nombreArchivosCache.Replace(",", ".");

                //Parallel.For(0, maxRepetitions, repetition =>
                for (var repetition=0; repetition < maxRepetitions; repetition++)
                {
                    //if (laNoticia != "LA042190-0060")
                    //    continue;

                    // Create the experiment folder If it does not exists
                    var directorioExperimento = directorioDeSalida + @"\" + repetition.ToString("000");
                    if (!Directory.Exists(directorioExperimento))
                        Directory.CreateDirectory(directorioExperimento);

                    directorioExperimento += @"\systems";
                    if (!Directory.Exists(directorioExperimento))
                        Directory.CreateDirectory(directorioExperimento);

                    SummarizerAlgorithm summarizer = null;
                    switch (theAlgorithm)
                    {
                        case "ContinuousLexRank":
                            summarizer = new ContinuousLexRank();
                            summarizer.Summarize(myParameters, theFullNews, nombreArchivosCache);
                            break;
                        case "DegreeCentralityLexRank":
                            summarizer = new DegreeCentralityLexRank();
                            summarizer.Summarize(myParameters, theFullNews, nombreArchivosCache);
                            break;
                        case "LexRankWithThreshold":
                            summarizer = new LexRankWithThreshold();
                            summarizer.Summarize(myParameters, theFullNews, nombreArchivosCache);
                            break;
                        case "FSP":
                            ((FSPParameters) myParameters).RandomGenerator = new Random(repetition);
                            summarizer = new FSP();
                            summarizer.Summarize(myParameters, theFullNews, nombreArchivosCache);
                            break;
                        case "GBHS":
                            ((GBHSParameters) myParameters).RandomGenerator = new Random(repetition);
                            summarizer = new GBHS();
                            summarizer.Summarize(myParameters, theFullNews, nombreArchivosCache);
                            break;
                        case "SFLA":
                            ((SFLAParameters)myParameters).RandomGenerator = new Random(repetition);
                            summarizer = new SFLA();
                            summarizer.Summarize(myParameters, theFullNews, nombreArchivosCache);
                            break;
                    }
                    if (summarizer != null)
                    {
                        var contenidoResumenFinal = summarizer.TextSummary;
                        File.WriteAllText(directorioExperimento + @"\" + thisNews, contenidoResumenFinal);
                    }
                    Debug.Write(repetition + ", ");
                } // Fin de for
                //}); // Fin de Parallel.For
                Debug.WriteLine("");
                Debug.WriteLine("THREAD :" + Thread.CurrentThread.ManagedThreadId + 
                                " NEWS " + thisNews);
            }

            Debug.WriteLine("--- EVALUATING ---");

            // Se realizan los Calculus de ROUGE para todos los experimentos de la segunda forma (segun normas exactas de DUC 2005)
            Parallel.For(0, maxRepetitions, experimento =>
            {
                var directorioExperimento = directorioDeSalida + @"\" + experimento.ToString("000");
                Rouge.EvaluateAnExperimentWithAllNewsPartA(midataset.RougeRootDirectory, directorioExperimento);
            });

            var salidaExperimentos = "Exp.\tR1R\tR1P\tR1F\tR2R\tR2P\tR2F\tRLR\tRLP\tRLF\tRSU4R\tRSU4P\tRSU4F\r\n";
            var resumenTodosExperimentos = new double[12];
            var subtotalesPorGrupoEvaluador = new SubTotalsByDataSet();
            for (var experimento = 0; experimento < maxRepetitions; experimento++)
            {
                var directorioExperimento = directorioDeSalida + @"\" + experimento.ToString("000");
                var resultadoEstaNoticia = new double[12];
                Rouge.EvaluateAnExperimentWithAllNewsPartB(midataset.RougeRootDirectory, directorioExperimento,
                    ref resultadoEstaNoticia, ref subtotalesPorGrupoEvaluador);

                salidaExperimentos += experimento.ToString("00") + "\t";
                for (var i = 0; i < 12; i++)
                {
                    salidaExperimentos += resultadoEstaNoticia[i] + "\t";
                    resumenTodosExperimentos[i] += resultadoEstaNoticia[i];
                }
                salidaExperimentos += "\r\n";
            }

            salidaExperimentos += "TOTAL\t";
            var salidaGlobal = outputFileName + "\t";
            for (var i = 0; i < 12; i++)
            {
                resumenTodosExperimentos[i] = resumenTodosExperimentos[i] / maxRepetitions;
                salidaExperimentos += resumenTodosExperimentos[i] + "\t";
                salidaGlobal += resumenTodosExperimentos[i] + "\t";
            }
            salidaExperimentos += "\r\n";
            salidaGlobal += "\r\n";
            File.AppendAllText(@"D:\SalidaGlobal.txt", salidaGlobal);

            //Thread.Sleep(2000);
            //GrabarEnExcelExperimentosPorNoticia(midataset.DirectorioRaizRouge, nombreArchivoDeSalida, "TodoExp", salidaExperimentos, true);

            //foreach (SubTotalsByDataSet.GrupoEvaluadorRow fila in subtotalesPorGrupoEvaluador.GrupoEvaluador.Rows)
            //{
            //    fila.Recall = decimal.Divide(fila.Recall, fila.Contador);
            //    fila.Precision = decimal.Divide(fila.Precision, fila.Contador);
            //    fila.Fmeasure = decimal.Divide(fila.Fmeasure, fila.Contador);
            //}

            //var listaRouges = new[] { "ROUGE-1", "ROUGE-2", "ROUGE-SU4" };
            //foreach (var elRouge in listaRouges)
            //{
            //    var salida = "Grupo\tRecall\tPrecision\tFmeasure\r\n";
            //    foreach (var dataRow in subtotalesPorGrupoEvaluador.GrupoEvaluador.Select("Rouge = '" + elRouge + "'"))
            //    {
            //        var fila = (SubTotalsByDataSet.GrupoEvaluadorRow)dataRow;
            //        salida += fila.Name + "\t" + fila.Recall + "\t" + fila.Precision + "\t" + fila.Fmeasure + "\r\n";
            //    }
            //    Thread.Sleep(2000);
            //    GrabarEnExcelExperimentosPorNoticia(midataset.DirectorioRaizRouge, nombreArchivoDeSalida, elRouge, salida, true);
            //}
        }

        public void GrabarEnExcelExperimentosPorNoticia(string directorioRaiz, string nombreArchivoExcel, string noticia, string datos, bool nuevaHoja)
        {
            // Para no tener problemas de CULTURA con el EXCEL
            datos = datos.Replace(",", ".");

            var fileName = directorioRaiz + "excel\\" + nombreArchivoExcel;
            if (!File.Exists(fileName))
                File.Copy(directorioRaiz + "excel\\Plantilla-30.xlsx", fileName);

            var miExcel = new Microsoft.Office.Interop.Excel.Application { Visible = false };
            var miLibro = miExcel.Workbooks.Open(fileName);
            if (nuevaHoja)
                miLibro.Sheets.Add(After: miLibro.Sheets.Item[miLibro.Sheets.Count]);
            else
            {
                var mipatron = (Microsoft.Office.Interop.Excel._Worksheet)miLibro.Sheets.Item[1];
                mipatron.Copy(After: miLibro.Sheets.Item[miLibro.Sheets.Count]);
            }

            var miHoja = (Microsoft.Office.Interop.Excel._Worksheet)miLibro.Sheets.Item[miLibro.Sheets.Count];
            miHoja.Name = noticia;
            var mirango = miHoja.Range["A2", "A2"];
            if (nuevaHoja)
                mirango = miHoja.Range["A1", "A1"];
            mirango.Select();

            var lineas = datos.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            var fila = 1;
            foreach (var linea in lineas)
            {
                var columna = 1;
                var valores = linea.Split(new[] { "\t" }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var valor in valores)
                {
                    ((Microsoft.Office.Interop.Excel.Range)miHoja.Cells[fila, columna]).Value = valor;
                    columna++;
                }
                fila++;
            }
            miLibro.Save();
            miExcel.Quit();
        }
    }
}