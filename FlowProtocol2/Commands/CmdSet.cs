namespace FlowProtocol2.Commands
{
    using System.Text.RegularExpressions;

    /// <summary>
    /// Implementiert den Set-Befehl
    /// </summary>
    public class CmdSet : CmdBaseCommand
    {
        public string VarName { get; set; }
        public string Text { get; set; }
        
        public static CommandParser GetComandParser()
        {
            return new CommandParser(@"^~Set ([A-Za-z0-9]*)\s*=(.*)", (rc, m) => CreateSetCommand(rc, m));
        }
        
        private static CmdBaseCommand CreateSetCommand(ReadContext rc, Match m)
        {
            CmdSet cmd = new CmdSet(rc);
            cmd.VarName = m.Groups[1].Value.Trim();
            cmd.Text = m.Groups[2].Value;
            return cmd;
        }

        public CmdSet(ReadContext readcontext) : base(readcontext)
        {
            VarName = string.Empty;
            Text = string.Empty;
        }

        public override CmdBaseCommand? Run(RunContext rc)
        {
            rc.InternalVars[VarName] = ReplaceVars(rc, Text);
            return NextCommand;
        }
    }
}