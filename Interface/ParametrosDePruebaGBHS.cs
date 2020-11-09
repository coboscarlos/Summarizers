using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using BusinessRules.ExtractiveSummarizer;
using BusinessRules.VectorSpaceModel;
using BusinessRules.ExtractiveSummarizer.Metaheuristics.GBHS;
using BusinessRules.Utils;

namespace Interface
{
    class ParametrosDePruebaGBHS
    {
        private static ParametrosDePruebaGBHS _instance;
        private readonly Object _thisLock;

        private readonly List<GBHSParameters> _listaDeParametrosaProbar;
        private int _indice;

        private ParametrosDePruebaGBHS()
        {
            _indice = 0;
            _thisLock = new Object();
            _listaDeParametrosaProbar = new List<GBHSParameters>();

            //var dsNoProbar = new DataSet();  // En este archivo se ponen los parametros que no se quieren probar
            //dsNoProbar.ReadXml("YaProbados.xml");

            //for (var alfa = 0.40m; alfa <= 0.60m; alfa += 0.05m)
            //    for (var beta = 0.00m; beta <= 0.00m; beta += 0.20m)
            //        for (var gamma = 0.20m; gamma <= 0.40m; gamma += 0.05m)
            //            for (var delta = 0.00m; delta <= 0.00m; delta += 0.20m)
            //                for (var ro = 0.10m; ro <= 0.30m; ro += 0.05m)
            //                {
            //                    var lsql = "Alpha = " + alfa + " AND Beta = " + beta + " AND " +
            //                               "Gamma = " + gamma + " AND Delta = " + delta + " AND " +
            //                               "Ro = " + ro;
            //                    lsql = lsql.Replace(",", ".");

            //                    if (dsNoProbar.Tables[0].Select(lsql).Count() != 0) continue;

            //                    var suma = alfa + beta + gamma + delta + ro;
            //                    if (Math.Abs(suma - 1.0m) > 0.000001m) continue;

            //                    Debug.WriteLine(alfa + Characters.Tab + beta + Characters.Tab + 
            //                                    gamma + Characters.Tab + delta + Characters.Tab + ro);
            //                    var elCasoDeOptimizacion = FijarParametro((double)alfa, (double)beta, (double)gamma, (double)delta, (double)ro);
            //                    _listaDeParametrosaProbar.Add(elCasoDeOptimizacion);
            //                }

            var dsProbar = new DataSet();  // En este archivo se ponen los parametros que fijo se quieren probar
            dsProbar.ReadXml("SeDebenProbar.xml");
            foreach (DataRow fila in dsProbar.Tables[0].Rows)
            {
                var alfa = double.Parse(fila["Alpha"].ToString());
                var beta = double.Parse(fila["Beta"].ToString());
                var gamma = double.Parse(fila["Gamma"].ToString());
                var delta = double.Parse(fila["Delta"].ToString());
                var ro = double.Parse(fila["Ro"].ToString());
                
                var elCasoDeOptimizacion = FijarParametro(alfa, beta, gamma, delta, ro);
                _listaDeParametrosaProbar.Add(elCasoDeOptimizacion);                
            }

            Debug.WriteLine("TOTAL : " + _listaDeParametrosaProbar.Count);
        }

        public GBHSParameters FijarParametro(double alfa, double beta, double gamma, double delta, double ro)
        {
            var elCasoDeOptimizacion = new GBHSParameters
            {
                DetailedReport = false,
                MySummaryType = SummaryType.Words,
                MaximumLengthOfSummaryForRouge = 100,
                MaximumSummaryLengthToEvolve = 110,
                MyTDMParameters = new TDMParameters
                {
                    MinimumFrequencyThresholdOfTermsForPhrase = 0, // 0 y 0 son los dos valores originales
                    MinimumThresholdForTheAcceptanceOfThePhrase = 0.0,
                    TheDocumentRepresentation = DocumentRepresentation.Centroid,
                    TheTFIDFWeight  = TFIDFWeight.BM25
                },
                MaximumNumberOfFitnessFunctionEvaluations = 1600,
                HMS = 10,
                HMCR = 0.85,
                ParMin = 0.01,
                ParMax = 0.99,
                TheFitnessFunction = FitnessFunction.MASDS,
                Alfa = alfa, 
                Beta = beta, 
                Gamma = gamma, 
                Delta = delta,
                Ro = ro,
                OptimizacionProbability = 0.4,
                MaxNumberOfOptimizacions = 5
            };
            return elCasoDeOptimizacion;
        }

        public static ParametrosDePruebaGBHS Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ParametrosDePruebaGBHS();
                }
                return _instance;
            }
        }

        public GBHSParameters GetLast()
        {
            int salida;
            lock (_thisLock)
            {
                salida = _indice;
                _indice++;
                if (_indice >= _listaDeParametrosaProbar.Count)
                    salida = -1;
            }
            if (salida == -1) return null;
            return _listaDeParametrosaProbar[salida];
        }
    }
}
