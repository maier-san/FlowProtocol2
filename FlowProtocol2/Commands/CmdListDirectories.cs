namespace FlowProtocol2.Commands
{
    using System.Text.RegularExpressions;
    using FlowProtocol2.Core;

    /// <summary>
    /// Implementiert den ListDirectories-Befehl
    /// </summary>
    public class CmdListDirectories : CmdBaseCommand
    {
        public string VarName { get; set; }
        public string DirPath { get; set; }
        public string Pattern { get; set; }

        public static CommandParser GetComandParser()
        {
            return new CommandParser(@"^~ListDirectories ([A-Za-z0-9\$\(\)]*)\s*=([^;]*)\s*(; Pattern=.*)?", (rc, m) => CreateListDirectoriesCommand(rc, m));
        }

        private static CmdBaseCommand CreateListDirectoriesCommand(ReadContext rc, Match m)
        {
            CmdListDirectories cmd = new CmdListDirectories(rc);
            cmd.VarName = m.Groups[1].Value.Trim();
            cmd.DirPath = m.Groups[2].Value.Trim();
            cmd.Pattern = m.Groups[3].Value.Trim();
            return cmd;
        }

        public CmdListDirectories(ReadContext readcontext) : base(readcontext)
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
                absolutePath = absolutePath.TrimEnd(Path.DirectorySeparatorChar);
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
                foreach (var di in diSource.EnumerateDirectories(expandedPattern))
                {
                    index++;
                    rc.InternalVars[$"{expandedVarName}({index})"] = di.Name;
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