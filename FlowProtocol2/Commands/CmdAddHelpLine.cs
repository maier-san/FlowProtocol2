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
            return new CommandParser(@"^(>&|~AddHelpLine) (.*)", (rc, m) => CreateAddHelpLineCommand(rc, m));
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
            
            var parentInputCommand = GetPreviousCommand<CmdInputBaseCommand>(c=>true, c=>false);
            if (parentInputCommand == null)
            {
                rc.SetError(ReadContext, "Hilfetext ohne Eingabebefehl",
                    "Der Hilfetext-Befehl kann keinem Eingabebefehl zugeordnet werden.");
            }
            else if (parentInputCommand.AssociatedInputElement != null)
            {
                parentInputCommand.AssociatedInputElement.HelpInfoBlock.AddHelpLine(Text);
            }
            return NextCommand;
        }
    }
}