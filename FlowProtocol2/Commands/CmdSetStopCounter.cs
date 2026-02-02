namespace FlowProtocol2.Commands
{
    using System.Globalization;
    using System.Text.RegularExpressions;
    using FlowProtocol2.Core;

    /// <summary>
    /// Implementiert den SetStopCounter-Befehl
    /// </summary>
    /// <remarks>
    /// Erstellt mit NewCC.fp2, Eingabe: ~SetStopCounter (iLoopStop) ; (iCommandStop)[; MaxLength = (iMaxReplaceLength)]
    /// </remarks>
    public class CmdSetStopCounter : CmdBaseCommand
    {
        public string LoopStop { get; set; }
        public string CommandStop { get; set; }
        public string MaxReplaceLength { get; set; }

        public static CommandParser GetComandParser()
        {
            return new CommandParser(@"^~SetStopCounter\s+([A-Za-z0-9\$\(\)]+)\s*;\s*([A-Za-z0-9\$\(\)]+)(\s*;\s*MaxLength\s*=\s*([A-Za-z0-9\$\(\)]+))?",
                                     (rc, m) => CreateSetStopCounterCommand(rc, m));
        }

        private static CmdBaseCommand CreateSetStopCounterCommand(ReadContext rc, Match m)
        {
            CmdSetStopCounter cmd = new CmdSetStopCounter(rc);
            cmd.LoopStop = m.Groups[1].Value.Trim();
            cmd.CommandStop = m.Groups[2].Value.Trim();
            cmd.MaxReplaceLength = m.Groups[4].Value.Trim();
            return cmd;
        }

        public CmdSetStopCounter(ReadContext readcontext) : base(readcontext)
        {
            LoopStop = string.Empty;
            CommandStop = string.Empty;
            MaxReplaceLength = string.Empty;
        }

        public override CmdBaseCommand? Run(RunContext rc)
        {
            string expandedLoopStop = ReplaceVars(rc, LoopStop);
            string expandedCommandStop = ReplaceVars(rc, CommandStop);
            string expandedMaxReplaceLength = ReplaceVars(rc, MaxReplaceLength);
            try
            {
                bool bOKLoopStop = Int32.TryParse(expandedLoopStop, out int resultLoopStop);
                if (!bOKLoopStop)
                {
                    rc.SetError(ReadContext, "Ungültiger numerischer Ausdruck",
                        $"Der Ausdruck '{expandedLoopStop}' kann nicht als ganze Zahl interpretiert werden. Die Ausführung wird abgebrochen.");
                    return null;
                }
                bool bOKCommandStop = Int32.TryParse(expandedCommandStop, out int resultCommandStop);
                if (!bOKCommandStop)
                {
                    rc.SetError(ReadContext, "Ungültiger numerischer Ausdruck",
                        $"Der Ausdruck '{expandedCommandStop}' kann nicht als ganze Zahl interpretiert werden. Die Ausführung wird abgebrochen.");
                    return null;
                }
                if (string.IsNullOrWhiteSpace(expandedMaxReplaceLength))
                {
                    // Optionales Argument mit Variable MaxReplaceLength wurde weggelassen
                    expandedMaxReplaceLength = rc.MaxReplaceLength.ToString();
                }
                bool bOKMaxReplaceLength = Int32.TryParse(expandedMaxReplaceLength, out int resultMaxReplaceLength);
                if (!bOKMaxReplaceLength)
                {
                    rc.SetError(ReadContext, "Ungültiger numerischer Ausdruck",
                        $"Der Ausdruck '{expandedMaxReplaceLength}' kann nicht als ganze Zahl interpretiert werden. Die Ausführung wird abgebrochen.");
                    return null;
                }
                rc.CommandStopCounter = resultCommandStop;
                rc.LoopStopCounter = resultLoopStop;
                rc.MaxReplaceLength = resultMaxReplaceLength;
            }
            catch (Exception ex)
            {
                rc.SetError(ReadContext, "Verarbeitungsfehler",
                    $"Beim Ausführen des Skriptes ist ein Fehler aufgetreten '{ex.Message}'. Die Ausführung wird abgebrochen."
                    + $"Variablenwerte: expandedLoopStop='{expandedLoopStop}' expandedCommandStop='{expandedCommandStop}' expandedMaxReplaceLength='{expandedMaxReplaceLength}'");
                return null;
            }
            return NextCommand;
        }
    }
}