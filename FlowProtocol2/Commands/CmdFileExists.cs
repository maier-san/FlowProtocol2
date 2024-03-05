namespace FlowProtocol2.Commands
{
    using System.Text.RegularExpressions;
    using FlowProtocol2.Core;

    /// <summary>
    /// Implementiert den FileExists-Befehl
    /// </summary>
    public class CmdFileExists : CmdBaseCommand
    {
        public string VarName { get; set; }
        public string FilePath { get; set; }

        public static CommandParser GetComandParser()
        {
            return new CommandParser(@"^~FileExists ([A-Za-z0-9\$\(\)]*)\s*=(.*)", (rc, m) => CreateFileExistsCommand(rc, m));
        }

        private static CmdBaseCommand CreateFileExistsCommand(ReadContext rc, Match m)
        {
            CmdFileExists cmd = new CmdFileExists(rc);
            cmd.VarName = m.Groups[1].Value.Trim();
            cmd.FilePath = m.Groups[2].Value.Trim();
            return cmd;
        }

        public CmdFileExists(ReadContext readcontext) : base(readcontext)
        {
            VarName = string.Empty;
            FilePath = string.Empty;
        }

        public override CmdBaseCommand? Run(RunContext rc)
        {
            string expandedVarName = ReplaceVars(rc, VarName);
            string expandedFilePath = ReplaceVars(rc, FilePath).Replace('|', Path.DirectorySeparatorChar);
            try
            {
                string absoluteFilePath = ExpandPath(rc, expandedFilePath, out bool fileexists);
                rc.InternalVars[expandedVarName] = BoolString(fileexists);
            }
            catch (Exception ex)
            {
                rc.SetError(ReadContext, "Verarbeitungfehler",
                    $"Beim Ausführen des Skriptes ist ein Fehler aufgetreten '{ex.Message}'. Die Ausführung wird abgebrochen."
                    + $"Variablenwerte: expandedVarName='{expandedVarName}' expandedFilePath='{expandedFilePath}'");
                return null;
            }
            return NextCommand;
        }
    }
}