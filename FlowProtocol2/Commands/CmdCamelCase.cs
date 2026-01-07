namespace FlowProtocol2.Commands
{
    using System.Text.RegularExpressions;

    /// <summary>
    /// Implementiert den CamelCase-Befehl
    /// </summary>
    public class CmdCamelCase : CmdBaseCommand
    {
        public string VarName { get; set; }
        public string Text { get; set; }

        public static CommandParser GetComandParser()
        {
            return new CommandParser(@"^~CamelCase\s+([A-Za-z0-9\$\(\)]+)\s*=\s*(.*)", (rc, m) => CreateCamelCaseCommand(rc, m));
        }

        private static CmdBaseCommand CreateCamelCaseCommand(ReadContext rc, Match m)
        {
            CmdCamelCase cmd = new CmdCamelCase(rc);
            cmd.VarName = m.Groups[1].Value.Trim();
            cmd.Text = m.Groups[2].Value.Trim();
            return cmd;
        }

        public CmdCamelCase(ReadContext readcontext) : base(readcontext)
        {
            VarName = string.Empty;
            Text = string.Empty;
        }

        public override CmdBaseCommand? Run(RunContext rc)
        {
            string expandedVarName = ReplaceVars(rc, VarName);
            string expandedText = ReplaceVars(rc, Text);
            try
            {
                string ccwert = expandedText.Trim();
                ccwert = ccwert.Replace("ä", "ae", false, null)
                            .Replace("ö", "oe", false, null)
                            .Replace("ü", "ue", false, null)
                            .Replace("Ä", "Ae", false, null)
                            .Replace("Ö", "Oe", false, null)
                            .Replace("Ü", "Ue", false, null)
                            .Replace("ß", "ss", false, null)
                            .Replace("á", "a", false, null)
                            .Replace("à", "a", false, null)
                            .Replace("â", "a", false, null)
                            .Replace("é", "e", false, null)
                            .Replace("è", "e", false, null)
                            .Replace("ê", "e", false, null)
                            .Replace("ë", "e", false, null)
                            .Replace("í", "i", false, null)
                            .Replace("ì", "i", false, null)
                            .Replace("î", "i", false, null)
                            .Replace("ï", "i", false, null)
                            .Replace("ó", "o", false, null)
                            .Replace("ò", "o", false, null)
                            .Replace("ô", "o", false, null)
                            .Replace("ú", "u", false, null)
                            .Replace("ù", "u", false, null)
                            .Replace("û", "u", false, null)
                            .Replace("ñ", "n", false, null)
                            .Replace("ç", "c", false, null)
                            .Replace("ÿ", "y", false, null)
                            .Replace("Á", "A", false, null)
                            .Replace("À", "A", false, null)
                            .Replace("Â", "A", false, null)
                            .Replace("É", "E", false, null)
                            .Replace("È", "E", false, null)
                            .Replace("Ê", "E", false, null)
                            .Replace("Ë", "E", false, null)
                            .Replace("Í", "I", false, null)
                            .Replace("Ì", "I", false, null)
                            .Replace("Î", "I", false, null)
                            .Replace("Ï", "I", false, null)
                            .Replace("Ó", "O", false, null)
                            .Replace("Ò", "O", false, null)
                            .Replace("Ô", "O", false, null)
                            .Replace("Ú", "U", false, null)
                            .Replace("Ù", "U", false, null)
                            .Replace("Û", "U", false, null)
                            .Replace("Ñ", "N", false, null)
                            .Replace("Ç", "C", false, null)
                            .Replace("Ÿ", "Y", false, null);
                ccwert = Regex.Replace(ccwert, @"[^\w]", "_");
                ccwert = Regex.Replace(ccwert, @"__*", "_");
                while (ccwert.Contains('_'))
                {
                    int pos = ccwert.IndexOf('_');
                    if (pos + 2 < ccwert.Length)
                    {
                        ccwert = ccwert[..pos] + ccwert.Substring(pos + 1, 1).ToUpper() + ccwert[(pos + 2)..];
                    }
                    else
                    {
                        ccwert = ccwert.Replace("_", "");
                    }
                }
                rc.InternalVars[expandedVarName] = ccwert;
            }
            catch (Exception ex)
            {
                rc.SetError(ReadContext, "Verarbeitungsfehler",
                    $"Beim Ausführen des Skriptes ist ein Fehler aufgetreten '{ex.Message}'. Die Ausführung wird abgebrochen."
                    + $"Variablenwerte: expandedVarName='{expandedVarName}' expandedText='{expandedText}'");
                return null;
            }            
            return NextCommand;
        }
    }
}