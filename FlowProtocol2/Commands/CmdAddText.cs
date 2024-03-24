namespace FlowProtocol2.Commands
{
    using System.Text.RegularExpressions;

    /// <summary>
    /// Implementiert den AddText-Befehl
    /// </summary>
    public class CmdAddText : CmdBaseCommand
    {
        public string Text { get; set; }

        public static CommandParser GetComandParser()
        {
            return new CommandParser(@"^~AddText (.*)", (rc, m) => CreateAddTextCommand(rc, m));
        }

        private static CmdBaseCommand CreateAddTextCommand(ReadContext rc, Match m)
        {
            CmdAddText cmd = new CmdAddText(rc);
            cmd.Text = m.Groups[1].Value;
            return cmd;
        }

        public CmdAddText(ReadContext readcontext) : base(readcontext)
        {
            Text = string.Empty;
        }

        public override CmdBaseCommand? Run(RunContext rc)
        {
            rc.DocumentBuilder.AddNewTextElement(ReplaceVars(rc, Text), string.Empty, false, false);
            return NextCommand;
        }
    }
}