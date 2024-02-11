namespace FlowProtocol2.Commands
{
    using System.Globalization;
    using System.Text.RegularExpressions;
    using FlowProtocol2.Core;

    /// <summary>
    /// Implementiert den DateFormat-Befehl
    /// </summary>
    public class CmdDateFormat : CmdBaseCommand
    {
        public string VarName { get; set; }
        public string Value { get; set; }
        public string Format { get; set; }

        public static CommandParser GetComandParser()
        {
            return new CommandParser(@"^~DateFormat ([A-Za-z0-9\$\(\)]*)\s*=\s*([A-Za-z0-9\$\(\)\.\-\/]*)\s*\|\s*([A-Za-z0-9\$\(\)\.\-\/]*)", (rc, m) => CreateDateFormatCommand(rc, m));
        }

        private static CmdBaseCommand CreateDateFormatCommand(ReadContext rc, Match m)
        {
            CmdDateFormat cmd = new CmdDateFormat(rc);
            cmd.VarName = m.Groups[1].Value.Trim();
            cmd.Value = m.Groups[2].Value.Trim();
            cmd.Format = m.Groups[3].Value.Trim();
            return cmd;
        }

        public CmdDateFormat(ReadContext readcontext) : base(readcontext)
        {
            VarName = string.Empty;
            Value = string.Empty;
            Format = string.Empty;
        }

        public override CmdBaseCommand? Run(RunContext rc)
        {
            string expandedVarName = ReplaceVars(rc, VarName);
            string expandedValue = ReplaceVars(rc, Value);
            string expandedFormat = ReplaceVars(rc, Format);            
            try
            {
                bool bOKBd = DateTime.TryParseExact(expandedValue, "yyyy-MM-dd HH:mm:ss", rc.Culture, DateTimeStyles.None, out DateTime result);
                if (!bOKBd)
                {
                    rc.SetError(ReadContext, "Falsches Datumsformat",
                        $"Die Zeichenkette '{expandedValue}' kann nicht als Datum im Format 'yyyy-MM-dd HH:mm:ss' interpretiert werden. Die Ausführung wird abgebrochen.");
                    return null;
                }
                rc.InternalVars[expandedVarName] = result.ToString(expandedFormat, rc.Culture);
            }
            catch (Exception ex)
            {
                rc.SetError(ReadContext, "Verarbeitungfehler",
                    $"Beim Ausführen des Skriptes ist ein Fehler aufgetreten '{ex.Message}'. Dies Ausführung wird abgebrochen."
                    + $"Variablenwerte: expandedVarName='{expandedVarName}' expandedValue='{expandedValue}' expandedFormat='{expandedFormat}'");
                return null;
            }
            return NextCommand;
        }
    }
}