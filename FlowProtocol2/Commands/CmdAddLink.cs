namespace FlowProtocol2.Commands
{
    using System.Text.RegularExpressions;

    /// <summary>
    /// Implementiert den AddLink-Befehl
    /// </summary>
    public class CmdAddLink : CmdBaseCommand
    {
        public string Text { get; set; }
        public string Link { get; set; }

        public static CommandParser GetComandParser()
        {
            return new CommandParser("^~AddLink (.*)\\|(.*)", (rc, m) => CreateAddLinkCommand(rc, m));
        }

        private static CmdBaseCommand CreateAddLinkCommand(ReadContext rc, Match m)
        {
            CmdAddLink cmd = new CmdAddLink(rc);
            cmd.Link = m.Groups[1].Value.Trim();
            cmd.Text = m.Groups[2].Value;
            return cmd;
        }

        public CmdAddLink(ReadContext readcontext) : base(readcontext)
        {
            Text = string.Empty;
            Link = string.Empty;
        }

        public override CmdBaseCommand? Run(RunContext rc)
        {
            string linkexpanded = ReplaceVars(rc, Link);
            string textexpanded = ReplaceVars(rc, Text);
            if (string.IsNullOrWhiteSpace(textexpanded)) textexpanded = linkexpanded;
            rc.DocumentBuilder.AddNewTextElement(textexpanded, linkexpanded, false);
            return NextCommand;
        }
    }
}