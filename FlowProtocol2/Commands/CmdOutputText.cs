namespace FlowProtocol2.Commands
{
    using FlowProtocol2.Core;

    public class CmdOutputText : CmdBaseCommand
    {
        public string LevelKey { get; set; }
        public string TypeKey { get; set; }
        public string Text { get; set; }
        public static CommandParser GetComandParser()
        {
            return new CommandParser(@"^>(>?)([_\*|#])(.*)", rc => CreateOutputTextCommand(rc));
        }

        private static CmdBaseCommand CreateOutputTextCommand(ReadContext rc)
        {
            CmdOutputText cmd = new CmdOutputText(rc);
            cmd.LevelKey = rc.ExpressionMatch.Groups[1].Value.Trim();
            cmd.TypeKey = rc.ExpressionMatch.Groups[2].Value.Trim();
            cmd.Text = rc.ExpressionMatch.Groups[3].Value;
            return cmd;
        }

        public CmdOutputText(ReadContext readcontext) : base(readcontext)
        {
            LevelKey = ">";
            TypeKey = string.Empty;
            Text = string.Empty;
        }
        
        public override CmdBaseCommand? Run(RunContext rc)
        {
            Level l = Level.Level1;
            if (string.IsNullOrEmpty(LevelKey))
            {
                l = Level.Level2;
            }
            OutputType ot = OutputType.None;
            switch (TypeKey)
            {
                case "*": ot = OutputType.Listing; break;
                case "#": ot = OutputType.Enumeration; break;
                case "|": ot = OutputType.Code; break;
                case "_": ot = OutputType.Paragraph; break;
            }
            if (ot != OutputType.Code) Text = Text.Trim(); else Text = Text.TrimEnd();
            string texex = ReplaceVars(rc, Text);
            rc.DocumentBuilder.AddNewTextLine(l, ot, texex);
            return this.NextCommand;
        }
    }
}