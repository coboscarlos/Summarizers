namespace BusinessRules.ExtractiveSummarizer.Metaheuristics.FSP
{
    public class DiscreteFSPParameters : BaseParameters
    {
        public int N { get; set; }
        public int L { get; set; }  // number of times the fishing net is thrown at a catch point
        public int M { get; set; }  // number of network position vectors
        public int C { get; set; }  // amplitude coefficient    
        public int T { get; set; }

        public override string ToString()
        {
            var result = base.ToString() + "-FSP-" + 
                            N + "-" +                            
                            L + "-" +
                            M + "-" +
                            C + "-" +
                            T;
            return result;
        }
    }
}