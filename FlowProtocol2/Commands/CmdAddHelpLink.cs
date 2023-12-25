namespace FlowProtocol2.Commands
{
    using System.Text.RegularExpressions;
    using FlowProtocol2.Core;

    /// <summary>
    /// Implementiert den AddHelpLink-Befehl
    /// </summary>
    public class CmdAddHelpLink : CmdBaseCommand
    {
        public string Text { get; set; }
        public string Link { get; set; }

        public static CommandParser GetComandParser()
        {
            return new CommandParser("^~AddHelpLink (.*)\\|(.*)", (rc, m) => CreateAddHelpLinkCommand(rc, m));
        }

        private static CmdBaseCommand CreateAddHelpLinkCommand(ReadContext rc, Match m)
        {
            CmdAddHelpLink cmd = new CmdAddHelpLink(rc);
            cmd.Link = m.Groups[1].Value.Trim();
            cmd.Text = m.Groups[2].Value;
            return cmd;
        }

        public CmdAddHelpLink(ReadContext readcontext) : base(readcontext)
        {
            Text = string.Empty;
            Link = string.Empty;
        }

        public override CmdBaseCommand? Run(RunContext rc)
        {
            var parentInputCommand = GetPreviousCommand<CmdInputBaseCommand>(c => true, c => false);
            if (parentInputCommand == null)
            {
                rc.SetError(ReadContext, "Hilfelink ohne Eingabebefehl",
                    "Der Hilfelink-Befehl kann keinem Eingabebefehl zugeordnet werden.");
            }
            else if (parentInputCommand.AssociatedInputElement != null)
            {
                string linkexpanded = ReplaceVars(rc, Link);
                string textexpanded = ReplaceVars(rc, Text);
                if (string.IsNullOrWhiteSpace(textexpanded)) textexpanded = linkexpanded;
                parentInputCommand.AssociatedInputElement.HelpInfoBlock.AddHelpText(textexpanded, linkexpanded);
            }
            return NextCommand;
        }
    }
}