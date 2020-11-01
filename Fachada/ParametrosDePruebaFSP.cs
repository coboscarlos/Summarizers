using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using BusinessRules.VectorSpaceModel;
using BusinessRules.ExtractiveSummarizer.Metaheuristics.FSP;
using BusinessRules.Utils;

namespace Interface
{
    class ParametrosDePruebaFSP
    {
        private static ParametrosDePruebaFSP _instance;
        private readonly object _thisLock;

        private readonly List<FSPDiscreto> _listaDeParametrosaProbar;
        private int _indice;

        private ParametrosDePruebaFSP()
        {
            _indice = 0;
            _thisLock = new object();
            _listaDeParametrosaProbar = new List<FSPDiscreto>();

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

        public FSPDiscreto FijarParametro(double alfa, double beta, double gamma, double delta, double ro)
        {
            var elCasoDeOptimizacion = new FSPDiscreto
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
                N = 7,
                M = 15,
                C = 2,
                L = 3,
                T = 5,
                TheFitnessFunction = FitnessFunction.MASDS,
                Alfa = alfa, 
                Beta = beta, 
                Gamma = gamma, 
                Delta = delta,
                Ro = ro
            };
            return elCasoDeOptimizacion;
        }

        public static ParametrosDePruebaFSP Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ParametrosDePruebaFSP();
                }
                return _instance;
            }
        }

        public FSPDiscreto GetLast()
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
