namespace FlowProtocol2.Commands
{
    using System.Text.RegularExpressions;
    using System.Globalization;
    using FlowProtocol2.Core;

    /// <summary>
    /// Implementiert den DateDiff-Befehl
    /// </summary>
    public class CmdDateDiff : CmdBaseCommand
    {
        public string VarName { get; set; }
        public string Date1 { get; set; }
        public string Date2 { get; set; }
        public string Interval { get; set; }

        public static CommandParser GetComandParser()
        {
            return new CommandParser(@"^~DateDiff ([A-Za-z0-9\$\(\)]*)\s*=\s*([A-Za-z0-9\$\(\)\.\-\/: ]*)\s*\.\.\s*([A-Za-z0-9\$\(\)\.\-\/: ]*)\s*\|\s*([A-Za-z0-9\$\(\)]*)", (rc, m) => CreateDateDiffCommand(rc, m));
        }

        private static CmdBaseCommand CreateDateDiffCommand(ReadContext rc, Match m)
        {
            CmdDateDiff cmd = new CmdDateDiff(rc);
            cmd.VarName = m.Groups[1].Value.Trim();
            cmd.Date1 = m.Groups[2].Value.Trim();
            cmd.Date2 = m.Groups[3].Value.Trim();
            cmd.Interval = m.Groups[4].Value.Trim();
            return cmd;
        }

        public CmdDateDiff(ReadContext readcontext) : base(readcontext)
        {
            VarName = string.Empty;
            Date1 = string.Empty;
            Date2 = string.Empty;
            Interval = string.Empty;
        }

        public override CmdBaseCommand? Run(RunContext rc)
        {
            string expandedVarName = ReplaceVars(rc, VarName);
            string expandedDate1 = ReplaceVars(rc, Date1);
            string expandedDate2 = ReplaceVars(rc, Date2);
            string expandedInterval = ReplaceVars(rc, Interval);
            try
            {
                bool bOKD1 = DateTime.TryParseExact(expandedDate1, "yyyy-MM-dd HH:mm:ss", rc.Culture, DateTimeStyles.None, out DateTime Date1);
                if (!bOKD1)
                {
                    rc.SetError(ReadContext, "Falsches Datumsformat",
                        $"Die Zeichenkette '{expandedDate1}' kann nicht als Datum im Format 'yyyy-MM-dd HH:mm:ss' interpretiert werden. Die Ausführung wird abgebrochen.");
                    return null;
                }
                bool bOKD2 = DateTime.TryParseExact(expandedDate2, "yyyy-MM-dd HH:mm:ss", rc.Culture, DateTimeStyles.None, out DateTime Date2);
                if (!bOKD2)
                {
                    rc.SetError(ReadContext, "Falsches Datumsformat",
                        $"Die Zeichenkette '{expandedDate2}' kann nicht als Datum im Format 'yyyy-MM-dd HH:mm:ss' interpretiert werden. Die Ausführung wird abgebrochen.");
                    return null;
                }
                TimeSpan diff = Date2 - Date1;
                int result;    
                switch (expandedInterval)
                {                    
                    case "d": result = diff.TotalDays >= 0 ? (int)diff.TotalDays : (int)Math.Ceiling(diff.TotalDays); break;
                    case "h": result = diff.TotalHours >= 0 ? (int)diff.TotalHours : (int)Math.Ceiling(diff.TotalHours); break;
                    case "m": result = diff.TotalMinutes >= 0 ? (int)diff.TotalMinutes : (int)Math.Ceiling(diff.TotalMinutes); break;
                    case "s": result = diff.TotalSeconds >= 0 ? (int)diff.TotalSeconds : (int)Math.Ceiling(diff.TotalSeconds); break;
                    default:
                        rc.SetError(ReadContext, "Ungültiger Intervallwert",
                            $"Der Ausdruck '{expandedInterval}' entspricht keinem zulässigen Intervallwert d, h, m, s. Die Ausführung wird abgebrochen.");
                        return null;
                }
                rc.InternalVars[expandedVarName] = result.ToString();
            }
            catch (Exception ex)
            {
                rc.SetError(ReadContext, "Verarbeitungfehler",
                    $"Beim Ausführen des Skriptes ist ein Fehler aufgetreten '{ex.Message}'. Die Ausführung wird abgebrochen."
                    + $"Variablenwerte: expandedVarName='{expandedVarName}' expandedDate1='{expandedDate1}' expandedDate2='{expandedDate2}' expandedInterval='{expandedInterval}'");
                return null;
            }
            return NextCommand;
        }
    }
}