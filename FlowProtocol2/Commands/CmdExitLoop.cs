namespace FlowProtocol2.Commands
{
    using System.Text.RegularExpressions;
    using FlowProtocol2.Core;

    /// <summary>
    /// Implementiert den ExitLoop-Befehl
    /// </summary>
    public class CmdExitLoop : CmdBaseCommand
    {
        public CmdLoop? AssociatedLoopCommand { get; set; }
        public static CommandParser GetComandParser()
        {
            return new CommandParser(@"^~ExitLoop", (rc, m) => CreateExitLoopCommand(rc, m));
        }

        private static CmdBaseCommand CreateExitLoopCommand(ReadContext rc, Match m)
        {
            CmdExitLoop cmd = new CmdExitLoop(rc);
            return cmd;
        }

        public CmdExitLoop(ReadContext readcontext) : base(readcontext)
        {
            AssociatedLoopCommand = null;
        }

        public override CmdBaseCommand? Run(RunContext rc)
        {
            try
            {
                GetAssociatedLoopCommand(rc);
                if (AssociatedLoopCommand != null)
                {                                
                    return AssociatedLoopCommand.NextCommand;                
                }
                rc.SetError(ReadContext, "ExitLoop ohne Loop",
                    "Dem ExitLoop-Befehl kann kein Loop-Befehl auf gleicher Ebene zugeordnet werden. Die Bearbeitung wird abgebrochen.");
                return null;
            }
            catch (Exception ex)
            {
                rc.SetError(ReadContext, "Verarbeitungfehler",
                    $"Beim Ausführen des Skriptes ist ein Fehler aufgetreten '{ex.Message}'. Die Ausführung wird abgebrochen.");
                return null;
            }
        }

        private void GetAssociatedLoopCommand(RunContext rc)
        {
            if (AssociatedLoopCommand == null)
            {
                AssociatedLoopCommand = GetNextCommand<CmdLoop>(
                    c => true,
                    c => false);
            }            
        } 
    }
}