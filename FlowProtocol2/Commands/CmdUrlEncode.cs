namespace FlowProtocol2.Commands
{
    using System.Text.RegularExpressions;
    using System.Web;

    /// <summary>
    /// Implementiert den UrlEncode-Befehl
    /// </summary>
    public class CmdUrlEncode : CmdBaseCommand
    {
        public string VarName { get; set; }
        public string Text { get; set; }        

        public static CommandParser GetComandParser()
        {
            return new CommandParser(@"^~UrlEncode ([A-Za-z0-9$]*)\s*=(.*)", (rc, m) => CreateUrlEncodeCommand(rc, m));
        }

        private static CmdBaseCommand CreateUrlEncodeCommand(ReadContext rc, Match m)
        {
            CmdUrlEncode cmd = new CmdUrlEncode(rc);
            cmd.VarName = m.Groups[1].Value.Trim();
            cmd.Text = m.Groups[2].Value.Trim();
            return cmd;
        }

        public CmdUrlEncode(ReadContext readcontext) : base(readcontext)
        {
            VarName = string.Empty;
            Text = string.Empty;
        }

        public override CmdBaseCommand? Run(RunContext rc)
        {
            string expandedVarName = ReplaceVars(rc, VarName);
            rc.InternalVars[expandedVarName] = HttpUtility.UrlEncode(ReplaceVars(rc, Text.Trim())).Replace("+", "%20");
            return NextCommand;
        }
    }
}