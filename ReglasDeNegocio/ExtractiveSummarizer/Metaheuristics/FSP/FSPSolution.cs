using System;
using System.Collections.Generic;
using System.Diagnostics;
using BusinessRules.Utils;

namespace BusinessRules.ExtractiveSummarizer.Metaheuristics.FSP
{
    public class FSPSolution : BaseSolucion
    {   
        public FSPSolution(DiscreteFSP myContainer): base(myContainer){
        }

        public FSPSolution(FSPSolution copia): base (copia){
        }

        public FSPSolution ThrowFishingNet(int c)
        {
            try
            {
                var misParamettros = (FSPDiscreto)((DiscreteFSP)MyContainer).MyParameters;
                var m = misParamettros.M;
                var solucionesCandidatasGeneredas = new List<FSPSolution>();

                for (var i = 0; i < m; i++)
                {
                    var nuevaSolucionCandidata = new FSPSolution(this);
                    var frasesSeleccionadas = nuevaSolucionCandidata.ObtainPhrasesSortedByCoverageCosine();
                    var frasesPosibles = nuevaSolucionCandidata.MyContainer.ViablePhrases;
                    //var solucionAjustada = (new Ajuste(MyContainer)).AjusteMultiBit(nuevaSolucionCandidata);
                    //Remuevo las C ultimas frases
                    if (frasesSeleccionadas.Count > c)
                        frasesSeleccionadas.RemoveRange(frasesSeleccionadas.Count - c, c);
                    //Agrego C frases para completar la solución
                    for (var j = 0; j < c; j++)
                    {
                        int idFraseCandidata;
                        int posicioncandidata;
                        bool noMasPosibles;
                        do
                        {
                            posicioncandidata = misParamettros.NumeroAleatorio.Next(frasesPosibles.Count);
                            idFraseCandidata = frasesPosibles[posicioncandidata].Position;
                            //Varieble por si todas la oraciones estan activas
                            noMasPosibles = frasesSeleccionadas.Count == frasesPosibles.Count;
                        } while (ActivePhrases.Contains(idFraseCandidata) & noMasPosibles);
                        if (noMasPosibles) break;
                        frasesSeleccionadas.Add(new PositionValue(idFraseCandidata, frasesPosibles[posicioncandidata].Value));
                    }

                    nuevaSolucionCandidata.ActivePhrases.Clear();
                    var cantidadPalabras = 0; // Permite controlar el tamaño del resumen                   
                    while (cantidadPalabras < misParamettros.MaximumLengthOfSummaryForRouge)
                    {
                        if (frasesSeleccionadas.Count == 0) break;
                        var valorEnFrases = misParamettros.NumeroAleatorio.Next(frasesSeleccionadas.Count);
                        var valor = frasesSeleccionadas[valorEnFrases].Position;
                        cantidadPalabras += MyContainer.MyTDM.PhrasesList[valor].Length;
                        frasesSeleccionadas.RemoveAt(valorEnFrases);
                        nuevaSolucionCandidata.ActivePhrases.Add(valor);
                    }
                    nuevaSolucionCandidata.ActivePhrases.Sort((x,y) => x.CompareTo(y));
                    nuevaSolucionCandidata.SummaryLength = cantidadPalabras;
                    nuevaSolucionCandidata.OptimizationComplete();
                    nuevaSolucionCandidata.CalculateFitness();

                    solucionesCandidatasGeneredas.Add(nuevaSolucionCandidata);
                }
                solucionesCandidatasGeneredas.Sort((x, y) => -1 * x.Fitness.CompareTo(y.Fitness));

                if (solucionesCandidatasGeneredas[0].Fitness > Fitness) // Se esta MAXIMIZANDO                
                    return solucionesCandidatasGeneredas[0];
                return new FSPSolution(this);                  
            }
            catch (Exception e)
            {                
                Debug.WriteLine("Error de indice atrapado en Tirar Red : " + e.Message);
                return null;
            }
        }
    }
}