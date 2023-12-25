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
            return new CommandParser(@"^~Round ([A-Za-z0-9$]*)\s*=(.*)\|\s*([A-Za-z0-9$]*)", (rc, m) => CreateRoundCommand(rc, m));
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
            string valExpanded = ReplaceVars(rc, Value).Trim();
            string precisionExpanded = ReplaceVars(rc, Precision).Trim();
            bool w1OK = double.TryParse(valExpanded, out double dblValue);            
            bool PrecisionOK = Int32.TryParse(precisionExpanded, out int iPrecision);
            double result = 0;
            if (!w1OK)
            {
                rc.SetError(ReadContext, "Ungültiger numerischer Ausdruck",
                        $"Der Ausdruck {valExpanded} kann nicht als Gleitkommazahl interpretiert werden.");
            }
            else if (!PrecisionOK || iPrecision < 0)
            {
                rc.SetError(ReadContext, "Ungültige Rundungsgenauigkeit",
                        $"Der Ausdruck {precisionExpanded} kann nicht als Gleitkommazahl interpretiert werden.");
            }
            else
            {
                result = Math.Round(dblValue, iPrecision, MidpointRounding.AwayFromZero);
                string expandedVarName = ReplaceVars(rc, VarName);
                rc.InternalVars[expandedVarName] = result.ToString();
            }
            return NextCommand;
        }
    }
}