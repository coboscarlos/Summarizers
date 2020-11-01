using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace BusinessRules.Utils
{
    public static class Rouge
    {
        public const string RougeComand1 =
            "../../ROUGE-1.5.5.pl -e ../../data -n 2 -m -2 4 -u -c 95 -r 1000 -f A -p 0.5 -t 0 -a ";

        public const string RougeComand2 =
            "perl.exe ../../../../../ROUGE-1.5.5.pl -e../../../../../data -n 2 -m -2 4 -u -c 95 -r 1000 -f A -p 0.5 -t 0 -a -d rougejk-sin.xml > salida.out";

        public static string Evaluate(string rougeRootDirectory, string theNews, string outputSummary)
        {
            var error = true;
            do
            {           
                try
                {
                    File.Delete(rougeRootDirectory + @"systems\" + theNews);
                    error = false;
                }
                catch (Exception e)
                {
                    Debug.WriteLine("ERROR Rouge.Evaluate : " + e.Message);
                    Thread.Sleep(10);
                }
            } while (error);

            File.AppendAllText(rougeRootDirectory + @"systems\" + theNews, outputSummary);
            
            var infoProcess = new ProcessStartInfo("perl.exe")
            {
                WindowStyle = ProcessWindowStyle.Minimized,
                Arguments = RougeComand1 + theNews + ".xml",
                CreateNoWindow = true,
                WorkingDirectory = rougeRootDirectory,
                RedirectStandardOutput = true,
                UseShellExecute = false               
            };
            var miProcess = new Process { StartInfo = infoProcess };
            miProcess.Start();
            miProcess.WaitForExit();
            var resultProcess = miProcess.StandardOutput.ReadToEnd();
            var separators = new[] { "\r\n" };
            var lines = resultProcess.Split(separators, StringSplitOptions.RemoveEmptyEntries);
            var output = "";
            output += CutNumber(lines[1], "1 ROUGE-1 Average_R:") + "\t";
            output += CutNumber(lines[2], "1 ROUGE-1 Average_P:") + "\t";
            output += CutNumber(lines[3], "1 ROUGE-1 Average_F:") + "\t";
            output += CutNumber(lines[5], "1 ROUGE-2 Average_R:") + "\t";
            output += CutNumber(lines[6], "1 ROUGE-2 Average_P:") + "\t";
            output += CutNumber(lines[7], "1 ROUGE-2 Average_F:") + "\t";
            output += CutNumber(lines[9], "1 ROUGE-L Average_R:") + "\t";
            output += CutNumber(lines[10], "1 ROUGE-L Average_P:") + "\t";
            output += CutNumber(lines[11], "1 ROUGE-L Average_F:") + "\t";
            output += CutNumber(lines[13], "1 ROUGE-SU4 Average_R:") + "\t";
            output += CutNumber(lines[14], "1 ROUGE-SU4 Average_P:") + "\t";
            output += CutNumber(lines[15], "1 ROUGE-SU4 Average_F:") + "\t";

            return output;
        }

        public static void EvaluateAnExperimentWithAllNewsPartA(string rougeRootDirectory,
            string specificDirectoryOfTheExperiment)
        {
            if (!File.Exists(specificDirectoryOfTheExperiment + @"\rougejk-sin.xml"))
            {
                File.Copy(rougeRootDirectory + "rougejk-sin.xml", specificDirectoryOfTheExperiment + @"\rougejk-sin.xml");
                File.WriteAllText(specificDirectoryOfTheExperiment + @"\eval.bat", RougeComand2);
            }

            var miProcessInfo = new ProcessStartInfo(specificDirectoryOfTheExperiment + @"\eval.bat") { WorkingDirectory = specificDirectoryOfTheExperiment };
            var miProcess = new Process { StartInfo = miProcessInfo };
            miProcess.Start();
            miProcess.WaitForExit();
        }

        public static void EvaluateAnExperimentWithAllNewsPartB(string rougeRootDirectory,
            string specificDirectoryOfTheExperiment, ref double[] resultOfTheExperiment, 
            ref SubTotalsByDataSet mySubtotals)
        {
            var contentFile = File.ReadAllText(specificDirectoryOfTheExperiment + @"\salida.out");
            var separators = new[] { "\r\n" };
            var lines = contentFile.Split(separators, StringSplitOptions.RemoveEmptyEntries);

            var valor = CutNumber(lines[1], "1 ROUGE-1 Average_R:");
            resultOfTheExperiment[0] = double.Parse(valor);

            valor = CutNumber(lines[2], "1 ROUGE-1 Average_P:");
            resultOfTheExperiment[1] = double.Parse(valor);

            valor = CutNumber(lines[3], "1 ROUGE-1 Average_F:");
            resultOfTheExperiment[2] = double.Parse(valor);

            var rouge = "ROUGE-1";
            var currentLine = 5;
            while (lines[currentLine] != "---------------------------------------------")
            {
                var details = lines[currentLine].Split(' ');
                var groupName = details[3].Trim();
                var recall = TrimHeader(details[4], "R:");
                var precision = TrimHeader(details[5], "P:");
                var fmeasure = TrimHeader(details[6], "F:");
                InsertAndModifyEvaluation(mySubtotals, groupName, rouge, recall, precision, fmeasure);
                currentLine++;
            }

            currentLine++;
            valor = CutNumber(lines[currentLine++], "1 ROUGE-2 Average_R:");
            resultOfTheExperiment[3] = double.Parse(valor);

            valor = CutNumber(lines[currentLine++], "1 ROUGE-2 Average_P:");
            resultOfTheExperiment[4] = double.Parse(valor);

            valor = CutNumber(lines[currentLine++], "1 ROUGE-2 Average_F:");
            resultOfTheExperiment[5] = double.Parse(valor);

            rouge = "ROUGE-2";
            currentLine++;
            while (lines[currentLine] != "---------------------------------------------")
            {
                var details = lines[currentLine].Split(' ');
                var groupName = details[3].Trim();
                var recall = TrimHeader(details[4], "R:");
                var precision = TrimHeader(details[5], "P:");
                var fmeasure = TrimHeader(details[6], "F:");
                InsertAndModifyEvaluation(mySubtotals, groupName, rouge, recall, precision, fmeasure);
                currentLine++;
            }

            currentLine++;
            valor = CutNumber(lines[currentLine++], "1 ROUGE-L Average_R:");
            resultOfTheExperiment[6] = double.Parse(valor);

            valor = CutNumber(lines[currentLine++], "1 ROUGE-L Average_P:");
            resultOfTheExperiment[7] = double.Parse(valor);

            valor = CutNumber(lines[currentLine++], "1 ROUGE-L Average_F:");
            resultOfTheExperiment[8] = double.Parse(valor);

            rouge = "ROUGE-L";
            currentLine++;
            while (lines[currentLine] != "---------------------------------------------")
            {
                var details = lines[currentLine].Split(' ');
                var groupName = details[3].Trim();
                var recall = TrimHeader(details[4], "R:");
                var precision = TrimHeader(details[5], "P:");
                var fmeasure = TrimHeader(details[6], "F:");
                InsertAndModifyEvaluation(mySubtotals, groupName, rouge, recall, precision, fmeasure);
                currentLine++;
            }

            currentLine++;
            valor = CutNumber(lines[currentLine++], "1 ROUGE-SU4 Average_R:");
            resultOfTheExperiment[9] = double.Parse(valor);

            valor = CutNumber(lines[currentLine++], "1 ROUGE-SU4 Average_P:");
            resultOfTheExperiment[10] = double.Parse(valor);

            valor = CutNumber(lines[currentLine++], "1 ROUGE-SU4 Average_F:");
            resultOfTheExperiment[11] = double.Parse(valor);

            rouge = "ROUGE-SU4";
            currentLine++;
            while (currentLine < lines.GetUpperBound(0) + 1)
            {
                var details = lines[currentLine].Split(' ');
                var groupName = details[3].Trim();
                var recall = TrimHeader(details[4], "R:");
                var precision = TrimHeader(details[5], "P:");
                var fmeasure = TrimHeader(details[6], "F:");
                InsertAndModifyEvaluation(mySubtotals, groupName, rouge, recall, precision, fmeasure);
                currentLine++;
            }
        }

        private static void InsertAndModifyEvaluation(SubTotalsByDataSet mySubtotal, 
            string nameOfEvaluatingGroup, string rouge, string r, string p, string f)
        {
            var groupName = nameOfEvaluatingGroup;

            var recall = decimal.Parse(r);
            var precision = decimal.Parse(p);
            var fmeasure = decimal.Parse(f);
            var results = mySubtotal.EvaluatingGroup.Select("Rouge = '" + rouge + "' AND Name = '" + groupName + "'");
            if (results.GetUpperBound(0)+1 <=0)
            {
                var newValues = mySubtotal.EvaluatingGroup.NewEvaluatingGroupRow();
                newValues.Rouge = rouge;
                newValues.Name = groupName;
                newValues.Recall = recall;
                newValues.Precision = precision;
                newValues.Fmeasure = fmeasure;
                newValues.Counter = 1M;
                mySubtotal.EvaluatingGroup.AddEvaluatingGroupRow(newValues);
                mySubtotal.AcceptChanges();
                return;
            }

            var actual = (SubTotalsByDataSet.EvaluatingGroupRow) results[0];
            actual.Recall = decimal.Add(actual.Recall, recall);
            actual.Precision = decimal.Add(actual.Precision, precision);
            actual.Fmeasure = decimal.Add(actual.Fmeasure, fmeasure);
            actual.Counter = decimal.Add(actual.Counter, 1M);
            mySubtotal.AcceptChanges();
        }

        private static string CutNumber(string content, string head)
        {
            var result = content.Remove(0, head.Length);
            var pos = result.IndexOf("(", StringComparison.Ordinal);
            result = result.Substring(0, pos);
            result = result.Trim();
            result = result.Replace(".", ",");
            return result;
        }

        private static string TrimHeader(string content, string head)
        {
            var result = content.Remove(0, head.Length);
            result = result.Replace(".", ",");
            return result;
        }
    }
}