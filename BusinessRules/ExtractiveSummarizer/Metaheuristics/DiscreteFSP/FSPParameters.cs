namespace BusinessRules.ExtractiveSummarizer.Metaheuristics.DiscreteFSP
{
    public class FSPParameters : BaseParameters
    {
        public int N { get; set; }
        public int L { get; set; }  // number of times the fishing net is thrown at a catch point
        public int M { get; set; }  // number of network position vectors
        public int Tenure { get; set; } = 8;

        public override string ToString()
        {
            var result = base.ToString() + "-FSP-" + 
                            N + "-" +                            
                            L + "-" +
                            M + "-" +
                            Tenure;
            return result;
        }
    }
}