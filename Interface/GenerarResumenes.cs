using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using BusinessRules.ExtractiveSummarizer;
using BusinessRules.ExtractiveSummarizer.Graphs;
using BusinessRules.ExtractiveSummarizer.Metaheuristics;
using BusinessRules.ExtractiveSummarizer.Metaheuristics.FSP;
using BusinessRules.ExtractiveSummarizer.Metaheuristics.GBHS;
using BusinessRules.Utils;

namespace Interface
{
    class GenerarResumenes
    {
        public void Ejecutar(DUCDataSet midataset, SummaryParameters misParametros, int idEjecucion, int totalEjecuciones, string algoritmo)
        {
            var inicio = DateTime.Now;

            //Se establece el identificador de la ejecucion y se crea el directorio de salida
            var directorioDeSalida = midataset.RougeRootDirectory + @"experimentos\" + idEjecucion.ToString("0000");
            Directory.CreateDirectory(directorioDeSalida);

            //Se define el nombre del archivo en el que se colocaran los resultados por medio de los parámetros de ejecución.
            var fecha = DateTime.Now.Year + "-" + DateTime.Now.Month.ToString("00") + "-" + DateTime.Now.Day.ToString("00") + "-" + DateTime.Now.Hour.ToString("00") + "-" + DateTime.Now.Minute.ToString("00");
            var nombreArchivoDeSalida = fecha + "-Exp-" + totalEjecuciones + "-" + misParametros + ".xlsx";
            nombreArchivoDeSalida = nombreArchivoDeSalida.Replace(",", ".");

            var listaDeDirectorios = new List<string>();
            listaDeDirectorios.AddRange(Directory.GetDirectories(midataset.RougeRootDirectory + "documents")); //Tiene la ruta de cada documento
            listaDeDirectorios.Sort();

            var contadorarchivos = 0;
            var exitosRemplazo = 0;
            var fracasosRemplazo = 0;
            var exitosOptimizacion = 0;
            var fracasosOptimizacion = 0;

            var todasLasNoticiasFull = new List<string>();
            foreach (var elDirectorioFull in listaDeDirectorios) //Recorre cada directorio de documentos y hace ...
            {
                var lasNoticiasFull = Directory.GetFiles(elDirectorioFull);
                todasLasNoticiasFull.AddRange(lasNoticiasFull);
            }

            foreach (var laNoticiaCompleta in todasLasNoticiasFull)
            {
                contadorarchivos++;
                var laNoticiaFull = laNoticiaCompleta;
                var x = new FileInfo(laNoticiaFull);
                var laNoticia = x.Name; //Deja solo el nombre de la noticia
                Debug.WriteLine(laNoticia + " " + contadorarchivos);

                //var noticia = int.Parse(Path.GetFileName(laNoticiaCompleta));
                //if (noticia > 6493) continue;

                if (midataset.Name == "CnnDm")
                {
                    var ds = new DataSet();
                    ds.ReadXml(midataset.RougeRootDirectory + @"DS-limites.xml", XmlReadMode.ReadSchema);
                    var readedrows = ds.Tables[0].Select("News = '" + laNoticia + "'");
                    var maxSentences = (int) readedrows[0][2];
                    misParametros.MySummaryType = SummaryType.Sentences;
                    misParametros.MaximumLengthOfSummaryForRouge = maxSentences;
                    var maxSentencesEvol = maxSentences;
                    if (algoritmo == "GBHS" || algoritmo == "DiscreteFSP")
                        ((BaseParameters)misParametros).MaximumSummaryLengthToEvolve = maxSentencesEvol;
                }

                var nombreArchivosCache = midataset.MatricesRootDirectory + laNoticia + "-" +
                                            misParametros.MyTDMParameters.MinimumFrequencyThresholdOfTermsForPhrase + "-" +
                                            misParametros.MyTDMParameters.MinimumThresholdForTheAcceptanceOfThePhrase + "-" +
                                            misParametros.MyTDMParameters.TheTFIDFWeight  + "-" +
                                            misParametros.MyTDMParameters.TheDocumentRepresentation;
                nombreArchivosCache = nombreArchivosCache.Replace(",", ".");

                //Parallel.For(0, totalEjecuciones, ejecucion =>
                for (var ejecucion=0; ejecucion < totalEjecuciones; ejecucion++)
                {
                    // Create the experiment folder If it does not exists
                    var directorioExperimento = directorioDeSalida + @"\" + ejecucion.ToString("000");
                    if (!Directory.Exists(directorioExperimento))
                        Directory.CreateDirectory(directorioExperimento);

                    directorioExperimento += @"\systems";
                    if (!Directory.Exists(directorioExperimento))
                        Directory.CreateDirectory(directorioExperimento);

                    SummarizerAlgorithm sumarizador = null;
                    switch (algoritmo)
                    {
                        case "ContinuousLexRank":
                            sumarizador = new ContinuousLexRank();
                            sumarizador.Summarize(misParametros, laNoticiaFull, nombreArchivosCache);
                            break;
                        case "DegreeCentralityLexRank":
                            sumarizador = new DegreeCentralityLexRank();
                            sumarizador.Summarize(misParametros, laNoticiaFull, nombreArchivosCache);
                            break;
                        case "LexRankWithThreshold":
                            sumarizador = new LexRankWithThreshold();
                            sumarizador.Summarize(misParametros, laNoticiaFull, nombreArchivosCache);
                            break;
                        case "DiscreteFSP":
                            ((DiscreteFSPParameters) misParametros).NumeroAleatorio = new Random(ejecucion);
                            sumarizador = new DiscreteFSP();
                            sumarizador.Summarize(misParametros, laNoticiaFull, nombreArchivosCache);
                            break;
                        case "GBHS":
                            ((GBHSParameters) misParametros).NumeroAleatorio = new Random(ejecucion);
                            sumarizador = new GBHS();
                            sumarizador.Summarize(misParametros, laNoticiaFull, nombreArchivosCache);
                            break;
                    }
                    if (sumarizador != null)
                    {
                        var contenidoResumenFinal = sumarizador.TextSummary;
                        File.WriteAllText(directorioExperimento + @"\" + laNoticia, contenidoResumenFinal);
                        exitosOptimizacion += sumarizador.SuccessInOptimization;
                        fracasosOptimizacion += sumarizador.OptimizationFailures;
                        exitosRemplazo += sumarizador.SuccessInReplacement;
                        fracasosRemplazo += sumarizador.ReplacementFailures;
                    }

                }; // Fin de for
                //}); // Fin de Parallel.For

                Debug.WriteLine("HILO :" + Thread.CurrentThread.ManagedThreadId + " Termino " + laNoticia);

                /*if (contadorarchivos % 10 == 0 || contadorarchivos>= todasLasNoticiasFull.Count)
                {
                    var diff = (DateTime.Now - inicio).TotalMinutes;
                    Debug.WriteLine("HILO :" + Thread.CurrentThread.ManagedThreadId + " " + diff.ToString("#0.00") + " minutos con " +
                                        (contadorarchivos * 100.0 / todasLasNoticiasFull.Count).ToString("#0.00") + "% Directorios " +
                                        ((100 * exitosRemplazo) / (exitosRemplazo + fracasosRemplazo)).ToString("#0.00") + "% " +
                                        ((100 * exitosOptimizacion) / (exitosOptimizacion + fracasosOptimizacion)).ToString("#0.00") + "%");
                }
                */
            }

            Debug.WriteLine("EMPIEZA A EVALUAR");

            // Se realizan los Calculus de ROUGE para todos los experimentos de la segunda forma (segun normas exactas de DUC 2005)
            Parallel.For(0, totalEjecuciones, experimento =>
            {
                var directorioExperimento = directorioDeSalida + @"\" + experimento.ToString("000");
                Rouge.EvaluateAnExperimentWithAllNewsPartA(midataset.RougeRootDirectory, directorioExperimento);
            });

            var salidaExperimentos = "Exp.\tR1R\tR1P\tR1F\tR2R\tR2P\tR2F\tRLR\tRLP\tRLF\tRSU4R\tRSU4P\tRSU4F\r\n";
            var resumenTodosExperimentos = new double[12];
            var subtotalesPorGrupoEvaluador = new SubTotalsByDataSet();
            for (var experimento = 0; experimento < totalEjecuciones; experimento++)
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
            var salidaGlobal = nombreArchivoDeSalida + "\t";
            for (var i = 0; i < 12; i++)
            {
                resumenTodosExperimentos[i] = resumenTodosExperimentos[i] / totalEjecuciones;
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