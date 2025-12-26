namespace FlowProtocol2.Commands
{
    using System.Text.RegularExpressions;
    using FlowProtocol2.Core;

    /// <summary>
    /// Implementiert den Sort-Befehl
    /// </summary>
    public class CmdSort : CmdBaseCommand
    {
        public string FieldName { get; set; }
        public string IndexField { get; set; }

        public static CommandParser GetComandParser()
        {
            return new CommandParser(@"^~Sort ([A-Za-z0-9\$\(\)]*)(\s*->\s*([A-Za-z0-9\$\(\)]*))?", (rc, m) => CreateSortCommand(rc, m));
        }

        private static CmdBaseCommand CreateSortCommand(ReadContext rc, Match m)
        {
            CmdSort cmd = new CmdSort(rc);
            cmd.FieldName = m.Groups[1].Value.Trim();
            cmd.IndexField = m.Groups[3].Value.Trim();
            return cmd;
        }

        public CmdSort(ReadContext readcontext) : base(readcontext)
        {
            FieldName = string.Empty;
            IndexField = string.Empty;
        }

        public override CmdBaseCommand? Run(RunContext rc)
        {
            string expandedFieldName = ReplaceVars(rc, FieldName);
            string expandedIndexField = ReplaceVars(rc, IndexField);
            try
            {
                Dictionary<int, string> sdic = new Dictionary<int, string>();
                int i = 1;
                string strIndexVar = expandedFieldName + $"({i})";
                while (rc.InternalVars.ContainsKey(strIndexVar))
                {
                    sdic[i] = rc.InternalVars[strIndexVar];
                    i++;
                    strIndexVar = expandedFieldName + $"({i})";
                }
                i = 1;
                foreach (var k in sdic.OrderBy((x) => x.Value))
                {
                    strIndexVar = expandedFieldName + $"({i})";
                    rc.InternalVars[strIndexVar] = k.Value;
                    if (!string.IsNullOrWhiteSpace(expandedIndexField))
                    {
                        rc.InternalVars[expandedIndexField + $"({i})"] = $"{k.Key}";
                    }
                    i++;
                }
                rc.InternalVars.Remove(expandedIndexField + $"({i})");
            }
            catch (Exception ex)
            {
                rc.SetError(ReadContext, "Verarbeitungsfehler",
                    $"Beim Ausführen des Skriptes ist ein Fehler aufgetreten '{ex.Message}'. Die Ausführung wird abgebrochen."
                    + $"Variablenwerte: expandedFieldName='{expandedFieldName}' expandedIndexField='{expandedIndexField}'");
                return null;
            }
            return NextCommand;
        }
    }
}