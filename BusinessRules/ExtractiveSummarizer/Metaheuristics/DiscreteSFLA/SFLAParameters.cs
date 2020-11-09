namespace BusinessRules.ExtractiveSummarizer.Metaheuristics.DiscreteSFLA
{    
    public class SFLAParameters : BaseParameters
    {
        public int M { get; set; }
        public int C { get; set; }      
        public int Tenure { get; set; }
        public int PondSize { get; set; } = 20;
        public int NumberOfMemeplexes { get; set; } = 5;
        public int MaxLocalIterations { get; set; } = 10;
        public double ProbabilityOfMutation { get; set; } = 0.06;

        public override string ToString()
        {
            var result = base.ToString() + "-SFLA-" +
                         M + "-" +
                         C + "-" +
                         Tenure + "-" +
                         PondSize + "-" +
                         NumberOfMemeplexes + "-" +
                         MaxLocalIterations + "-" +
                         ProbabilityOfMutation + "-";
            return result;
        }
    }

    public enum WhaShouldBeOptimized
    {
        Nothing = 0,
        Best = 1,
        Worst = 2,
        Randomly = 3
    }

    public enum WhereToOptimize
    {
        WhenInitializing = 1,
        WhenIterating = 2
    }

    public enum OptimizationMethod
    {
        None = 0,
        HillClimbing = 1,
        TabuSearch = 2
    }

    public enum TabuMemoryType
    {
        LocalExplicit,
        GlobalExplicit
    }

    public enum OrderingPhrasesByPriority
    {
        Position = 0,
        Coverage = 1,
        Length = 2,
        SimilarityToTitle = 3,
        PositionAndCoverage = 4,
        SimilarityToTitleAndCoverage = 5,
        Cohesion = 6,
        CoverageAndCohesion = 7,
        PositionAndCohesion = 8,
        PositionAndSimilarityToTitle = 9,
        CoverageAndSimilarityToTitle = 10,
        PositionAndCoverageAndSimilarityToTitle = 11
    }
}