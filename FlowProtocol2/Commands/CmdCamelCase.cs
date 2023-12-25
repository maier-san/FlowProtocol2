namespace FlowProtocol2.Commands
{
    using System.Text.RegularExpressions;

    /// <summary>
    /// Implementiert den CamelCase-Befehl
    /// </summary>
    public class CmdCamelCase : CmdBaseCommand
    {
        public string VarName { get; set; }
        public string Text { get; set; }

        public static CommandParser GetComandParser()
        {
            return new CommandParser(@"^~CamelCase ([A-Za-z0-9$]*)\s*=(.*)", (rc, m) => CreateCamelCaseCommand(rc, m));
        }

        private static CmdBaseCommand CreateCamelCaseCommand(ReadContext rc, Match m)
        {
            CmdCamelCase cmd = new CmdCamelCase(rc);
            cmd.VarName = m.Groups[1].Value.Trim();
            cmd.Text = m.Groups[2].Value.Trim();
            return cmd;
        }

        public CmdCamelCase(ReadContext readcontext) : base(readcontext)
        {
            VarName = string.Empty;
            Text = string.Empty;
        }

        public override CmdBaseCommand? Run(RunContext rc)
        {
            string ccwert = ReplaceVars(rc, Text.Trim());
            ccwert = ccwert.Replace("ä", "ae", false, null)
                           .Replace("ö", "oe", false, null)
                           .Replace("ü", "ue", false, null)
                           .Replace("Ä", "Ae", false, null)
                           .Replace("Ö", "Oe", false, null)
                           .Replace("Ü", "Ue", false, null)
                           .Replace("ß", "ss", false, null);
            ccwert = Regex.Replace(ccwert, @"[^\w]", "_");
            ccwert = Regex.Replace(ccwert, @"__*", "_");
            while (ccwert.Contains('_'))
            {
                int pos = ccwert.IndexOf('_');
                if (pos + 2 < ccwert.Length)
                {
                    ccwert = ccwert.Substring(0, pos) + ccwert.Substring(pos + 1, 1).ToUpper() + ccwert.Substring(pos + 2);
                }
                else
                {
                    ccwert = ccwert.Replace("_", "");
                }
            }
            string expandedVarName = ReplaceVars(rc, VarName);
            rc.InternalVars[expandedVarName] = ccwert;
            return NextCommand;
        }
    }
}