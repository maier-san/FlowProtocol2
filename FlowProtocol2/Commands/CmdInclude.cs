namespace FlowProtocol2.Commands
{
    using System.Text.RegularExpressions;
    using FlowProtocol2.Core;

    /// <summary>
    ///   Implementiert den Include-Befehl
    /// </summary>
    /// <remarks>
    /// Erstellt mit NewCC.fp2, Eingabe: ~Include (sScriptNameOrPath)[; BaseKey=(vBaseKey)]
    /// </remarks>
    public class CmdInclude : CmdBaseCommand
    {
        public string ScriptNameOrPath { get; set; }
        public string BaseKey { get; set; }

        public static CommandParser GetComandParser()
        {
            return new CommandParser(@"^~Include\s+([^;]*)(\s*;\s*BaseKey\s*=\s*([A-Za-z0-9\$\(\)]+))?",
                                     (rc, m) => CreateIncludeCommand(rc, m));
        }

        private static CmdBaseCommand CreateIncludeCommand(ReadContext rc, Match m)
        {
            CmdInclude cmd = new CmdInclude(rc);
            cmd.ScriptNameOrPath = m.Groups[1].Value.Trim();
            cmd.BaseKey = m.Groups[3].Value.Trim();
            return cmd;
        }


        public CmdInclude(ReadContext readcontext) : base(readcontext)
        {
            ScriptNameOrPath = string.Empty;
            BaseKey = string.Empty;
        }

        public override CmdBaseCommand? Run(RunContext rc)
        {
            string expandedScriptNameOrPath = ReplaceVars(rc, ScriptNameOrPath).Replace('|', Path.DirectorySeparatorChar);
            string expandedBaseKey = ReplaceVars(rc, BaseKey);
            try
            {
                if (string.IsNullOrWhiteSpace(expandedBaseKey))
                {
                    // Optionales Argument mit Variable BaseKey wurde weggelassen
                    expandedBaseKey = string.Empty;
                }
                string absolutescriptfilepath = ExpandPath(rc, expandedScriptNameOrPath, out bool fileexists);
                if (!rc.ScriptRepository.ContainsKey(absolutescriptfilepath))
                {
                    if (!fileexists)
                    {
                        rc.SetError(ReadContext, "Skriptdatei nicht gefunden",
                            $"Die Skriptdatei '{absolutescriptfilepath}' konnte nicht gefunden werden. Die Skriptausführung wird abgebrochen.");
                        return null;
                    }
                    ScriptParser sp = new ScriptParser();
                    var newScriptinfo = sp.ReadScript(rc, absolutescriptfilepath, Indent);
                    rc.ScriptRepository[absolutescriptfilepath] = newScriptinfo;
                }
                var sinfo = rc.ScriptRepository[absolutescriptfilepath];
                if (sinfo.StartCommand != null)
                {                    
                    if (NextCommand != null) rc.ReturnStack.Push(new EntryPoint(NextCommand, rc.BaseKey));
                    rc.BaseKey = expandedBaseKey;
                    return sinfo.StartCommand;
                }
            }
            catch (Exception ex)
            {
                rc.SetError(ReadContext, "Verarbeitungsfehler",
                    $"Beim Ausführen des Skriptes ist ein Fehler aufgetreten '{ex.Message}'. Die Ausführung wird abgebrochen."
                    + $"Variablenwerte: expandedScriptNameOrPath='{expandedScriptNameOrPath}' expandedBaseKey='{expandedBaseKey}'");
                return null;
            }
            return NextCommand;
        }
    }
}