namespace FlowProtocol2.Commands
{
    using System.Text.RegularExpressions;

    /// <summary>
    /// Implementiert den Round-Befehl
    /// </summary>
    public class CmdRound : CmdBaseCommand
    {
        public string VarName { get; set; }
        public string Value { get; set; }
        public string Precision { get; set; }

        public static CommandParser GetComandParser()
        {
            return new CommandParser(@"^~Round\s+([A-Za-z0-9\$\(\)]+)\s*=\s*([^|]+)\|([A-Za-z0-9\$\(\)]+)", (rc, m) => CreateRoundCommand(rc, m));
        }

        private static CmdBaseCommand CreateRoundCommand(ReadContext rc, Match m)
        {
            CmdRound cmd = new CmdRound(rc);
            cmd.VarName = m.Groups[1].Value.Trim();
            cmd.Value = m.Groups[2].Value.Trim();
            cmd.Precision = m.Groups[3].Value.Trim();
            return cmd;
        }

        public CmdRound(ReadContext readcontext) : base(readcontext)
        {
            VarName = string.Empty;
            Value = string.Empty;
            Precision = string.Empty;
        }

        public override CmdBaseCommand? Run(RunContext rc)
        {
            string expandedVarName = ReplaceVars(rc, VarName);
            string expandedValue = ReplaceVars(rc, Value);
            string expandedPrecision = ReplaceVars(rc, Precision);
            try
            {
                double result = 0;
                bool bOKValue = double.TryParse(expandedValue, out double resultValue);
                if (!bOKValue)
                {
                    rc.SetError(ReadContext, "Ungültiger numerischer Ausdruck",
                            $"Der Ausdruck '{expandedValue}' kann nicht als Gleitkommazahl interpretiert werden. Die Ausführung wird abgebrochen.");
                    return null;
                }
                bool bOKPrecision = Int32.TryParse(expandedPrecision, out int resultPrecision);
                if (!bOKPrecision || resultPrecision < 0)
                {
                    rc.SetError(ReadContext, "Ungültiger numerischer Ausdruck",
                        $"Der Ausdruck '{expandedPrecision}' kann nicht als ganze Zahl interpretiert werden. Die Ausführung wird abgebrochen.");
                    return null;
                }
                result = Math.Round(resultValue, resultPrecision, MidpointRounding.AwayFromZero);
                rc.InternalVars[expandedVarName] = result.ToString();
            }
            catch (Exception ex)
            {
                rc.SetError(ReadContext, "Verarbeitungsfehler",
                    $"Beim Ausführen des Skriptes ist ein Fehler aufgetreten '{ex.Message}'. Die Ausführung wird abgebrochen."
                    + $"Variablenwerte: expandedVarName='{expandedVarName}' expandedValue='{expandedValue}' expandedPrecision='{expandedPrecision}'");
                return null;
            }
            return NextCommand;
        }
    }
}