namespace FlowProtocol2.Commands
{
    using System.Text.RegularExpressions;
    using FlowProtocol2.Core;

    /// <summary>
    /// Implementiert den Calculate-Befehl
    /// </summary>
    public class CmdCalculate : CmdBaseCommand
    {
        public string VarName { get; set; }
        public string Value1 { get; set; }
        public string Value2 { get; set; }
        public string Operator { get; set; }
        // ToDo: Weitere Eigenschaften deklarieren

        public static CommandParser GetComandParser()
        {
            return new CommandParser(@"^~Calculate ([A-Za-z0-9]*)\s*=\s*(-?[A-Za-z0-9$]*)\s*([\+\-\*/])\s*(-?[A-Za-z0-9$]*)",
                (rc, m) => CreateCalculateCommand(rc, m));
        }

        private static CmdBaseCommand CreateCalculateCommand(ReadContext rc, Match m)
        {
            CmdCalculate cmd = new CmdCalculate(rc);
            cmd.VarName = m.Groups[1].Value.Trim();
            cmd.Value1 = m.Groups[2].Value.Trim();
            cmd.Value2 = m.Groups[4].Value.Trim();
            cmd.Operator = m.Groups[3].Value.Trim();
            return cmd;
        }

        public CmdCalculate(ReadContext readcontext) : base(readcontext)
        {
            VarName = string.Empty;
            Value1 = string.Empty;
            Value2 = string.Empty;
            Operator = string.Empty;
        }

        public override CmdBaseCommand? Run(RunContext rc)
        {
            bool CalculationOK = false;
            bool w1OK = double.TryParse(ReplaceVars(rc, Value1), out double dblValue1);
            bool w2OK = double.TryParse(ReplaceVars(rc, Value2), out double dblValue2);
            double result = 0;
            if (!w1OK)
            {
                rc.SetError(ReadContext, "Ungültiger numerischer Ausdruck",
                        $"Der Ausdruck {dblValue1} kann nicht als Gleitkommazahl interpretiert werden.");
            }
            else if (!w2OK)
            {
                rc.SetError(ReadContext, "Ungültiger numerischer Ausdruck",
                        $"Der Ausdruck {dblValue2} kann nicht als Gleitkommazahl interpretiert werden.");
            }
            else if (Operator == "+")
            {
                result = dblValue1 + dblValue2;
                CalculationOK = true;
            }
            else if (Operator == "-")
            {
                result = dblValue1 - dblValue2;
                CalculationOK = true;
            }
            else if (Operator == "*")
            {
                result = dblValue1 * dblValue2;
                CalculationOK = true;
            }
            else if (Operator == "/")
            {
                if (dblValue2 != 0)
                {
                    result = dblValue1 / dblValue2;
                    CalculationOK = true;
                }
                else
                {
                    rc.SetError(ReadContext, "Division durch 0",
                        $"Der Divisor {Value2} im Berechnungsausdruck ist 0.");
                }
            }
            else
            {
                rc.SetError(ReadContext, "Ungültiger Operator",
                        $"Der Operator {Operator} kann nicht interpretiert werden.");
            }
            if (CalculationOK)
            {
                rc.InternalVars[VarName] = result.ToString();
            }
            return NextCommand;
        }
    }
}