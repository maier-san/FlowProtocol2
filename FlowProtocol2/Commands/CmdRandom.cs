namespace FlowProtocol2.Commands
{
    using System.Text.RegularExpressions;

    /// <summary>
    /// Implementiert den Random-Befehl
    /// </summary>
    public class CmdRandom : CmdBaseCommand
    {
        public string VarName { get; set; }
        public string RangeFrom { get; set; }
        public string RangeTo { get; set; }

        public static CommandParser GetComandParser()
        {
            return new CommandParser(@"^~Random ([A-Za-z0-9\$\(\)]*)\s*=\s*(-?[A-Za-z0-9\$\(\)]*)\s*..\s*(-?[A-Za-z0-9\$\(\)]*)",
                (rc, m) => CreateRandomCommand(rc, m));
        }

        private static CmdBaseCommand CreateRandomCommand(ReadContext rc, Match m)
        {
            CmdRandom cmd = new CmdRandom(rc);
            cmd.VarName = m.Groups[1].Value.Trim();
            cmd.RangeFrom = m.Groups[2].Value.Trim();
            cmd.RangeTo = m.Groups[3].Value.Trim();
            return cmd;
        }

        public CmdRandom(ReadContext readcontext) : base(readcontext)
        {
            VarName = string.Empty;
            RangeFrom = string.Empty;
            RangeTo = string.Empty;
        }

        public override CmdBaseCommand? Run(RunContext rc)
        {
            string expandedVarName = ReplaceVars(rc, VarName);
            string expandedRangeFrom = ReplaceVars(rc, RangeFrom);
            string expandedRangeTo = ReplaceVars(rc, RangeTo);
            try
            {
                bool bRangeAOK = Int32.TryParse(expandedRangeFrom, out int iRangeA);                
                if (!bRangeAOK)
                {
                    rc.SetError(ReadContext, "Ungültiger numerischer Ausdruck",
                            $"Der Ausdruck '{expandedRangeFrom}' kann nicht als ganze Zahl interpretiert werden. Die Ausführung wird abgebrochen.");
                    return null;
                }
                bool bRangeBOK = Int32.TryParse(expandedRangeTo, out int iRangeB);
                if (!bRangeBOK)
                {
                    rc.SetError(ReadContext, "Ungültiger numerischer Ausdruck",
                            $"Der Ausdruck '{expandedRangeTo}' kann nicht als ganze Zahl interpretiert werden. Die Ausführung wird abgebrochen.");
                    return null;
                }
                if (iRangeA > iRangeB)
                {
                    rc.SetError(ReadContext, "Ungültiger Wertebereich",
                            $"Der Ausdruck '{expandedRangeFrom}..{expandedRangeTo}' beschreibt keinen gültigen Wertebereich. Die Ausführung wird abgebrochen.");
                    return null;
                }                
                int rndval = new Random().Next(iRangeA, iRangeB + 1);                    
                rc.InternalVars[expandedVarName] = rndval.ToString();                
            }
            catch (Exception ex)
            {
                rc.SetError(ReadContext, "Verarbeitungsfehler",
                    $"Beim Ausführen des Skriptes ist ein Fehler aufgetreten '{ex.Message}'. Die Ausführung wird abgebrochen."
                    + $"Variablenwerte: expandedVarName='{expandedVarName}' expandedRangeFrom='{expandedRangeFrom}' expandedRangeTo='{expandedRangeTo}'");
                return null;
            }
            return NextCommand;
        }
    }
}