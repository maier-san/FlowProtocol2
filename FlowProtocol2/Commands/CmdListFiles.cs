namespace FlowProtocol2.Commands
{
    using System.Text.RegularExpressions;
    using FlowProtocol2.Core;

    /// <summary>
    /// Implementiert den ListFiles-Befehl
    /// </summary>
    public class CmdListFiles : CmdBaseCommand
    {
        public string VarName { get; set; }
        public string DirPath { get; set; }
        public string Pattern { get; set; }

        public static CommandParser GetComandParser()
        {
            return new CommandParser(@"^~ListFiles ([A-Za-z0-9\$\(\)]*)\s*=([^;]*)\s*(; Pattern=.*)?", (rc, m) => CreateListFilesCommand(rc, m));
        }

        private static CmdBaseCommand CreateListFilesCommand(ReadContext rc, Match m)
        {
            CmdListFiles cmd = new CmdListFiles(rc);
            cmd.VarName = m.Groups[1].Value.Trim();
            cmd.DirPath = m.Groups[2].Value.Trim();
            cmd.Pattern = m.Groups[3].Value.Trim();
            return cmd;
        }

        public CmdListFiles(ReadContext readcontext) : base(readcontext)
        {
            VarName = string.Empty;
            DirPath = string.Empty;
            Pattern = string.Empty;
        }

        public override CmdBaseCommand? Run(RunContext rc)
        {
            string expandedVarName = ReplaceVars(rc, VarName);
            string expandedDirPath = ReplaceVars(rc, DirPath).Replace('|', Path.DirectorySeparatorChar);
            string expandedPattern = ReplaceVars(rc, Pattern.Replace("; Pattern=", string.Empty));
            try
            {
                string absolutePath = $"{rc.CurrentScriptPath}";
                if (expandedDirPath.Trim() != ".")
                {
                    absolutePath = ExpandPath(rc, expandedDirPath);
                }
                DirectoryInfo diSource = new DirectoryInfo(absolutePath);
                if (!diSource.Exists)
                {
                    rc.SetError(ReadContext, "Verzeichnis nicht gefunden",
                        $"Das Verzeichnis '{absolutePath}' wurde nicht gefunden. Die Ausführung wird abgebrochen.");
                    return null;
                }
                int index = 0;
                if (string.IsNullOrWhiteSpace(expandedPattern))
                {
                    expandedPattern = "*";
                }
                foreach (var fi in diSource.EnumerateFiles(expandedPattern))
                {
                    index++;
                    rc.InternalVars[$"{expandedVarName}({index})"] = fi.Name;
                }
                rc.InternalVars[$"{expandedVarName}(0)"] = index.ToString();
                index++;
                rc.InternalVars.Remove($"{expandedVarName}({index})");
            }
            catch (Exception ex)
            {
                rc.SetError(ReadContext, "Verarbeitungfehler",
                    $"Beim Ausführen des Skriptes ist ein Fehler aufgetreten '{ex.Message}'. Die Ausführung wird abgebrochen."
                    + $"Variablenwerte: expandedVarName='{expandedVarName}' expandedDirPath='{expandedDirPath}' expandedPattern='{expandedPattern}'");
                return null;
            }
            return NextCommand;
        }
    }
}