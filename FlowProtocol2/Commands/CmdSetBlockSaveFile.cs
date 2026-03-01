namespace FlowProtocol2.Commands
{
    using System.Text.RegularExpressions;

    /// <summary>
    /// Implementiert den SetBlockSaveFile-Befehl
    /// </summary>
    /// <remarks>
    /// Erstellt mit NewCC.fp2, Eingabe: ~SetBlockSaveFile (sFileName)[; Encoding=(sEncodingName)]
    /// </remarks>
    public class CmdSetBlockSaveFile : CmdBaseCommand
    {
        public string FileName { get; set; }
        public string EncodingName { get; set; }

        public static CommandParser GetComandParser()
        {
            return new CommandParser(@"^~SetBlockSaveFile\s+([^;]*)(\s*;\s*Encoding\s*=\s*([^;]*))?",
                                     (rc, m) => CreateSetBlockSaveFileCommand(rc, m));
        }

        private static CmdBaseCommand CreateSetBlockSaveFileCommand(ReadContext rc, Match m)
        {
            CmdSetBlockSaveFile cmd = new CmdSetBlockSaveFile(rc);
            cmd.FileName = m.Groups[1].Value.Trim();
            cmd.EncodingName = m.Groups[3].Value.Trim();
            return cmd;
        }

        public CmdSetBlockSaveFile(ReadContext readcontext) : base(readcontext)
        {
            FileName = string.Empty;
            EncodingName = string.Empty;
        }

        public override CmdBaseCommand? Run(RunContext rc)
        {
            string expandedFileName = ReplaceVars(rc, FileName);            
            string expandedEncodingName = ReplaceVars(rc, EncodingName);
            try
            {
                if (string.IsNullOrWhiteSpace(expandedEncodingName))
                {
                    // Optionales Argument mit Variable EncodingName wurde weggelassen
                    expandedEncodingName = "utf-8";
                }
                if (!IsValidEncodingName(expandedEncodingName))
                {
                    rc.SetError(ReadContext, "Unbekannte Codierung",
                        $"Die angegebene Codierung '{expandedEncodingName}' kann nicht verarbeitet werden. Stattdessen wird die Codierung UTF-8 verwendet.");
                    expandedEncodingName = "utf-8";
                }
                rc.DocumentBuilder.SetBlockSaveFile(expandedFileName, expandedEncodingName);
            }
            catch (Exception ex)
            {
                rc.SetError(ReadContext, "Verarbeitungsfehler",
                    $"Beim Ausführen des Skriptes ist ein Fehler aufgetreten '{ex.Message}'. Die Ausführung wird abgebrochen."
                    + $"Variablenwerte: expandedFileName='{expandedFileName}' expandedEncodingName='{expandedEncodingName}'");
                return null;
            }
            return NextCommand;
        }

        private bool IsValidEncodingName(string expandedEncodingName)
        {
            if (string.IsNullOrWhiteSpace(expandedEncodingName))
            {
                return false;
            }
            string enc = expandedEncodingName.Trim().ToLowerInvariant();

            // Die Prüfung entspricht dem, was SaveFunction in Run.cshtml verarbeiten kann.
            // - Kodierungen, die mit "utf-16" beginnen, werden als UTF-16 (LE with BOM) behandelt.
            // - Zulässige Ein-Byte-Kodierungen sind: ascii, windows-1252, latin1, iso-8859-1
            // - Ansonsten Rückfall auf Standardwert UTF-8.
            if (enc.StartsWith("utf-16"))
            {
                return true;
            }

            switch (enc)
            {
                case "ascii":
                case "windows-1252":
                case "latin1":
                case "iso-8859-1":
                case "utf-8":
                case "utf8":
                    return true;
                default:
                    return false;
            }
        }
    }
}