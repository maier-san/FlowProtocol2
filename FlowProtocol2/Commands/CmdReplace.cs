namespace FlowProtocol2.Commands
{
    using System.Text.RegularExpressions;

    /// <summary>
    /// Implementiert den Replace-Befehl
    /// </summary>
    public class CmdReplace : CmdBaseCommand
    {
        public string VarName { get; set; }
        public string Text { get; set; }
        public string SearchFor { get; set; }
        public string ReplaceBy { get; set; }

        public static CommandParser GetComandParser()
        {
            return new CommandParser(@"^~Replace ([A-Za-z0-9]*)\s*=(.*)\|(.*)->(.*)", (rc, m) => CreateReplaceCommand(rc, m));
        }

        private static CmdBaseCommand CreateReplaceCommand(ReadContext rc, Match m)
        {
            CmdReplace cmd = new CmdReplace(rc);
            cmd.VarName = m.Groups[1].Value.Trim();
            cmd.Text = m.Groups[2].Value.Trim();
            cmd.SearchFor = m.Groups[3].Value;
            cmd.ReplaceBy = m.Groups[4].Value;
            return cmd;
        }

        public CmdReplace(ReadContext readcontext) : base(readcontext)
        {
            VarName = string.Empty;
            Text = string.Empty;
            SearchFor = string.Empty;
            ReplaceBy = string.Empty;
        }

        public override CmdBaseCommand? Run(RunContext rc)
        {
            rc.InternalVars[VarName] = ReplaceVars(rc, Text).Replace(ReplaceVars(rc, SearchFor), ReplaceVars(rc, ReplaceBy));
            return NextCommand;
        }
    }
}