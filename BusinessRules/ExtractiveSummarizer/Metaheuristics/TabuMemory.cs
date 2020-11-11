using System.Collections.Generic;
using System.Linq;

namespace BusinessRules.ExtractiveSummarizer.Metaheuristics
{
    public class TabuMemory
    {
        public List<List<int>> MyMemory;
        public int Tenure;

        public TabuMemory(int tenure)
        {
            Tenure = tenure;
            MyMemory = new List<List<int>>();
        }        
       
        public bool AreListsEquals(List<int> solucion1,List<int> solucion2)
        {
            if (solucion1.Count != solucion2.Count) return false;
            return !solucion1.Where((t, i) => t != solucion2[i]).Any();
        }

        public bool IsTabu(List<int> selectedPhrases)
        {
            var tabu = false;
            foreach (var lstfrases in MyMemory)
                if (AreListsEquals(lstfrases, selectedPhrases))
                    tabu = true;

            if (MyMemory.Count >= Tenure)
                MyMemory.RemoveAt(0);
            MyMemory.Add(selectedPhrases);
            return tabu;
        }

        public void Include(List<int> selectedPhrases)
        {
            if (MyMemory.Count >= Tenure)
                MyMemory.RemoveAt(0);
            MyMemory.Add(selectedPhrases);
        }
    }
}