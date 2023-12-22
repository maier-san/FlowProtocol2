namespace FlowProtocol2.Commands
{
    using FlowProtocol2.Core;
    public class CmdSet : CmdBaseCommand
    {
        public string VarName { get; set; }
        public string Text {get; set;}
        public static CommandParser GetComandParser()
        {
            return new CommandParser(@"^~Set ([A-Za-z0-9]*)\s*=(.*)", rc => CreateSetCommand(rc));
        }
        private static CmdBaseCommand CreateSetCommand(ReadContext rc)
        {
            CmdSet cmd = new CmdSet(rc);
            cmd.VarName = rc.ExpressionMatch.Groups[1].Value.Trim();
            cmd.Text = rc.ExpressionMatch.Groups[2].Value;
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