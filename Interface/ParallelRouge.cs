using System;
using System.Threading;
using BusinessRules.Utils;

namespace Interface
{
    public class ParallelRouge
    {
        private readonly string _directoryRootRouge;
        private readonly string _outputDirectory;
        private readonly int _experiment;
        private readonly ManualResetEvent _doneEvent;

        public ParallelRouge(string dirRouge, string dirOutput, int exp, ManualResetEvent doneEvent)
        {
            _directoryRootRouge = dirRouge;
            _outputDirectory = dirOutput;
            _experiment = exp;
            _doneEvent = doneEvent;
        }

        public void Execute(Object threadContext)
        {
            var experimentDirectory = _outputDirectory + @"\" + _experiment.ToString("000");
            Rouge.EvaluateAnExperimentWithAllNewsPartA(_directoryRootRouge, experimentDirectory);
            _doneEvent.Set();
        }
    }
}
