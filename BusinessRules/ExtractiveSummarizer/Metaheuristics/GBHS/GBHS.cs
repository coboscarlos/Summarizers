using System.Collections.Generic;
using BusinessRules.VectorSpaceModel;
using BusinessRules.Utils;

namespace BusinessRules.ExtractiveSummarizer.Metaheuristics.GBHS
{
    public class GBHS: AlgorithmBase
    {
        public List<BaseSolucionGBHS> HarmonyMemory;

        public int OportunidadSA;

        public override void Summarize(SummaryParameters mySummaryParameters, string newsDirectory, string cacheFileName)
        {
            MyParameters = (GBHSParameters)mySummaryParameters;

            MyTDM = new TDM(newsDirectory, MyParameters.MyTDMParameters, cacheFileName);
            MyExternalMDS = new SimilarityMatrix(MyTDM, cacheFileName, true);
            SolutionSize = MyTDM.PhrasesList.Count;

            var phrasesList = Execute();

            TextSummary = Util.SummarizeByCompressionRatio(MyTDM, phrasesList, mySummaryParameters.MySummaryType, 
                MyParameters.MaximumLengthOfSummaryForRouge, out SummaryByPhrases);
        }

        public List<KeyValuePair<int, double>> Execute()
        {
            CurrentFFEs = 0;
            MaximumNumberOfFitnessFunctionEvaluations = MyParameters.MaximumNumberOfFitnessFunctionEvaluations;
            var myParameters = (GBHSParameters) MyParameters;

            CalculateRankingPhrasePosition();
            SortLengths();

            ObtainViablePhrasesSortedByCosineCoverage();
            HarmonyMemory = new List<BaseSolucionGBHS>();

            // Population initialization, fitness is calculated for each agent and local search is applied
            for (var i = 0; i < myParameters.HMS; i++)
            {
                var newAgent = new BaseSolucionGBHS(this);
                newAgent.InicializarAleatorio();
                HarmonyMemory.Add(newAgent);
                if (CurrentFFEs > MaximumNumberOfFitnessFunctionEvaluations)  // MaxFFEs exceeded?
                    break;
            }
            HarmonyMemoryFitnessUpdate();

            for (var i = 0; i < myParameters.HMS; i++)
                HarmonyMemory[i].Optimizar();

            HarmonyMemoryFitnessUpdate();

            // Sort the population from highest to lowest fitness ... it is maximizing
            HarmonyMemory.Sort((x,y) => -1 * x.Fitness.CompareTo(y.Fitness));

            while (CurrentFFEs < MaximumNumberOfFitnessFunctionEvaluations)
            {
                var miPAR = ParGn(myParameters.ParMin, myParameters.ParMax, CurrentFFEs, MaximumNumberOfFitnessFunctionEvaluations);
                var nuevoImproviso = new BaseSolucionGBHS(this);
                var cantidadPalabras = 0;
                var intentos = 0;

                while (cantidadPalabras < MyParameters.MaximumLengthOfSummaryForRouge)
                {
                    int pos;
                    if (MyParameters.NumeroAleatorio.NextDouble() < myParameters.HMCR)
                    {                                
                        var posEnMemoria = MyParameters.NumeroAleatorio.Next(myParameters.HMS);
                        if (MyParameters.NumeroAleatorio.NextDouble() < miPAR)
                            posEnMemoria = 0; // Choose the best harmony from harmony memory

                        var posFrase = MyParameters.NumeroAleatorio.Next(HarmonyMemory[posEnMemoria].ActivePhrases.Count);
                        pos = HarmonyMemory[posEnMemoria].ActivePhrases[posFrase];
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

                HarmonyMemoryFitnessUpdate();

                if (!HarmonyMemory.Exists(x => x.Equals(nuevoImproviso)))
                    if (nuevoImproviso.Fitness > HarmonyMemory[HarmonyMemory.Count - 1].Fitness) // New harmony is better than the worst in the Harmony Memory?
                    {
                        HarmonyMemory.RemoveAt(HarmonyMemory.Count - 1);
                        HarmonyMemory.Add(nuevoImproviso);
                        HarmonyMemory.Sort((x, y) => -1*x.Fitness.CompareTo(y.Fitness));
                    }

                if (CurrentFFEs > MaximumNumberOfFitnessFunctionEvaluations)  // MaxFFEs exceeded?
                    break;
            }

            HarmonyMemory[0].OptimizationComplete();
            var phrasesList = new List<KeyValuePair<int, double>>();
            var total = HarmonyMemory[0].ActivePhrases.Count;
            for (var i = 0; i < total ; i++)
                phrasesList.Add(new KeyValuePair<int, double>(HarmonyMemory[0].ActivePhrases[i], total-i));

            return phrasesList;
        }

        private static double ParGn(double parMin, double parMax, int numCiclo, int ni)
        {
            var parGn = parMin + ((parMax - parMin) * (numCiclo * 1.0 / ni));
            return parGn;
        }

        private void HarmonyMemoryFitnessUpdate()
        {
            if (UpdateFitness)
            {
                for (var i = 0; i < ((GBHSParameters) MyParameters).HMS; i++)
                    HarmonyMemory[i].CalculateFitness(true); // Fitness is recalculated with new ranges
                HarmonyMemory.Sort((x, y) => -1 * x.Fitness.CompareTo(y.Fitness));
                UpdateFitness = false;
            }
        }
    }
}