namespace FlowProtocol2.Commands
{
    using System.Text.RegularExpressions;
    using FlowProtocol2.Core;

    /// <summary>
    /// Implementiert den GoTo-Befehl
    /// </summary>
    /// <remarks>
    /// Erstellt mit NewCC.fp2, Eingabe: ~GoTo (vJumpMark)
    /// </remarks>
    public class CmdGoTo : CmdBaseCommand
    {
        public string JumpMark { get; set; }

        public static CommandParser GetComandParser()
        {
            return new CommandParser(@"^~GoTo\s+([A-Za-z0-9\$\(\)]+)",
                                     (rc, m) => CreateGoToCommand(rc, m));
        }

        private static CmdBaseCommand CreateGoToCommand(ReadContext rc, Match m)
        {
            CmdGoTo cmd = new CmdGoTo(rc);
            cmd.JumpMark = m.Groups[1].Value.Trim();
            return cmd;
        }

        public CmdGoTo(ReadContext readcontext) : base(readcontext)
        {
            JumpMark = string.Empty;
        }

        public override CmdBaseCommand? Run(RunContext rc)
        {
            string expandedJumpMark = ReplaceVars(rc, JumpMark);
            try
            {
                CmdJumpMark? jump = GetFirstCommand<CmdJumpMark>(rc, c => c.Mark == expandedJumpMark, c => false);
                if (jump == null)
                {
                    rc.SetError(ReadContext, "Sprungziel nicht gefunden",
                        $"Das Sprungziel '{expandedJumpMark}' konnte nicht gefunden werden. Die Skriptausführung wird abgebrochen.");
                    return null;
                }
                return jump;
            }
            catch (Exception ex)
            {
                rc.SetError(ReadContext, "Verarbeitungsfehler",
                    $"Beim Ausführen des Skriptes ist ein Fehler aufgetreten '{ex.Message}'. Die Ausführung wird abgebrochen."
                    + $"Variablenwerte: expandedJumpMark='{expandedJumpMark}'");
                return null;
            }
        }
    }
}