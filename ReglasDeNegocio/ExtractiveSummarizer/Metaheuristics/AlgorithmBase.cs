using System;
using System.Collections.Generic;
using BusinessRules.VectorSpaceModel;
using BusinessRules.Utils;

namespace BusinessRules.ExtractiveSummarizer.Metaheuristics
{
    public abstract class AlgorithmBase : SummarizerAlgorithm
    {
        public BaseParameters MyParameters;

        public int CurrentFFEs;
        public int MaximumNumberOfFitnessFunctionEvaluations;
        public TDM MyTDM;
        public SimilarityMatrix MyExternalMDS;
        public int SolutionSize;

        public List<PositionValue> ViablePhrases;
        
        public double[] PhrasePositionRanking;

        public List<int> OrderedLengths;

        public double MaxCoverage = 0.0;
        public double MinCoverage = 1.0;

        public double MaxCohesion = 0.0;
        public double MinCohesion = 1.0;

        public bool UpdateFitness;

        protected void ObtenerFrasesViablesOrdenadasPorCoberturaCoseno()
        {
            ViablePhrases = new List<PositionValue>();
            for (var gen = 0; gen < SolutionSize; gen++)
            {
                ViablePhrases.Add(new PositionValue(gen,
                    MyTDM.PhrasesList[gen].SimilarityToDocument));
            }
            ViablePhrases.Sort((x, y) => -1 * x.Value.CompareTo(y.Value));
        }

        protected List<KeyValuePair<int, double>> SeleccionarFrasesResumenFinal(List<int> frasesActivas)
        {
            var listaFrases = new List<KeyValuePair<int, double>>();

            switch (MyParameters.TheFitnessFunction)
            {
                case FitnessFunction.MCMR:
                    foreach (var frase in frasesActivas)
                    {
                        var nuevo = new KeyValuePair<int, double>(frase, 0);
                        listaFrases.Add(nuevo);
                    }
                    break;

                case FitnessFunction.CRP:
                    foreach (var frase in frasesActivas)
                    {
                        var nuevo = new KeyValuePair<int, double>(frase, MyTDM.PhrasesList[frase].SimilarityToDocument);
                        listaFrases.Add(nuevo);
                    }
                    listaFrases.Sort((x, y) => -1 * x.Value.CompareTo(y.Value));
                    break;

                case FitnessFunction.MASDS:
                    var tamano = frasesActivas.Count;
                    var longitudPromedio = 0.0;
                    for (var i = 0; i < tamano; i++)
                        longitudPromedio += MyTDM.PhrasesList[frasesActivas[i]].Length;
                    longitudPromedio = longitudPromedio / tamano;

                    var longitudDesviacion = 0.0;
                    for (var i = 0; i < tamano; i++)
                        longitudDesviacion += Math.Pow((MyTDM.PhrasesList[frasesActivas[i]].Length - longitudPromedio), 2);
                    longitudDesviacion = Math.Sqrt(longitudDesviacion / tamano);

                    foreach (var frase in frasesActivas)
                    {
                        var longitud = MyTDM.PhrasesList[frase].Length;
                        var complemento = Math.Exp((-longitud - longitudPromedio) / longitudDesviacion);

                        var cs = 0.0;
                        foreach (var otrafrase in frasesActivas)
                            if (otrafrase != frase)
                                cs += MyExternalMDS.GetCosineSimilarityBetweenPhrases(frase, otrafrase);

                        var cov = 0.0;
                        foreach (var otrafrase in frasesActivas)
                            if (otrafrase > frase)
                                cov += MyTDM.PhrasesList[frase].SimilarityToDocument +
                                        MyTDM.PhrasesList[otrafrase].SimilarityToDocument;

                        var f = Math.Sqrt(1.0 / (MyTDM.PhrasesList[frase].PositionInDocument));
                        f += MyTDM.PhrasesList[frase].SimilarityToTitle;
                        f += (1 - complemento) / (1 + complemento);
                        f += cs + cov;

                        var nuevo = new KeyValuePair<int, double>(frase, f);
                        listaFrases.Add(nuevo);
                    }
                    listaFrases.Sort((x, y) => -1 * x.Value.CompareTo(y.Value));
                    break;
            }
            return listaFrases;
        }

        public void CalcularRankingPosicionFrases()
        {
            var totalFrases = MyTDM.PhrasesList.Count;
            PhrasePositionRanking = new double[totalFrases];
            for (var i = 0; i < totalFrases; i++)
                PhrasePositionRanking[i] = (2.0 - (2.0 * (i - 1) / (totalFrases - 1))) / totalFrases;
        }

        public void OrdenarLongitudes()
        {
            OrderedLengths = new List<int>();
            foreach (var t in MyTDM.PhrasesList)
                OrderedLengths.Add(t.Length);

            OrderedLengths.Sort((x,y) => -1 * x.CompareTo(y));
        }
    }
} 