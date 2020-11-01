using System;
using System.Threading;
using BusinessRules.Utils;

namespace Interface
{
    public class RougeEnParalelo
    {
        private readonly string _directorioRaizRouge;
        private readonly string _directorioDeSalida;
        private readonly int _experimento;
        private readonly ManualResetEvent _doneEvent;

        public RougeEnParalelo(string dirRouge, string dirSalida, int exp, ManualResetEvent doneEvent)
        {
            _directorioRaizRouge = dirRouge;
            _directorioDeSalida = dirSalida;
            _experimento = exp;
            _doneEvent = doneEvent;
        }

        public void Ejecutar(Object threadContext)
        {
            var directorioExperimento = _directorioDeSalida + @"\" + _experimento.ToString("000");
            Rouge.EvaluateAnExperimentWithAllNewsPartA(_directorioRaizRouge, directorioExperimento);
            _doneEvent.Set();
        }
    }
}
