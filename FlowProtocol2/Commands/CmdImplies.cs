namespace FlowProtocol2.Commands
{
    using System.Text.RegularExpressions;

    /// <summary>
    /// Implementiert den Implies-Befehl
    /// </summary>
    public class CmdImplies : CmdBaseCommand
    {
        public string Key { get; set; }
        public string Text { get; set; }

        public static CommandParser GetComandParser()
        {
            return new CommandParser(@"^~Implies ([A-Za-z0-9\$]*)\s*=(.*)", (rc, m) => CreateImpliesCommand(rc, m));
        }

        private static CmdBaseCommand CreateImpliesCommand(ReadContext rc, Match m)
        {
            CmdImplies cmd = new CmdImplies(rc);
            cmd.Key = m.Groups[1].Value.Trim();
            cmd.Text = m.Groups[2].Value.Trim();
            return cmd;
        }

        public CmdImplies(ReadContext readcontext) : base(readcontext)
        {
            Key = string.Empty;
            Text = string.Empty;
        }

        public override CmdBaseCommand? Run(RunContext rc)
        {
            string expandedKey = ReplaceVars(rc, Key).Trim();
            if (!string.IsNullOrEmpty(rc.BaseKey))
            {
                expandedKey = rc.BaseKey + "_" + expandedKey;
            }
            rc.BoundVars[expandedKey] = ReplaceVars(rc, Text);
            rc.GivenKeys.Add(expandedKey);
            return NextCommand;
        }
    }
}