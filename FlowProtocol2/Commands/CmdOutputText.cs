namespace FlowProtocol2.Commands
{
    using System;
    using FlowProtocol2.Commands;

    public class CmdOutputText : CmdBaseCommand
    {
        
        public string Type {get; set;}
        public string Text {get; set;}
        public static CommandParser GetComandParser()
        {
            CommandParser cp = new CommandParser(@"^>([TI])>(.*)", rc => CreateOutputTextCommand(rc));
            return cp;
        }

        private static CmdBaseCommand CreateOutputTextCommand(ReadContext rc)
        {
            CmdOutputText cmd = new CmdOutputText(rc);
            cmd.Type = rc.ExpressionMatch.Groups[1].Value.Trim();
            cmd.Text = rc.ExpressionMatch.Groups[2].Value.Trim();
            return cmd;
        }

        public CmdOutputText(ReadContext readcontext) : base(readcontext)
        {
            Type = string.Empty;
            Text = string.Empty;
        }
        public override CmdBaseCommand? Run(ref RunContext rc)
        {
            return NextCommand;
        }
    }
}