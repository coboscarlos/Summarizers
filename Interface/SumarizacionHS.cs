using System;
using System.Collections.Generic;
using System.Windows.Forms;
using AccesoDatos;
using ReglasDeNegocio.UtilidadesSumHS;
using ReglasDeNegocio.HarmonySearch;
using System.IO;


//AGREGADO PARA MA
using ReglasDeNegocio.Memetico;

namespace Fachada
{
    public partial class SumarizacionHS : Form
    {
       
        private string _texto;
        //private string resumen;
        
        //variable para almacenar las url de los documentos a resumir
        static List<string> urlArchivo = new List<string>();

        float[] PRResumenes = new float[2]; // precision y recuerdo del conjunto de resumenes
        List<string> resumenTotal = new List<string>();

        
        public SumarizacionHS()
        {
            InitializeComponent();
        }
        static List<string> resumenModelo = new List<string>();

        private void btoResumen_Click(object sender, EventArgs e)
        {
            ejecutarAlgoritmo();
            urlArchivo.Clear();
        }
       
        private float[] calcularPreRecu(List<string> mejorResumen)
        {
            ///Correctas: numero de oraciones extraidas por el Usuario y Por el Humano
            ///Incorrectas: Numero de oraciones extraidas por el Sistema y no por el Humano
            ///Olvidades: Numero de oraciones extraidas por el Humano y no por el Sistema
            ///Presicion: Cuantas oraciones extraidas por el sistema fueron buenas
            ///Recuerdo: Refleja cuantas de las oraciones buenas olvido el sistema
            float correctas = 0;
            float incorrectas = 0;
            foreach (string mejor in mejorResumen)
            {
                if (resumenModelo.Contains(mejor)) 
                    correctas++;
                else
                    incorrectas++;
            }
            float olvidadas = 0;
            foreach (string modelo in resumenModelo)
            {
                if (!mejorResumen.Contains(modelo))
                    olvidadas++;
            }
            float presicion = correctas/(correctas+incorrectas);
            float recuerdo = correctas/(correctas+olvidadas);
            float[] presiRecuer = new float[2];
            presiRecuer[0] = presicion;
            presiRecuer[1] = recuerdo;
            return presiRecuer;
        }

        private void btoResumenModelo_Click(object sender, EventArgs e)
        {
            
            LeerArchivos objLeerArchivo = new LeerArchivos();
            string texto;
            texto = objLeerArchivo.LeerDTDResumenModelo(txtExaminarPR.Text);
            resumenModelo = objLeerArchivo.ObtenerFrases();
            foreach (string mode in resumenModelo)
            {
                txtResumenModelo.Text += "->";
                txtResumenModelo.Text += mode;
            }
            ///finaliza lectura de resumen modelo
            ///
            
           
        }

        private void salirToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btoExaminar_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
            string url;
            url = openFileDialog1.FileName;
            txtExaminar.Text = url;
            
        }

        private void btoExaminarPR_Click(object sender, EventArgs e)
        {
            openFileDialog2.ShowDialog();
            string url;
            url=openFileDialog2.FileName;
            txtExaminarPR.Text = url;
        }

        private void SumarizacionHS_Load(object sender, EventArgs e)
        {
            cbxCantidadPalabras.SelectedIndex = 0;
            //cbxIncremento.SelectedIndex = 0;
            cbxNi.SelectedIndex = 0;
        }

        private void cargar_Click(object sender, EventArgs e)
        {
            
            //urlArchivo = txtExaminar.Text; ///archivo a resumir
            if (!urlArchivo.Contains(txtExaminar.Text))
            {
                urlArchivo.Add(txtExaminar.Text);
                urlCargada.Text = urlArchivo[urlArchivo.Count - 1];
            }
            else { MessageBox.Show("El archivo ya está cargado"); }
        }

        private void mostrarurls_Click(object sender, EventArgs e)
        {
            
            MessageBox.Show("total de archivos cargados: "+urlArchivo.Count);
        }

        private void ejecutarAlgoritmo() {
            // se crea un archivo donde se almacenarán los resumenes
            string bestResume = "MejorResumen.txt";
            File.Delete(bestResume);
            FileStream streamResumen = new FileStream(bestResume, FileMode.OpenOrCreate, FileAccess.Write);
            StreamWriter writerResumen = new StreamWriter(streamResumen);

            // se crea el archivo en donde se almacenarán los resultados 
            string fileName = "Resultado.txt";
            File.Delete(fileName);
            FileStream stream = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.Write);
            StreamWriter writer = new StreamWriter(stream);


            //OJOOOOOOOOOOOOOO QUITAR ALGUNOS CAMPOS
            _texto = "ITERAC HMCR  PRESI_ALFA  PRESI_BETA  PRESI_GAMMA RECU_ALFA  RECU_BETA  RECU_GAMMA  MAX_PRESICION MAX_RECUERDO PROMEDIO_PRESICION  PROMEDIO_RECUERDO";
            writer.WriteLine(_texto);
            
            ///cantidad de palabras que debe cotener el resume
            int longRe = Convert.ToInt16(cbxCantidadPalabras.SelectedItem);

