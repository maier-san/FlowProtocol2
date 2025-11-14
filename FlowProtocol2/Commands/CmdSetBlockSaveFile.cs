namespace FlowProtocol2.Commands
{
    using System.Text.RegularExpressions;

    /// <summary>
    /// Implementiert den SetBlockSaveFile-Befehl
    /// </summary>
    public class CmdSetBlockSaveFile : CmdBaseCommand
    {
        public string FileName { get; set; }

        public static CommandParser GetComandParser()
        {
            return new CommandParser(@"^~SetBlockSaveFile (.*)", (rc, m) => CreateSetBlockSaveFileCommand(rc, m));
        }

        private static CmdBaseCommand CreateSetBlockSaveFileCommand(ReadContext rc, Match m)
        {
            CmdSetBlockSaveFile cmd = new CmdSetBlockSaveFile(rc);
            cmd.FileName = m.Groups[1].Value.Trim();
            return cmd;
        }

        public CmdSetBlockSaveFile(ReadContext readcontext) : base(readcontext)
        {
            FileName = string.Empty;
        }

        public override CmdBaseCommand? Run(RunContext rc)
        {
            string expandedFileName = ReplaceVars(rc, FileName);
            try
            {
                rc.DocumentBuilder.SetBlockSaveFile(expandedFileName);
            }
            catch (Exception ex)
            {
                rc.SetError(ReadContext, "Verarbeitungsfehler",
                    $"Beim Ausführen des Skriptes ist ein Fehler aufgetreten '{ex.Message}'. Die Ausführung wird abgebrochen."
                    + $"Variablenwerte: expandedFileName='{expandedFileName}'");
                return null;
            }
            return NextCommand;
        }
    }
}