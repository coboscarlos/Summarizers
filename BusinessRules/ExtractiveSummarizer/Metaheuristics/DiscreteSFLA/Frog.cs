using System;

namespace BusinessRules.ExtractiveSummarizer.Metaheuristics.DiscreteSFLA
{
    public class Frog : BaseSolucion
    {
        public readonly double Alfa = 0.4;

        public Frog(BaseAlgorithm myContainer) : base(myContainer) {
        }

        public Frog(Frog origin) : base(origin)
        {
            Alfa = origin.Alfa;
        }

        public void Jump(Frog best)
        {
            var alfaSup = 0.5 * (Alfa + 1);

            for (var i = 0; i < MyContainer.SolutionSize; i++)
            {
                if (Phrase(i) != best.Phrase(i))
                {
                    var s = MyContainer.MyParameters.RandomGenerator.NextDouble() * (Phrase(i) - best.Phrase(i));
                    var t = 1.0 / (1.0 + Math.Exp(-s));
                    if (t <= Alfa) InActivate(i); // y = 0
                    if (t > Alfa && t <= alfaSup) {/* do nothing y = Phrase(i)*/}
                    if (t > alfaSup) Activate(i); //y = 1;
                }
            }

            while (SummaryLength > MyContainer.MyParameters.MaximumLengthOfSummaryForRouge)
            {
                var pos = MyContainer.MyParameters.RandomGenerator.Next(SelectedPhrases.Count);
                pos = SelectedPhrases[pos];
                InActivate(pos);
            }

            AddValidPhrases();
            CalculateFitness();
        }

        /// <summary>
        /// This operator unselect a randomly selected phrase and complete the frog
        /// </summary>
        public void Mutate()
        {
            var pos = MyContainer.MyParameters.RandomGenerator.Next(SelectedPhrases.Count);
            pos = SelectedPhrases[pos];
            InActivate(pos);
            AddValidPhrases();
            CalculateFitness();
        }
    }
}