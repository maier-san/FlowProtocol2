namespace FlowProtocol2.Commands
{
    using System.Text.RegularExpressions;

    /// <summary>
    /// Implementiert den OptionValue-Befehl
    /// </summary>
    public class CmdOptionValue : CmdBaseCommand
    {
        public string Key { get; set; }
        public string Prompt { get; set; }
        public CmdOptionGroup? ParentOptionGroupCommand { get; set; }

        public static CommandParser GetComandParser()
        {
            return new CommandParser(@"^#([A-Za-z0-9]*)\s*:(.*)", (rc, m) => CreateOptionValueCommand(rc, m));
        }

        private static CmdBaseCommand CreateOptionValueCommand(ReadContext rc, Match m)
        {
            CmdOptionValue cmd = new CmdOptionValue(rc);
            cmd.Key = m.Groups[1].Value.Trim();
            cmd.Prompt = m.Groups[2].Value.Trim();
            return cmd;
        }

        public CmdOptionValue(ReadContext readcontext) : base(readcontext)
        {
            Key = string.Empty;
            Prompt = string.Empty;
        }

        public override CmdBaseCommand? Run(RunContext rc)
        {
            try
            {
                if (ParentOptionGroupCommand == null)
                {
                    rc.SetError(ReadContext, "Optionswert ohne Optionsgruppe",
                        "Dem Optionswert konnte keine übergeordnete Gruppe zugeordnet werden.");
                    return GetNextSameOrHigherLevelCommand();
                }
                if (ParentOptionGroupCommand.SelectedOptionCommand == this)
                {
                    return NextCommand;
                }
            }
            catch (Exception ex)
            {
                rc.SetError(ReadContext, "Verarbeitungsfehler",
                    $"Beim Ausführen des Skriptes ist ein Fehler aufgetreten '{ex.Message}'. Die Ausführung wird abgebrochen."
                    + $"Variablenwerte: Key='{Key}' Prompt='{Prompt}'");
                return null;
            }
            return GetNextSameOrHigherLevelCommand();
        }
    }
}