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
            return new CommandParser(@"^~AddLink\s+(.*)\|(.*)", (rc, m) => CreateAddLinkCommand(rc, m));
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
            string expandedLink = ReplaceVars(rc, Link);
            string expandedText = ReplaceVars(rc, Text);
            try
            {
                bool isonwhitelist = rc.IsOnWhitelist(expandedLink);
                if (string.IsNullOrWhiteSpace(expandedText)) expandedText = expandedLink;
                rc.DocumentBuilder.AddNewTextElement(expandedText, expandedLink, false, isonwhitelist);
            }
            catch (Exception ex)
            {
                rc.SetError(ReadContext, "Verarbeitungsfehler",
                    $"Beim Ausführen des Skriptes ist ein Fehler aufgetreten '{ex.Message}'. Die Ausführung wird abgebrochen."
                    + $"Variablenwerte: expandedLink='{expandedLink}' expandedText='{expandedText}'");
                return null;
            }
            return NextCommand;
        }
    }
}