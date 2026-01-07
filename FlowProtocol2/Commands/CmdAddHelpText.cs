namespace FlowProtocol2.Commands
{
    using System.Text.RegularExpressions;
    using FlowProtocol2.Core;

    /// <summary>
    /// Implementiert den AddHelpText-Befehl
    /// </summary>
    public class CmdAddHelpText : CmdBaseCommand
    {        
        public string Text { get; set; }

        public static CommandParser GetComandParser()
        {
            return new CommandParser(@"^~AddHelpText (.*)", (rc, m) => CreateAddHelpTextCommand(rc, m));
        }

        private static CmdBaseCommand CreateAddHelpTextCommand(ReadContext rc, Match m)
        {
            CmdAddHelpText cmd = new CmdAddHelpText(rc);
            cmd.Text = m.Groups[1].Value;
            return cmd;
        }

        public CmdAddHelpText(ReadContext readcontext) : base(readcontext)
        {
            Text = string.Empty;
        }

        public override CmdBaseCommand? Run(RunContext rc)
        {
            var parentInputCommand = GetPreviousCommand<CmdInputBaseCommand>(c => true, c => false);
            if (parentInputCommand == null)
            {
                rc.SetError(ReadContext, "Hilfetext ohne Eingabebefehl",
                    "Der Hilfetext-Befehl kann keinem Eingabebefehl zugeordnet werden.");
            }
            else if (parentInputCommand.AssociatedInputElement != null)
            {
                string expandedText = ReplaceVars(rc, Text);
                try
                {
                    parentInputCommand.AssociatedInputElement.HelpInfoBlock.AddHelpText(expandedText, string.Empty, false);
                }
                catch (Exception ex)
                {
                    rc.SetError(ReadContext, "Verarbeitungsfehler",
                        $"Beim Ausführen des Skriptes ist ein Fehler aufgetreten '{ex.Message}'. Die Ausführung wird abgebrochen."
                        + $"Variablenwerte: expandedText='{expandedText}'");
                    return null;
                }                
            }
            return NextCommand;
        }
    }
}