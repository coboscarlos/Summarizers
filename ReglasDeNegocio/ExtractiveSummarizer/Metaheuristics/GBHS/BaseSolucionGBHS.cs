using System;
using System.Collections.Generic;
using System.Diagnostics;
using BusinessRules.Utils;

namespace BusinessRules.ExtractiveSummarizer.Metaheuristics.GBHS
{
    public class BaseSolucionGBHS : BaseSolucion
    {
        public BaseSolucionGBHS(GBHS myContainer): base (myContainer){
        }

        public BaseSolucionGBHS(BaseSolucionGBHS copia): base(copia){
        }

        public void CopyTo(BaseSolucionGBHS destino){
            base.CopyTo(destino);
        }

        public void Optimizar()
        {
            var misParametros = (GBHSParameters)MyContainer.MyParameters;
            // Si NO esta dentro de la probabilidad NO se optimiza
            if (misParametros.NumeroAleatorio.NextDouble() > misParametros.ProbabilidadOptimizacion)
                return;

            try
            {
                OptimizarGredyMejorPorCobertura(1);
            }
            catch (Exception e1)
            {
                Debug.WriteLine("Error : " + e1.Message);
            }
        }

        public void IncluirFrasesCumpliendoRestriccion(List<PositionValue> frasesSeleccionadas)
        {
            ActivePhrases.Clear();
            SummaryLength = 0; // Permite controlar el tamaño del resumen
            while (SummaryLength < MyContainer.MyParameters.MaximumLengthOfSummaryForRouge)
            {
                var faltan = MyContainer.MyParameters.MaximumLengthOfSummaryForRouge - SummaryLength;
                frasesSeleccionadas.RemoveAll(x => MyContainer.MyTDM.PhrasesList[x.Position].Length > faltan);

                if (frasesSeleccionadas.Count == 0) break;

                var valor = frasesSeleccionadas[0];
                frasesSeleccionadas.RemoveAt(0);
                ActivePhrases.Add(valor.Position);
                SummaryLength += MyContainer.MyTDM.PhrasesList[valor.Position].Length;
            }
        }

        //// Version de prueba
        //private void OptimizarGredyMejorPorCobertura(int vecinos)
        //{
        //    try
        //    {
        //        var solucionOriginal = new BaseSolucionGBHS(this);
        //        var frasesPosibles = MyContainer.ViablePhrases;
        //        var frasesSeleccionadas = ObtainPhrasesSortedByCoverageCosine();

        //        if (ActivePhrases.Count == MyContainer.SolutionSize)
        //            return;

        //        var myContainer = (GBHS)MyContainer;
        //        for (var cambio = 0; cambio < ((GBHS)myContainer.MyTDMParameters).MaximoNumeroOptimizacion; cambio++)
        //        {
        //            var posEliminar = MyContainer.MyTDMParameters.NumeroAleatorio.Next(ActivePhrases.Count);
        //            SummaryLength -= MyContainer.MyTDM.PhraseList[ActivePhrases[posEliminar]].Length;
        //            ActivePhrases.RemoveAt(posEliminar);


        //            IncluirFrasesCumpliendoRestriccion(frasesSeleccionadas);
        //            AgregarFrasesValidas();
        //            CalculateFitness();
        //            ActivePhrases.Sort((x, y) => x.CompareTo(y));

        //            if (myContainer.UpdateFitness)
        //            {
        //                solucionOriginal.CalculateFitness(true);
        //            }

        //            var aleatorio = myContainer.MyTDMParameters.NumeroAleatorio.NextDouble();
        //            //var probabilidadSA = (Fitness - solucionOriginal.Fitness);
        //            //var divisor = (double)(MyContainer.MaximumNumberOfFitnessFunctionEvaluations - MyContainer.CurrentFFEs);
        //            //if (divisor < 1.0)
        //            //    divisor = 1.0;
        //            //probabilidadSA = probabilidadSA / divisor;
        //            //probabilidadSA = Math.Pow(Math.E, probabilidadSA);
        //            var probabilidadSA = 0.0;

        //            if (CompareTo(solucionOriginal) < 0 || aleatorio < probabilidadSA)
        //            {
        //                // La nueva solución es mejor ... se entra en profundidad por este camino
        //                // o el recocido simulado le da chance de entrar asi no sea mejor

        //                if (CompareTo(solucionOriginal) < 0) //es mejor?
        //                    myContainer.SuccessInOptimization++;
        //                else
        //                    myContainer.OportunidadSA++;

