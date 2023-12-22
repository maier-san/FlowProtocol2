namespace FlowProtocol2.Commands
{
    using System.Text.RegularExpressions;
    using FlowProtocol2.Core;

    /// <summary>
    /// Implementiert den OptionValue-Befehl
    /// </summary>
    public class CmdOptionValue : CmdBaseCommand
    {
        public string Key { get; set; }
        public string Promt { get; set; }
        public CmdOptionGroup? ParentOptionGroupCommand { get; set; }

        public static CommandParser GetComandParser()
        {
            return new CommandParser(@"^#([A-Za-z0-9]*)\s*:(.*)", (rc, m) => CreateOptionValueCommand(rc, m));
        }

        private static CmdBaseCommand CreateOptionValueCommand(ReadContext rc, Match m)
        {
            CmdOptionValue cmd = new CmdOptionValue(rc);
            cmd.Key = m.Groups[1].Value.Trim();
            cmd.Promt = m.Groups[2].Value.Trim();
            return cmd;
        }

        public CmdOptionValue(ReadContext readcontext) : base(readcontext)
        {
            Key = string.Empty;
            Promt = string.Empty;
        }

        public override CmdBaseCommand? Run(RunContext rc)
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
            return GetNextSameOrHigherLevelCommand();
        }
    }
}