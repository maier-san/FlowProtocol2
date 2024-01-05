namespace FlowProtocol2.Commands
{
    using System.Text.RegularExpressions;

    /// <summary>
    /// Implementiert den AddCode-Befehl
    /// </summary>
    public class CmdAddCode : CmdBaseCommand
    {
        public string Text { get; set; }

        public static CommandParser GetComandParser()
        {
            return new CommandParser(@"^~AddCode (.*)", (rc, m) => CreateAddCodeCommand(rc, m));
        }

        private static CmdBaseCommand CreateAddCodeCommand(ReadContext rc, Match m)
        {
            CmdAddCode cmd = new CmdAddCode(rc);
            cmd.Text = m.Groups[1].Value;
            return cmd;
        }

        public CmdAddCode(ReadContext readcontext) : base(readcontext)
        {
            Text = string.Empty;
        }

        public override CmdBaseCommand? Run(RunContext rc)
        {
            string textexpanded = ReplaceVars(rc, Text);
            rc.DocumentBuilder.AddNewTextElement(textexpanded, string.Empty, true);
            return NextCommand;
        }
    }
}