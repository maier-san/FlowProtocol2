namespace FlowProtocol2.Commands
{
    using System.Text.RegularExpressions;
    using FlowProtocol2.Core;

    /// <summary>
    /// Implementiert den GoSub-Befehl
    /// </summary>
    /// <remarks>
    /// Erstellt mit NewCC.fp2, Eingabe: ~GoSub (tSubName)[; BaseKey=(vBaseKey)]
    /// </remarks>
    public class CmdGoSub : CmdBaseCommand
    {
        public string SubName { get; set; }
        public string BaseKey { get; set; }

        public static CommandParser GetComandParser()
        {
            return new CommandParser(@"^~GoSub\s+([A-Za-z0-9]+)(\s*;\s*BaseKey\s*=\s*([A-Za-z0-9\$\(\)]+))?",
                                     (rc, m) => CreateGoSubCommand(rc, m));
        }

        private static CmdBaseCommand CreateGoSubCommand(ReadContext rc, Match m)
        {
            CmdGoSub cmd = new CmdGoSub(rc);
            cmd.SubName = m.Groups[1].Value.Trim();
            cmd.BaseKey = m.Groups[3].Value.Trim();
            return cmd;
        }

        public CmdGoSub(ReadContext readcontext) : base(readcontext)
        {
            SubName = string.Empty;
            BaseKey = string.Empty;
        }

        public override CmdBaseCommand? Run(RunContext rc)
        {
            string expandedSubName = ReplaceVars(rc, SubName);
            string expandedBaseKey = ReplaceVars(rc, BaseKey);
            try
            {
                if (string.IsNullOrWhiteSpace(expandedBaseKey))
                {
                    // Optionales Argument mit Variable BaseKey wurde weggelassen
                    expandedBaseKey = string.Empty;
                }                    
                CmdDefineSub? sub = GetFirstCommand<CmdDefineSub>(rc, c => c.Name == expandedSubName, c => false);
                if (sub == null)
                {
                    rc.SetError(ReadContext, "Sprungziel nicht gefunden",
                        $"Das Sprungziel '{expandedSubName}' konnte nicht gefunden werden. Die Skriptausführung wird abgebrochen.");
                    return null;
                }
                if (NextCommand != null) rc.ReturnStack.Push(new EntryPoint(NextCommand, rc.BaseKey));
                rc.BaseKey = expandedBaseKey;
                return sub.NextCommand;
            }
            catch (Exception ex)
            {
                rc.SetError(ReadContext, "Verarbeitungsfehler",
                    $"Beim Ausführen des Skriptes ist ein Fehler aufgetreten '{ex.Message}'. Die Ausführung wird abgebrochen."
                    + $"Variablenwerte: expandedSubName='{expandedSubName}' expandedBaseKey='{expandedBaseKey}'");
                return null;
            }
        }
    }
}