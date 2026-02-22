namespace FlowProtocol2.Commands
{
    using System.Globalization;
    using System.Text.RegularExpressions;
    using FlowProtocol2.Core;

    /// <summary>
    /// Implementiert den For-Befehl
    /// </summary>
    /// <remarks>
    /// Erstellt mit NewCC.fp2, Eingabe: ~For (vVarName) = (iVon) To (iBis) [; Step (iStep)]
    /// </remarks>
    public class CmdFor : CmdLoopBaseCommand
    {
        public string VarName { get; set; }
        public string Von { get; set; }
        public string Bis { get; set; }
        public string Step { get; set; }
        private Dictionary<string, int> ForIndices { get; set; }
        private Dictionary<string, int> StepValues { get; set; }

        public static CommandParser GetComandParser()
        {
            return new CommandParser(@"^~For\s+([A-Za-z0-9\$\(\)]+)\s*=\s*(-?[A-Za-z0-9\$\(\)]+)\s*To\s*(-?[A-Za-z0-9\$\(\)]+)(\s+Step\s*(-?[A-Za-z0-9\$\(\)]+))?",
                                     (rc, m) => CreateForCommand(rc, m));
        }

        private static CmdBaseCommand CreateForCommand(ReadContext rc, Match m)
        {
            CmdFor cmd = new CmdFor(rc);
            cmd.VarName = m.Groups[1].Value.Trim();
            cmd.Von = m.Groups[2].Value.Trim();
            cmd.Bis = m.Groups[3].Value.Trim();
            cmd.Step = m.Groups[5].Value.Trim();
            return cmd;
        }

        public CmdFor(ReadContext readcontext) : base(readcontext)
        {
            VarName = string.Empty;
            Von = string.Empty;
            Bis = string.Empty;
            Step = string.Empty;
            ForIndices = new Dictionary<string, int>();
            StepValues = new Dictionary<string, int>();
        }

        public override CmdBaseCommand? Run(RunContext rc)
        {
            string expandedVarName = ReplaceVars(rc, VarName);
            string expandedVon = ReplaceVars(rc, Von);
            string expandedBis = ReplaceVars(rc, Bis);
            string expandedStep = ReplaceVars(rc, Step);
            try
            {
                bool bOKVon = Int32.TryParse(expandedVon, out int resultVon);
                if (!bOKVon)
                {
                    rc.SetError(ReadContext, "Ungültiger numerischer Ausdruck",
                        $"Der Ausdruck '{expandedVon}' kann nicht als ganze Zahl interpretiert werden. Die Ausführung wird abgebrochen.");
                    return null;
                }
                bool bOKBis = Int32.TryParse(expandedBis, out int resultBis);
                if (!bOKBis)
                {
                    rc.SetError(ReadContext, "Ungültiger numerischer Ausdruck",
                        $"Der Ausdruck '{expandedBis}' kann nicht als ganze Zahl interpretiert werden. Die Ausführung wird abgebrochen.");
                    return null;
                }
                if (string.IsNullOrWhiteSpace(expandedStep))
                {
                    // Optionales Argument mit Variable Step wurde weggelassen
                    expandedStep = "1";
                }
                bool bOKStep = Int32.TryParse(expandedStep, out int resultStep);
                if (!bOKStep)
                {
                    rc.SetError(ReadContext, "Ungültiger numerischer Ausdruck",
                        $"Der Ausdruck '{expandedStep}' kann nicht als ganze Zahl interpretiert werden. Die Ausführung wird abgebrochen.");
                    return null;
                }
                if (resultStep == 0)
                {
                    rc.SetError(ReadContext, "Ungültiger Step-Wert",
                        $"Der Step-Wert darf nicht 0 sein. Die Ausführung wird abgebrochen.");
                    return null;
                }
                LinkAssociatedLoopCommand(rc, "For");
                if (AssociatedLoopCommand == null)
                {
                    rc.SetError(ReadContext, "For ohne Loop",
                        "Dem For-Befehl kann kein Loop-Befehl auf gleicher Ebene zugeordnet werden. Die Bearbeitung wird abgebrochen.");
                    return null;
                }                
                if (!IsInitialized.ContainsKey(rc.BaseKey) || !IsInitialized[rc.BaseKey] || !ForIndices.ContainsKey(rc.BaseKey))
                {
                    ForIndices[rc.BaseKey] = resultVon;
                    StepValues[rc.BaseKey] = resultStep;
                    IsInitialized[rc.BaseKey] = true;
                    AssociatedLoopCommand.LoopCounter = 0;
                }
                else
                {
                    // Ergebnis berechnen
                    ForIndices[rc.BaseKey] += StepValues[rc.BaseKey];    
                }
                // und der Variablen zuweisen                
                rc.InternalVars[expandedVarName] = ForIndices[rc.BaseKey].ToString();
                if ((StepValues[rc.BaseKey] > 0 && ForIndices[rc.BaseKey] <= resultBis) ||
                    (StepValues[rc.BaseKey] < 0 && ForIndices[rc.BaseKey] >= resultBis))
                {
                    return NextCommand;
                }
                else
                {
                    // Zurücksetzen für den nächsten Durchlauf
                    ForIndices[rc.BaseKey] = resultVon;
                    IsInitialized[rc.BaseKey] = false;
                    return AssociatedLoopCommand.NextCommand;
                }
            }
            catch (Exception ex)
            {
                rc.SetError(ReadContext, "Verarbeitungsfehler",
                    $"Beim Ausführen des Skriptes ist ein Fehler aufgetreten '{ex.Message}'. Die Ausführung wird abgebrochen."
                    + $"Variablenwerte: expandedVarName='{expandedVarName}' expandedVon='{expandedVon}' expandedBis='{expandedBis}' expandedStep='{expandedStep}'");
                return null;
            }
        }
    }
}