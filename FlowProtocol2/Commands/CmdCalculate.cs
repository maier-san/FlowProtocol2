namespace FlowProtocol2.Commands
{
    using System.Text.RegularExpressions;

    /// <summary>
    /// Implementiert den Calculate-Befehl
    /// </summary>
    public class CmdCalculate : CmdBaseCommand
    {
        public string VarName { get; set; }
        public string Value1 { get; set; }
        public string Value2 { get; set; }
        public string Operator { get; set; }

        public static CommandParser GetComandParser()
        {
            return new CommandParser(@"^~Calculate ([A-Za-z0-9\$\(\)]*)\s*=\s*(-?[A-Za-z0-9\$\(\)\.\,]*)\s*([\+\-\*/\%])\s*(-?[A-Za-z0-9\$\(\)\.\,]*)",
                (rc, m) => CreateCalculateCommand(rc, m));
        }

        private static CmdBaseCommand CreateCalculateCommand(ReadContext rc, Match m)
        {
            CmdCalculate cmd = new CmdCalculate(rc);
            cmd.VarName = m.Groups[1].Value.Trim();
            cmd.Value1 = m.Groups[2].Value.Trim();
            cmd.Operator = m.Groups[3].Value.Trim();
            cmd.Value2 = m.Groups[4].Value.Trim();            
            return cmd;
        }

        public CmdCalculate(ReadContext readcontext) : base(readcontext)
        {
            VarName = string.Empty;
            Value1 = string.Empty;            
            Operator = string.Empty;
            Value2 = string.Empty;
        }

        public override CmdBaseCommand? Run(RunContext rc)
        {
            bool calculationOK = false;
            string expandedVarName = ReplaceVars(rc, VarName);
            string expandedValue1 = ReplaceVars(rc, Value1);
            string expandedOperator = ReplaceVars(rc, Operator);
            string expandedValue2 = ReplaceVars(rc, Value2);
            try
            {
                double result = 0;
                bool bOKValue1 = double.TryParse(expandedValue1, out double resultValue1);
                if (!bOKValue1)
                {
                    rc.SetError(ReadContext, "Ungültiger numerischer Ausdruck",
                            $"Der Ausdruck '{expandedValue1}' kann nicht als Gleitkommazahl interpretiert werden. Die Ausführung wird abgebrochen.");
                    return null;
                }
                bool bOKValue2 = double.TryParse(expandedValue2, out double resultValue2);
                if (!bOKValue2)
                {
                    rc.SetError(ReadContext, "Ungültiger numerischer Ausdruck",
                            $"Der Ausdruck '{expandedValue2}' kann nicht als Gleitkommazahl interpretiert werden. Die Ausführung wird abgebrochen.");
                    return null;
                }
                if (Operator == "+")
                {
                    result = resultValue1 + resultValue2;
                    calculationOK = true;
                }
                else if (Operator == "-")
                {
                    result = resultValue1 - resultValue2;
                    calculationOK = true;
                }
                else if (Operator == "*")
                {
                    result = resultValue1 * resultValue2;
                    calculationOK = true;
                }
                else if (Operator == "%")
                {
                    result = resultValue1 % resultValue2;
                    calculationOK = true;
                }
                else if (Operator == "/")
                {
                    if (resultValue2 != 0)
                    {
                        result = resultValue1 / resultValue2;
                        calculationOK = true;
                    }
                    else
                    {
                        rc.SetError(ReadContext, "Division durch 0",
                            $"Der Divisor '{Value2}' im Berechnungsausdruck ist 0.");
                    }
                }
                else
                {
                    rc.SetError(ReadContext, "Ungültiger Operator",
                        $"Der Operator '{Operator}' kann nicht interpretiert werden.");
                }
                if (calculationOK)
                {                    
                    rc.InternalVars[expandedVarName] = result.ToString();
                }
            }
            catch (Exception ex)
            {
                rc.SetError(ReadContext, "Verarbeitungsfehler",
                    $"Beim Ausführen des Skriptes ist ein Fehler aufgetreten '{ex.Message}'. Die Ausführung wird abgebrochen."
                    + $"Variablenwerte: expandedVarName='{expandedVarName}' expandedValue1='{expandedValue1}' expandedOperator='{expandedOperator}' expandedValue2='{expandedValue2}'");
                return null;
            }
            return NextCommand;
        }
    }
}