using System.Collections.Generic;
using System.Linq;

namespace BusinessRules.ExtractiveSummarizer.Metaheuristics.DiscreteSFLA
{
    public class TabuMemory
    {
        public List<List<int>> GlobalExplicitTabuMemory;
        public int Tenure;

        public TabuMemory(int tenure)
        {
            Tenure = tenure;
            GlobalExplicitTabuMemory = new List<List<int>>();
        }        
       
        public bool AreListsEquals(List<int> solucion1,List<int> solucion2)
        {
            if (solucion1.Count != solucion2.Count) return false;
            return !solucion1.Where((t, i) => t != solucion2[i]).Any();
        }

        public bool IsTabu(List<int> activePhrases)
        {
            var tabu = false;
            foreach (var lstfrases in GlobalExplicitTabuMemory)
                if (AreListsEquals(lstfrases, activePhrases))
                    tabu = true;

            if (GlobalExplicitTabuMemory.Count >= Tenure)
                GlobalExplicitTabuMemory.RemoveAt(0);
            GlobalExplicitTabuMemory.Add(activePhrases);
            return tabu;
        }

        public void Include(Frog frog)
        {
            if (GlobalExplicitTabuMemory.Count >= Tenure)
                GlobalExplicitTabuMemory.RemoveAt(0);
            GlobalExplicitTabuMemory.Add(frog.SelectedPhrases);
        }
    }
}