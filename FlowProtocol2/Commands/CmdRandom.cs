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
            return new CommandParser(@"^~Random ([A-Za-z0-9\$]*)\s*=\s*(-?[A-Za-z0-9\$]*)\s*..\s*(-?[A-Za-z0-9\$]*)",
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
            string rangeFromExpanded = ReplaceVars(rc, RangeFrom).Trim();
            string rangeToExpanded = ReplaceVars(rc, RangeTo).Trim();
            bool bRangeAOK = Int32.TryParse(rangeFromExpanded, out int iRangeA);
            bool bRangeBOK = Int32.TryParse(rangeToExpanded, out int iRangeB);
            if (!bRangeAOK)
            {
                rc.SetError(ReadContext, "Ungültiger numerischer Ausdruck",
                        $"Der Ausdruck '{rangeFromExpanded}' kann nicht als ganze Zahl interpretiert werden.");
            }
            else if (!bRangeBOK)
            {
                rc.SetError(ReadContext, "Ungültiger numerischer Ausdruck",
                        $"Der Ausdruck '{rangeToExpanded}' kann nicht als ganze Zahl interpretiert werden.");
            }
            else if (iRangeA > iRangeB)
            {
                rc.SetError(ReadContext, "Ungültiger Wertebereich",
                        $"Der Ausdruck '{rangeFromExpanded}..{rangeToExpanded}' beschreibt keinen gültigen Wertebereich.");
            }
            else
            {
                int rndval = new Random().Next(iRangeA, iRangeB + 1);
                string expandedVarName = ReplaceVars(rc, VarName);
                rc.InternalVars[expandedVarName] = rndval.ToString();
            }
            return NextCommand;
        }
    }
}