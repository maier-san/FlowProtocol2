namespace FlowProtocol2.Commands
{
    using System.Globalization;
    using System.Text.RegularExpressions;
    using FlowProtocol2.Core;

    /// <summary>
    /// Implementiert den DateAdd-Befehl
    /// </summary>
    public class CmdDateAdd : CmdBaseCommand
    {
        public string VarName { get; set; }
        public string BaseDate { get; set; }
        public string Value { get; set; }
        public string Interval { get; set; }

        public static CommandParser GetComandParser()
        {
            return new CommandParser(@"^~DateAdd ([A-Za-z0-9\$\(\)]*)\s*=\s*([A-Za-z0-9\$\(\)\.\-\/: ]*)\s*\|\s*(-?[A-Za-z0-9\$\(\)]*)\s*\|\s*([A-Za-z0-9\$\(\)]*)", (rc, m) => CreateDateAddCommand(rc, m));
        }

        private static CmdBaseCommand CreateDateAddCommand(ReadContext rc, Match m)
        {
            CmdDateAdd cmd = new CmdDateAdd(rc);
            cmd.VarName = m.Groups[1].Value.Trim();
            cmd.BaseDate = m.Groups[2].Value.Trim();
            cmd.Value = m.Groups[3].Value.Trim();
            cmd.Interval = m.Groups[4].Value.Trim();
            return cmd;
        }

        public CmdDateAdd(ReadContext readcontext) : base(readcontext)
        {
            VarName = string.Empty;
            BaseDate = string.Empty;
            Value = string.Empty;
            Interval = string.Empty;
        }

        public override CmdBaseCommand? Run(RunContext rc)
        {
            string expandedVarName = ReplaceVars(rc, VarName);
            string expandedBaseDate = ReplaceVars(rc, BaseDate);
            string expandedValue = ReplaceVars(rc, Value);
            string expandedInterval = ReplaceVars(rc, Interval);
            try
            {
                bool bOKBd = DateTime.TryParseExact(expandedBaseDate, "yyyy-MM-dd HH:mm:ss", rc.Culture, DateTimeStyles.None, out DateTime baseDate);
                if (!bOKBd)
                {
                    rc.SetError(ReadContext, "Falsches Datumsformat",
                        $"Die Zeichenkette '{expandedBaseDate}' kann nicht als Datum im Format 'yyyy-MM-dd HH:mm:ss' interpretiert werden. Die Ausführung wird abgebrochen.");
                    return null;
                }
                bool bOKVal = Int32.TryParse(expandedValue, out int ival);
                if (!bOKVal)
                {
                    rc.SetError(ReadContext, "Ungültiger numerischer Ausdruck",
                        $"Der Ausdruck '{expandedValue}' kann nicht als ganze Zahl interpretiert werden. Die Ausführung wird abgebrochen.");
                    return null;
                }
                DateTime result = baseDate;
                switch (expandedInterval)
                {
                    case "y": result = baseDate.AddYears(ival); break;
                    case "M": result = baseDate.AddMonths(ival); break;
                    case "w": result = baseDate.AddDays(ival * 7); break;
                    case "d": result = baseDate.AddDays(ival); break;
                    case "h": result = baseDate.AddHours(ival); break;
                    case "m": result = baseDate.AddMinutes(ival); break;
                    default:
                        rc.SetError(ReadContext, "Ungültiger Intervallwert",
                            $"Der Ausdruck '{expandedInterval}' entspricht keinem zulässigen Intervallwert y, M, w, d, h, m. Die Ausführung wird abgebrochen.");
                        return null;
                }
                rc.InternalVars[expandedVarName] = result.ToString("yyyy-MM-dd HH:mm:ss");
            }
            catch (Exception ex)
            {
                rc.SetError(ReadContext, "Verarbeitungfehler",
                    $"Beim Ausführen des Skriptes ist ein Fehler aufgetreten '{ex.Message}'. Die Ausführung wird abgebrochen."
                    + $"Variablenwerte: expandedVarName='{expandedVarName}' expandedBaseDate='{expandedBaseDate}' expandedValue='{expandedValue}' expandedInterval='{expandedInterval}'");
                return null;
            }
            return NextCommand;
        }
    }
}