        //                solucionOriginal = new BaseSolucionGBHS(this);
        //                myContainer.SuccessInOptimization++;
        //            }
        //            else
        //            {
        //                solucionOriginal.CopyTo(this);
        //                myContainer.OptimizationFailures++;

        //            }
        //            frasesSeleccionadas = ObtainPhrasesSortedByCoverageCosine();

        //            if (CompareTo(mejorSolucion) < 0) //es mejor?
        //                mejorSolucion = new BaseSolucionGBHS(this);
        //        }

        //        mejorSolucion.CopyTo(this);
        //    }
        //    catch (Exception e)
        //    {
        //        Debug.WriteLine("Error de indice atrapado en Optimizar : " + e.Message);
        //    }
        //}



        // Version estable: da buenos resultados pero no suficientes!
        private void OptimizarGredyMejorPorCobertura(int vecinos)
        {
            try
            {
                var solucionOriginal = new BaseSolucionGBHS(this);
                var mejorSolucion = new BaseSolucionGBHS(this);
                var frasesSeleccionadas = ObtainPhrasesSortedByCoverageCosine();
                var frasesPosibles = MyContainer.ViablePhrases;

                if (frasesSeleccionadas.Count == MyContainer.SolutionSize)
                    return;

                var posicioncandidata = 0;
                var miAlgoritmoContenedor = (GBHS)MyContainer;
                for (var cambio = 0; cambio < ((GBHSParameters)miAlgoritmoContenedor.MyParameters).MaximoNumeroOptimizacion; cambio++)
                {
                    for (var cambios = 0; cambios < vecinos; cambios++)
                    {
                        int idFraseCandidata;
                        var intentos = 0;
                        double valor;
                        do
                        {
                            if (posicioncandidata >= frasesPosibles.Count) posicioncandidata = 0;

                            idFraseCandidata = frasesPosibles[posicioncandidata].Position;
                            valor = frasesPosibles[posicioncandidata].Value;
                            posicioncandidata++;
                            intentos++;
                            if (intentos >= frasesPosibles.Count) return;
                        } while (ActivePhrases.Contains(idFraseCandidata));

                        frasesSeleccionadas.RemoveAt(frasesSeleccionadas.Count - 1);
                        frasesSeleccionadas.Add(new PositionValue(idFraseCandidata, valor));
                    }
                    frasesSeleccionadas.Sort((x, y) => -1 * x.Value.CompareTo(y.Value));

                    IncluirFrasesCumpliendoRestriccion(frasesSeleccionadas);
                    AgregarFrasesValidas();
                    CalculateFitness();
                    ActivePhrases.Sort((x, y) => x.CompareTo(y));

                    if (miAlgoritmoContenedor.UpdateFitness)
                    {
                        solucionOriginal.CalculateFitness(true);
                        mejorSolucion.CalculateFitness(true);
                    }

                    var aleatorio = miAlgoritmoContenedor.MyParameters.NumeroAleatorio.NextDouble();
                    //var probabilidadSA = (Fitness - solucionOriginal.Fitness);
                    //var divisor = (double)(MyContainer.MaximumNumberOfFitnessFunctionEvaluations - MyContainer.CurrentFFEs);
                    //if (divisor < 1.0)
                    //    divisor = 1.0;
                    //probabilidadSA = probabilidadSA / divisor;
                    //probabilidadSA = Math.Pow(Math.E, probabilidadSA);
                    var probabilidadSA = 0.0;

                    if (CompareTo(solucionOriginal) < 0 || aleatorio < probabilidadSA)
                    {
                        // La nueva solución es mejor ... se entra en profundidad por este camino
                        // o el recocido simulado le da chance de entrar asi no sea mejor

                        if (CompareTo(solucionOriginal) < 0) //es mejor?
                            miAlgoritmoContenedor.SuccessInOptimization++;
                        else
                            miAlgoritmoContenedor.OportunidadSA++;

                        solucionOriginal = new BaseSolucionGBHS(this);
                        miAlgoritmoContenedor.SuccessInOptimization++;
                    }
                    else
                    {
                        solucionOriginal.CopyTo(this);
                        miAlgoritmoContenedor.OptimizationFailures++;

                    }
                    frasesSeleccionadas = ObtainPhrasesSortedByCoverageCosine();

                    if (CompareTo(mejorSolucion) < 0) //es mejor?
                        mejorSolucion = new BaseSolucionGBHS(this);
                }

                mejorSolucion.CopyTo(this);
            }
            catch (Exception e)
            {
                Debug.WriteLine("Error de indice atrapado en Optimizar : " + e.Message);
            }
        }
    }
}