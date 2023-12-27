namespace FlowProtocol2.Commands
{
    using System.Text.RegularExpressions;
    using FlowProtocol2.Core;

    public class CmdOutputText : CmdBaseCommand
    {
        public string LevelKey { get; set; }
        public string TypeKey { get; set; }
        public string Text { get; set; }
        public static CommandParser GetComandParser()
        {
            return new CommandParser(@"^>(>?)([_\*\|\#\s])(.*)", (rc, m) => CreateOutputTextCommand(rc, m));
        }

        private static CmdBaseCommand CreateOutputTextCommand(ReadContext rc, Match m)
        {
            CmdOutputText cmd = new CmdOutputText(rc);
            cmd.LevelKey = m.Groups[1].Value.Trim();
            cmd.TypeKey = m.Groups[2].Value.Trim();
            cmd.Text = m.Groups[3].Value;
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
            OutputType ot = OutputType.Enumeration;
            if (string.IsNullOrEmpty(LevelKey))
            {
                l = Level.Level2;
                ot = OutputType.Listing;
            }
            switch (TypeKey)
            {
                case "*": ot = OutputType.Listing; break;
                case "#": ot = OutputType.Enumeration; break;
                case "|": ot = OutputType.Code; break;
                case "_": ot = OutputType.Paragraph; break;
            }
            string expandedtext = ReplaceVars(rc, Text);
            if (ot != OutputType.Code) expandedtext = expandedtext.Trim(); else expandedtext = expandedtext.TrimEnd();
            rc.DocumentBuilder.AddNewTextLine(l, ot, expandedtext);
            return this.NextCommand;
        }
    }
}