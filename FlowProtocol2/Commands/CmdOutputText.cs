namespace FlowProtocol2.Commands
{
    using System;
    using FlowProtocol2.Commands;
    using FlowProtocol2.Core;

    public class CmdOutputText : CmdBaseCommand
    {        
        public string TypeKey {get; set;}
        public string Text {get; set;}
        public static CommandParser GetComandParser()
        {
            return new CommandParser(@"^>([L])>(.*)", rc => CreateOutputTextCommand(rc));
        }

        private static CmdBaseCommand CreateOutputTextCommand(ReadContext rc)
        {
            CmdOutputText cmd = new CmdOutputText(rc);
            cmd.TypeKey = rc.ExpressionMatch.Groups[1].Value.Trim();
            cmd.Text = rc.ExpressionMatch.Groups[2].Value.Trim();
            return cmd;
        }

        public CmdOutputText(ReadContext readcontext) : base(readcontext)
        {
            TypeKey = string.Empty;
            Text = string.Empty;
        }
        public override CmdBaseCommand? Run(ref RunContext rc)
        {
            OutputType ot = OutputType.None;
            switch (TypeKey)
            {
                case "L": ot = OutputType.Listing; break;
                case "T": ot = OutputType.FloatingText; break;
            }
            var outputtext = new OutputElement()
            {
                Type = ot,
                Text = Text                
            };            
            rc.OutputItems.Add(outputtext);
            return this.NextCommand;
        }
    }
}