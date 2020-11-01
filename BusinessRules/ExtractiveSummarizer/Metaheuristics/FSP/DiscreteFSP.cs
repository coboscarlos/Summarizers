using System.Collections.Generic;
using BusinessRules.VectorSpaceModel;
using BusinessRules.Utils;
using System.Linq;

namespace BusinessRules.ExtractiveSummarizer.Metaheuristics.FSP
{
    public class DiscreteFSP: AlgorithmBase
    {
        public FSPSolution Mejor;        
        public List<FSPSolution> PuntosDeCaptura;
        public List<FSPSolution> Pi;//Memoria local de la mejor solucion encontrada en cada vecindad de el punto de captura

        public override void Summarize(SummaryParameters mySummaryParameters, string newsDirectory, string cacheFileName)
        {
            MyParameters = (FSPDiscreto)mySummaryParameters;

            //Debug.Write("Iniciando ejecución FSP ");
            //var inicio = DateTime.Now;

            MyTDM = new TDM(newsDirectory, MyParameters.MyTDMParameters, cacheFileName);
            MyExternalMDS = new SimilarityMatrix(MyTDM, cacheFileName, true);
            SolutionSize = MyTDM.PhrasesList.Count;

            var listaDeFrases = Ejecutar();

            TextSummary = Util.SummarizeByCompressionRatio(MyTDM, listaDeFrases, mySummaryParameters.MySummaryType, 
                MyParameters.MaximumLengthOfSummaryForRouge, out SummaryByPhrases);            

            //var fin = DateTime.Now - inicio;
            //Debug.Write("FIN - Tiempo FSP " + fin.TotalSeconds);
        }
        
        public override string ToString()
        {
            return PuntosDeCaptura.Aggregate("", (current, sol) => current + sol);
        }

        public List<KeyValuePair<int, double>> Ejecutar()
        {
            // Definir el valor de C como una Heuristica aceptable 
            // Al principio debe ser suficientemene grande para explorar el espacio = 2 % rango de busqueda al princio
            // Al final debe ser suficientemete pequeño para explotar el vecinadrio = 0.000001 al final
            //var rangoBusqueda = MiFuncion.LimiteSuperiorDimension(0) - MiFuncion.LimiteInferiorDimension(0);
            //C = 0.02*rangoBusqueda;
            //var factorDecrecimiento = Math.Pow(0.000001 / C, 1.0/ MaximoNumeroGeneraciones);
            CurrentFFEs = 0;
            MaximumNumberOfFitnessFunctionEvaluations = MyParameters.MaximumNumberOfFitnessFunctionEvaluations;
            var misParametros = (FSPDiscreto) MyParameters;

            CalcularRankingPosicionFrases();
            ObtenerFrasesViablesOrdenadasPorCoberturaCoseno();
            PuntosDeCaptura = new List<FSPSolution>();
            Pi = new List<FSPSolution>();
            Mejor = new FSPSolution(this);            

            // Inicializa la poblacion, le calcula fitness a cada agente y le aplica búsqueda local

            for (var i = 0; i < misParametros.N; i++)
            {
                FSPSolution puntoCapturaNuevo;
                do
                {
                    puntoCapturaNuevo = new FSPSolution(this);
                    puntoCapturaNuevo.InicializarAleatorio();                    
                } while (PuntosDeCaptura.Exists(x => x.Equals(puntoCapturaNuevo)));                
                PuntosDeCaptura.Add(puntoCapturaNuevo);
                Pi.Add(puntoCapturaNuevo);
            }
            PuntosDeCaptura.Sort((x, y) => -1 * x.Fitness.CompareTo(y.Fitness));
            Pi.Sort((x, y) => -1 * x.Fitness.CompareTo(y.Fitness));
            Mejor = new FSPSolution(PuntosDeCaptura[0]);
            /*Debug.WriteLine("Soluciones Iniciales");   
            foreach (var item in PuntosDeCaptura)
               Debug.WriteLine(item);*/
            
            var c = misParametros.C;
            for (var generacionActual = 0; generacionActual < misParametros.T; generacionActual++)
            {
                for (var i = 0; i < misParametros.N; i++)
                {
                    // Cada pescador en su punto de captura tira la red L veces
                    var auxL = misParametros.L;
                    while ((auxL > 0))
                    {
                        var mejorsolucionLocal = PuntosDeCaptura[i].ThrowFishingNet(c); // Tira la red, actualiza pi y gbest si es el caso
                        if (mejorsolucionLocal.Fitness > Pi[i].Fitness)
                            Pi[i] = new FSPSolution(mejorsolucionLocal);
                        auxL--;
                        if (CurrentFFEs >= MaximumNumberOfFitnessFunctionEvaluations) break;
                    }
                    if (Pi[i].Fitness > PuntosDeCaptura[i].Fitness)
                        PuntosDeCaptura[i] = new FSPSolution(Pi[i]);

                    if (PuntosDeCaptura[i].Fitness > Mejor.Fitness)
                        Mejor = new FSPSolution(PuntosDeCaptura[i]);

                    if (CurrentFFEs >= MaximumNumberOfFitnessFunctionEvaluations) break;
                }

                PuntosDeCaptura.Sort((x, y) => -1 * x.Fitness.CompareTo(y.Fitness));
                Pi.Sort((x, y) => -1 * x.Fitness.CompareTo(y.Fitness));
                if (PuntosDeCaptura[0].Fitness > Mejor.Fitness)
                    Mejor = new FSPSolution(PuntosDeCaptura[0]);

                //c = c * factorDecrecimiento;               

                //if (generacionActual % 20 == 0)
                //    Debug.WriteLine(generacionActual + ": " + Mejor.Fitness + " EFOs = " + EFOs);

                if (CurrentFFEs >= MaximumNumberOfFitnessFunctionEvaluations) break;
            }

            /*Debug.WriteLine("Soluciones Finales");
            foreach (var item in PuntosDeCaptura)
                Debug.WriteLine(item);
            Debug.WriteLine("Best solucion" + Mejor);*/

            var listaFrases = SeleccionarFrasesResumenFinal(Mejor.ActivePhrases); 
            return listaFrases;
        }
    }
}