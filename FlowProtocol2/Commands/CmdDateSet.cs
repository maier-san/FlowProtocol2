namespace FlowProtocol2.Commands
{
    using System.Text.RegularExpressions;
    using FlowProtocol2.Core;
    using System.Globalization;

    /// <summary>
    /// Implementiert den DateSet-Befehl
    /// </summary>
    public class CmdDateSet : CmdBaseCommand
    {
        public string VarName { get; set; }
        public string Value { get; set; }
        public string Format { get; set; }

        public static CommandParser GetComandParser()
        {
            return new CommandParser(@"^~DateSet ([A-Za-z0-9\$\(\)]*)\s*=\s*([A-Za-z0-9\$\(\)\.\-\/]*)\s*\|\s*([A-Za-z0-9\$\(\)\.\-\/]*)", (rc, m) => CreateDateSetCommand(rc, m));
        }

        private static CmdBaseCommand CreateDateSetCommand(ReadContext rc, Match m)
        {
            CmdDateSet cmd = new CmdDateSet(rc);
            cmd.VarName = m.Groups[1].Value.Trim();
            cmd.Value = m.Groups[2].Value.Trim();
            cmd.Format = m.Groups[3].Value.Trim();
            return cmd;
        }

        public CmdDateSet(ReadContext readcontext) : base(readcontext)
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
                bool bOK = DateTime.TryParseExact(expandedValue, expandedFormat, rc.Culture, DateTimeStyles.None, out DateTime result);
                if (!bOK)
                {
                    rc.SetError(ReadContext, "Falsches Datumsformat",
                        $"Die Zeichenkette '{expandedValue}' kann nicht als Datum im Format '{expandedFormat}' interpretiert werden. Die Ausführung wird abgebrochen.");
                    return null;
                }
                rc.InternalVars[expandedVarName] = result.ToString("yyyy-MM-dd HH:mm:ss");
            }
            catch (Exception ex)
            {
                rc.SetError(ReadContext, "Verarbeitungsfehler",
                    $"Beim Ausführen des Skriptes ist ein Fehler aufgetreten: '{ex.Message}'. Die Ausführung wird abgebrochen. "
                    + $"Variablenwerte: expandedValue='{expandedValue}', expandedFormat='{expandedFormat}'");
                return null;
            }
            return NextCommand;
        }
    }
}