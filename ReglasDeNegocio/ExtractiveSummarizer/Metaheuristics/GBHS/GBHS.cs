using System.Collections.Generic;
using BusinessRules.VectorSpaceModel;
using BusinessRules.Utils;

namespace BusinessRules.ExtractiveSummarizer.Metaheuristics.GBHS
{
    public class GBHS: AlgorithmBase
    {
        public List<BaseSolucionGBHS> MemoriaArmonica;

        public int OportunidadSA;

        public override void Summarize(SummaryParameters mySummaryParameters, string newsDirectory, string cacheFileName)
        {
            
            MyParameters = (GBHSParameters)mySummaryParameters;

            //Debug.WriteLine("Iniciando ejecución de Memetico ");
            //var inicio = DateTime.Now;

            MyTDM = new TDM(newsDirectory, MyParameters.MyTDMParameters, cacheFileName);
            MyExternalMDS = new SimilarityMatrix(MyTDM, cacheFileName, true);
            SolutionSize = MyTDM.PhrasesList.Count;

            var listaDeFrases = Ejecutar();

            TextSummary = Util.SummarizeByCompressionRatio(MyTDM, listaDeFrases, mySummaryParameters.MySummaryType, 
                MyParameters.MaximumLengthOfSummaryForRouge, out SummaryByPhrases);

            //var total = SuccessInOptimization + OportunidadSA + OptimizationFailures;
            //Debug.WriteLine("Exitos Optimizacion  : " + SuccessInOptimization + " " + (SuccessInOptimization * 100.0 / total).ToString("##.0") + "% " + 
            //                "Oportunidades SA : " + OportunidadSA + " " + (OportunidadSA *100.0 /total).ToString("##.0") + "% " + 
            //                "Fracasos :" + OptimizationFailures + " " + (OptimizationFailures *100.0/total).ToString("##.0") + "%");

            //var fin = DateTime.Now - inicio;
            //Debug.WriteLine("Segundos de Memetico " + fin.TotalSeconds);
        }

        public List<KeyValuePair<int, double>> Ejecutar()
        {
            CurrentFFEs = 0;
            SuccessInOptimization = 0;
            SuccessInReplacement = 0;
            OptimizationFailures = 0;
            ReplacementFailures = 0;

            MaximumNumberOfFitnessFunctionEvaluations = MyParameters.MaximumNumberOfFitnessFunctionEvaluations;
            var misParametros = (GBHSParameters) MyParameters;

            CalcularRankingPosicionFrases();
            OrdenarLongitudes();

            ObtenerFrasesViablesOrdenadasPorCoberturaCoseno();
            MemoriaArmonica = new List<BaseSolucionGBHS>();

            // Inicializa la poblacion, le calcula fitness a cada agente y le aplica búsqueda local
            for (var i = 0; i < misParametros.HMS; i++)
            {
                var agenteNuevo = new BaseSolucionGBHS(this);
                agenteNuevo.InicializarAleatorio();
                MemoriaArmonica.Add(agenteNuevo);
                if (CurrentFFEs > MaximumNumberOfFitnessFunctionEvaluations)  // Ya no se pueden generar 
                    break;
            }
            ActualizarFitnessMemoria();

            for (var i = 0; i < misParametros.HMS; i++)
                MemoriaArmonica[i].Optimizar();

            ActualizarFitnessMemoria();

            // Ordena la poblacion de mayor a menor por fitness ... se esta Maximizando
            MemoriaArmonica.Sort((x,y) => -1 * x.Fitness.CompareTo(y.Fitness));

            while (CurrentFFEs < MaximumNumberOfFitnessFunctionEvaluations)
            {
                var miPAR = ParGn(misParametros.ParMin, misParametros.ParMax, CurrentFFEs, MaximumNumberOfFitnessFunctionEvaluations);
                BaseSolucionGBHS nuevoImproviso = new BaseSolucionGBHS(this);
                var cantidadPalabras = 0;
                var intentos = 0;
                while (cantidadPalabras < MyParameters.MaximumLengthOfSummaryForRouge)
                {
                    int pos;
                    if (MyParameters.NumeroAleatorio.NextDouble() < misParametros.HMCR)
                    {                                
                        var posEnMemoria = MyParameters.NumeroAleatorio.Next(misParametros.HMS);
                        if (MyParameters.NumeroAleatorio.NextDouble() < miPAR)
                            posEnMemoria = 0; // Esoge el mejor ... PSO

                        var posFrase = MyParameters.NumeroAleatorio.Next(MemoriaArmonica[posEnMemoria].ActivePhrases.Count);
                        pos = MemoriaArmonica[posEnMemoria].ActivePhrases[posFrase];
                    }
                    else
                    {
                        pos = MyParameters.NumeroAleatorio.Next(SolutionSize);
                    }


                    intentos++;
                    if (intentos >= SolutionSize) break;
                    if (nuevoImproviso.ActivePhrases.Contains(pos)) continue;

                    nuevoImproviso.ActivePhrases.Add(pos);
                    cantidadPalabras += MyTDM.PhrasesList[pos].Length;
                }
                var frasesSeleccionadas = new List<PositionValue>();
                foreach (var posFrase in nuevoImproviso.ActivePhrases)
                    frasesSeleccionadas.Add(new PositionValue(posFrase, MyTDM.PhrasesList[posFrase].SimilarityToDocument));
                frasesSeleccionadas.Sort((x, y) => -1 * x.Value.CompareTo(y.Value));

                nuevoImproviso.IncluirFrasesCumpliendoRestriccion(frasesSeleccionadas);
                nuevoImproviso.AgregarFrasesValidas();

                nuevoImproviso.ActivePhrases.Sort((x,y)=> x.CompareTo(y));
                nuevoImproviso.CalculateFitness();
                nuevoImproviso.Optimizar();

                ActualizarFitnessMemoria();

                if (!MemoriaArmonica.Exists(x => x.Equals(nuevoImproviso)))
                    if (nuevoImproviso.Fitness > MemoriaArmonica[MemoriaArmonica.Count - 1].Fitness) // nuevo es mejor
                    {
                        MemoriaArmonica.RemoveAt(MemoriaArmonica.Count - 1);
                        MemoriaArmonica.Add(nuevoImproviso);
                        SuccessInReplacement++;
                        MemoriaArmonica.Sort((x, y) => -1*x.Fitness.CompareTo(y.Fitness));
                    }
                    else
                        ReplacementFailures++;

                if (CurrentFFEs > MaximumNumberOfFitnessFunctionEvaluations)  // Ya no se pueden generar 
                    break;
            }

            MemoriaArmonica[0].OptimizationComplete();
            var listaFrases = new List<KeyValuePair<int, double>>();
            var total = MemoriaArmonica[0].ActivePhrases.Count;
            for (var i = 0; i < total ; i++)
                listaFrases.Add(new KeyValuePair<int, double>(MemoriaArmonica[0].ActivePhrases[i], total-i));

            return listaFrases;
        }

        private static double ParGn(double parMin, double parMax, int numCiclo, int ni)
        {
            var parGn = parMin + ((parMax - parMin) * (numCiclo * 1.0 / ni));
            return parGn;
        }

        private void ActualizarFitnessMemoria()
        {
            if (UpdateFitness)
            {
                for (var i = 0; i < ((GBHSParameters) MyParameters).HMS; i++)
                    MemoriaArmonica[i].CalculateFitness(true); // Se recalcula el fitness con nuevos rangos
                MemoriaArmonica.Sort((x, y) => -1 * x.Fitness.CompareTo(y.Fitness));
                UpdateFitness = false;
            }
        }
    }
}