            //se hace un recorrido de los archivos a resumir.
           // for (int k = 0; k < urlArchivo.Count; k++)
           // {
                List<string> resumen = new List<string>();
                Hs objHs = new Hs();


            //AGREGADO PARA MA

                Memetico objMA;

                Agente objAgente = new Agente();


                ///coeficientes
                double[] cftes = new double[3];
                List<string> mejorResumenHS = new List<string>();

            ///AGREGADO PARA MA
                List<string> mejorResumenMA = new List<string>();


                List<string> mejorResumen = new List<string>();

                /// url de la lista de archivos cargados inicialmente
                ///cargar el archivo a resumir, sacar stopword, sacar frases, ...
                RepresentacionDoc objRepre = new RepresentacionDoc(urlArchivo);
                double Hmcr = Convert.ToDouble(txtHMCR.Text);
                ///Numero de iteraciones
                int Ni = Convert.ToInt16(cbxNi.SelectedItem);
                ///algortimo para maxima presicion y recuerdo
                float Max_Precision = 0;
                float Max_Recall = 0;
                float Average_Precision = 0;
                float Average_Recall = 0;
                double beta, alfa, gamma;
                beta = Convert.ToDouble(txtBeta.Text);
                alfa = Convert.ToDouble(txtAlfa.Text);
                gamma = Convert.ToDouble(txtGamma.Text);
                cftes[0] = alfa;
                cftes[1] = beta;
                cftes[2] = gamma;
                double mejorBetaP = 0, mejorAlfaP = 0, mejorGammaP = 0;
                double mejorBetaR = 0, mejorAlfaR = 0, mejorGammaR = 0;
                float[] presiRecuer = new float[2];
                //buscar los mejores coeficientes       
                for (int i = 0; i < 10; i++)
                {
                    alfa = Convert.ToDouble(i) / 10;
                    cftes[0] = alfa;
                    for (int j = 0; j < 10 - i; j++)
                    {
                        beta = Convert.ToDouble(j) / 10;
                        cftes[1] = beta;
                        gamma = 1 - beta - alfa;
                        cftes[2] = gamma;
                        //Run HS and Compute Precision and Recall

                        //LLAMAR A MEMETICO
                        objMA = new Memetico(objRepre.Frases.Count, longRe, objRepre.MatrizSimilituF, objRepre.MatrizPesosFrases, cftes, objRepre.Frases);

                        /*
                         * MA:
                         * double[] CostosCaracteristicas = new double[objRepre.MatrizPesosFrases.GetLength(0)-2];
                        CostosCaracteristicas = objMA.CostosCaracteristicas(objRepre.MatrizSimilituF, objRepre.MatrizPesosFrases);*/

                                                
                        //mejorResumenHS = objHs.ejecutarHS(Hmcr, Ni, objRepre.Frases, objRepre.MatrizPesosFrases, objRepre.MatrizSimilituF, cftes,longRe);

                        objAgente = objMA.Ejecutar();

                        ///METODO Q CONVIERTA UN VECTOR SLN DE 1'S Y 0'S EN UN LIST<STRING>
                        mejorResumenMA = objMA.ObtenerTexto(objAgente, objRepre.Frases);

                        for (int t = 0; t < mejorResumenMA.Count; t++)
                        {
                            txtResultados.Text += mejorResumenMA[i] + " ";
                        }
                        
                        //retorna la presicion y el recuerdo de este resumen
                        //presiRecuer = calcularPreRecu(mejorResumenHS);-->HS
                        
                        //AGREGADO PARA MA
                        //presiRecuer = calcularPreRecu(mejorResumenMA);

                        Console.WriteLine("aqui");    

                    /*if (presiRecuer[0] > Max_Precision)
                        {
                            Max_Precision = presiRecuer[0];
                            mejorBetaP = beta;
                            mejorAlfaP = alfa;
                            mejorGammaP = gamma;
                            mejorResumen = mejorResumenHS;
                        }
                        if (presiRecuer[1] > Max_Recall)
                        {
                            Max_Recall = presiRecuer[1];
                            mejorBetaR = beta;
                            mejorAlfaR = alfa;
                            mejorGammaR = gamma;
                            mejorResumen = mejorResumenHS;
                        }
                        Average_Precision = Average_Precision + presiRecuer[0];
                        Average_Recall = Average_Recall + presiRecuer[1];*/
                        
                    }
                }
                /*Average_Precision = Average_Precision / 66;
                Average_Recall = Average_Recall / 66;

                texto =  Ni.ToString() + "   " + Hmcr.ToString() + "    " + mejorAlfaP.ToString() + "    " + mejorBetaP.ToString() + "   " + mejorGammaP.ToString() + "     " + mejorAlfaR.ToString() + "    " + mejorBetaR.ToString() + "   " + mejorGammaR.ToString() + "   " + Max_Precision.ToString() + "   " + Max_Recall.ToString() + "   " + Average_Precision.ToString() + "   " + Average_Recall.ToString();
                writer.WriteLine(texto);

                Console.WriteLine("Precision: " + Max_Precision.ToString() + " Recuerdo:" + Max_Recall.ToString());
                //Console.WriteLine("Preciscion:" + presiRecuer[0] + "Recuerdo " + presiRecuer[1]);
            writerResumen.Close();
            writer.Close();*/
        
        }

    }
}