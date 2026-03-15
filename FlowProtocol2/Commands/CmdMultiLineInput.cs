namespace FlowProtocol2.Commands
{
    using System.Globalization;
    using System.Text.RegularExpressions;
    using FlowProtocol2.Core;

    /// <summary>
    /// Implementiert den MultiLineInput-Befehl
    /// </summary>
    /// <remarks>
    /// Erstellt mit NewCC.fp2, Eingabe: ~MultiLineInput (kKey): (sPrompt)[; ShowLines=(iShowLines)]
    /// </remarks>
    public class CmdMultiLineInput : CmdInputBaseCommand
    {
        public string Key { get; set; }
        public string Prompt { get; set; }
        public string ShowLines { get; set; }

        public static CommandParser GetComandParser()
        {
            return new CommandParser(@"^~MultiLineInput\s+([A-Za-z0-9\$\(\)]*'?):\s*([^;]*)(\s*;\s*ShowLines\s*=\s*([A-Za-z0-9\$\(\)]+))?",
                                     (rc, m) => CreateMultiLineInputCommand(rc, m));
        }

        private static CmdBaseCommand CreateMultiLineInputCommand(ReadContext rc, Match m)
        {
            CmdMultiLineInput cmd = new CmdMultiLineInput(rc);
            cmd.Key = m.Groups[1].Value.Trim();
            cmd.Prompt = m.Groups[2].Value.Trim();
            cmd.ShowLines = m.Groups[4].Value.Trim();
            return cmd;
        }

        public CmdMultiLineInput(ReadContext readcontext) : base(readcontext)
        {
            Key = string.Empty;
            Prompt = string.Empty;
            ShowLines = string.Empty;
        }

        public override CmdBaseCommand? Run(RunContext rc)
        {
            string expandedKey = ReplaceVars(rc, Key);
            string expandedPrompt = ReplaceVars(rc, Prompt);
            string expandedShowLines = ReplaceVars(rc, ShowLines);
            try
            {
                if (string.IsNullOrWhiteSpace(expandedShowLines))
                {
                    // Optionales Argument mit Variable ShowLines wurde weggelassen
                    expandedShowLines = "5"; // Standardwert verwenden
                }
                bool bOKShowLines = Int32.TryParse(expandedShowLines, out int resultShowLines);
                if (!bOKShowLines)
                {
                    rc.SetError(ReadContext, "Ungültiger numerischer Ausdruck",
                        $"Der Ausdruck '{expandedShowLines}' kann nicht als ganze Zahl interpretiert werden. Die Ausführung wird abgebrochen.");
                    return null;
                }
                var textarea = new IMTextAreaElement();

                string plainKey = expandedKey;
                if (!string.IsNullOrEmpty(rc.BaseKey))
                {
                    expandedKey = rc.BaseKey + "_" + expandedKey;
                }

                textarea.Key = expandedKey;
                textarea.Prompt = expandedPrompt;
                textarea.ShowLines = resultShowLines;

                if (rc.BoundVars.ContainsKey(expandedKey) && !string.IsNullOrEmpty(rc.BoundVars[expandedKey]))
                {
                    rc.GivenKeys.Add(expandedKey);
                }
                else
                {
                    rc.BoundVars[expandedKey] = string.Empty;
                    rc.InputForm.AddInputItem(textarea);
                    AssociatedInputElements[rc.BaseKey] = textarea;
                }
                if (rc.BoundVars.ContainsKey(expandedKey))
                {
                    rc.InternalVars[plainKey] = rc.BoundVars[expandedKey];
                }
            }
            catch (Exception ex)
            {
                rc.SetError(ReadContext, "Verarbeitungsfehler",
                    $"Beim Ausführen des Skriptes ist ein Fehler aufgetreten '{ex.Message}'. Die Ausführung wird abgebrochen."
                    + $"Variablenwerte: expandedKey='{expandedKey}' expandedPrompt='{expandedPrompt}' expandedShowLines='{expandedShowLines}'");
                return null;
            }
            return NextCommand;
        }
    }
}