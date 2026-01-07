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
        public string ValueType { get; set; }

        public static CommandParser GetComandParser()
        {
            return new CommandParser(@"^~Sort ([A-Za-z0-9\$\(\)]*)(\s*->\s*([A-Za-z0-9\$\(\)]*))?(\s*;\s*ValueType=([A-Za-z0-9\$\(\)]+))?", (rc, m) => CreateSortCommand(rc, m));
        }

        private static CmdBaseCommand CreateSortCommand(ReadContext rc, Match m)
        {
            CmdSort cmd = new CmdSort(rc);
            cmd.FieldName = m.Groups[1].Value.Trim();
            cmd.IndexField = m.Groups[3].Value.Trim();
            cmd.ValueType = m.Groups[5].Value.Trim();
            return cmd;
        }

        public CmdSort(ReadContext readcontext) : base(readcontext)
        {
            FieldName = string.Empty;
            IndexField = string.Empty;
            ValueType = string.Empty;
        }

        public override CmdBaseCommand? Run(RunContext rc)
        {
            string expandedFieldName = ReplaceVars(rc, FieldName);
            string expandedIndexField = ReplaceVars(rc, IndexField);
            string expandedValueType = ReplaceVars(rc, ValueType);
            try
            {
                List<(int index, IComparable? value, string formattedValue)> items = new List<(int, IComparable?, string)>();
                int i = 1;
                string strIndexVar = expandedFieldName + $"({i})";
                while (rc.InternalVars.ContainsKey(strIndexVar))
                {
                    string valueStr = rc.InternalVars[strIndexVar];
                    IComparable? parsedValue;
                    if (!TryParseValue(valueStr, expandedValueType, out parsedValue))
                    {
                        rc.SetError(ReadContext, "Falschen Typenformat",
                            $"Der Wert bei Index {i}, '{valueStr}' kann nicht als Wert vom Typ {expandedValueType} interpretert werden. Die Ausführung wird abgebrochen.");
                        return null;
                    }
                    items.Add((i, parsedValue, valueStr));
                    i++;
                    strIndexVar = expandedFieldName + $"({i})";
                }
                var sortedItems = items.OrderBy(x => x.value);
                i = 1;
                foreach (var item in sortedItems)
                {                    
                    rc.InternalVars[expandedFieldName + $"({i})"] = $"{item.formattedValue}";
                    if (!string.IsNullOrWhiteSpace(expandedIndexField))
                    {
                        rc.InternalVars[expandedIndexField + $"({i})"] = $"{item.index}";
                    }
                    i++;
                }
                rc.InternalVars.Remove(expandedIndexField + $"({i})");
            }
            catch (Exception ex)
            {
                rc.SetError(ReadContext, "Verarbeitungsfehler",
                    $"Beim Ausführen des Skriptes ist ein Fehler aufgetreten '{ex.Message}'. Die Ausführung wird abgebrochen."
                    + $"Variablenwerte: expandedFieldName='{expandedFieldName}' expandedIndexField='{expandedIndexField}' expandedValueType='{expandedValueType}'");
                return null;
            }
            return NextCommand;
        }

        private bool TryParseValue(string valueStr, string valueType, out IComparable? parsedValue)
        {
            parsedValue = null;
            switch (valueType.ToLower())
            {
                case "int":
                    if (int.TryParse(valueStr, out int intVal))
                    {
                        parsedValue = intVal;
                        return true;
                    }
                    return false;
                case "double":
                    if (double.TryParse(valueStr, out double doubleVal))
                    {
                        parsedValue = doubleVal;
                        return true;
                    }
                    return false;
                case "datetime":
                    if (DateTime.TryParseExact(valueStr, "yyyy-MM-dd HH:mm:ss", null, System.Globalization.DateTimeStyles.None, out DateTime dateVal))
                    {
                        parsedValue = dateVal;
                        return true;
                    }
                    return false;
                default:
                    parsedValue = valueStr;
                    return true;
            }
        }
    }
}