namespace BusinessRules.ExtractiveSummarizer.Metaheuristics.FSP
{
    public class FSPDiscreto : BaseParameters
    {
        public int N { get; set; }
        public int L { get; set; }  // numero de veces que se arroja la red de pesca en un punto de captura
        public int M { get; set; }  // cantidad de vectores de posicion de la red
        public int C { get; set; }  // coeficiente de amplitud     
        public int T { get; set; }

        public override string ToString()
        {
            var resultado = base.ToString() + "-FSP-" + 
                            N + "-" +                            
                            L + "-" +
                            M + "-" +
                            C + "-" +
                            T;
            return resultado;
        }
    }
}