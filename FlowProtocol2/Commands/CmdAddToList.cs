namespace FlowProtocol2.Commands
{
    using System.Globalization;
    using System.Text.RegularExpressions;
    using FlowProtocol2.Core;

    /// <summary>
    /// Implementiert den AddToList-Befehl
    /// </summary>
    /// <remarks>
    /// Erstellt mit NewCC.fp2, Eingabe: ~AddToList (vVarName) += (xValue)
    /// </remarks>
    public class CmdAddToList : CmdBaseCommand
    {
        public string VarName { get; set; }
        public string Value { get; set; }

        public static CommandParser GetComandParser()
        {
            return new CommandParser(@"^~AddToList\s+([A-Za-z0-9\$\(\)]+)\s*\+=\s*(.*)",
                                     (rc, m) => CreateAddToListCommand(rc, m));
        }

        private static CmdBaseCommand CreateAddToListCommand(ReadContext rc, Match m)
        {
            CmdAddToList cmd = new CmdAddToList(rc);
            cmd.VarName = m.Groups[1].Value.Trim();
            cmd.Value = m.Groups[2].Value.Trim();
            return cmd;
        }

        public CmdAddToList(ReadContext readcontext) : base(readcontext)
        {
            VarName = string.Empty;
            Value = string.Empty;
        }

        public override CmdBaseCommand? Run(RunContext rc)
        {
            string expandedVarName = ReplaceVars(rc, VarName);
            string expandedValue = ReplaceVars(rc, Value);
            try
            {
                int idx = 1;
                while (rc.InternalVars.ContainsKey($"{expandedVarName}({idx})"))
                {
                    idx++;
                }
                rc.InternalVars[$"{expandedVarName}({idx})"] = expandedValue;
                rc.InternalVars[$"{expandedVarName}(0)"] = idx.ToString();
                rc.InternalVars.Remove($"{expandedVarName}({idx + 1})");
            }
            catch (Exception ex)
            {
                rc.SetError(ReadContext, "Verarbeitungsfehler",
                    $"Beim Ausführen des Skriptes ist ein Fehler aufgetreten '{ex.Message}'. Die Ausführung wird abgebrochen."
                    + $"Variablenwerte: expandedVarName='{expandedVarName}' expandedValue='{expandedValue}'");
                return null;
            }
            return NextCommand;
        }
    }
}