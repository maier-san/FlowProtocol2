namespace FlowProtocol2.Commands
{
    using System.Text.RegularExpressions;

    /// <summary>
    /// Implementiert den AddHelpLine-Befehl
    /// </summary>
    public class CmdAddHelpLine : CmdBaseCommand
    {
        public string Text { get; set; }

        public static CommandParser GetComandParser()
        {
            return new CommandParser(@"^(&>|~AddHelpLine)(.*)", (rc, m) => CreateAddHelpLineCommand(rc, m));
        }

        private static CmdBaseCommand CreateAddHelpLineCommand(ReadContext rc, Match m)
        {
            CmdAddHelpLine cmd = new CmdAddHelpLine(rc);
            cmd.Text = m.Groups[2].Value.Trim();
            return cmd;
        }

        public CmdAddHelpLine(ReadContext readcontext) : base(readcontext)
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
            else if (parentInputCommand.AssociatedInputElements.ContainsKey(rc.BaseKey) && parentInputCommand.AssociatedInputElements[rc.BaseKey] != null)
            {                
                string expandedText = ReplaceVars(rc, Text);
                try
                {
                    parentInputCommand.AssociatedInputElements[rc.BaseKey]?.HelpInfoBlock.AddHelpLine(expandedText);
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