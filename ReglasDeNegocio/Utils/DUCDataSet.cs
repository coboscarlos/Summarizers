namespace BusinessRules.Utils
{
    public class DUCDataSet
    {
        public string Name;
        public string RougeRootDirectory;
        public string MatricesRootDirectory;

        public DUCDataSet(string name, string dirRouge, string dirMatrix)
        {
            Name = name;
            RougeRootDirectory = dirRouge;
            MatricesRootDirectory = dirMatrix;
        }
    }
}