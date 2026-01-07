namespace FlowProtocol2.Commands
{
    using System.Text.RegularExpressions;
    using FlowProtocol2.Core;

    /// <summary>
    /// Implementiert den ForEach-Befehl
    /// </summary>
    public class CmdForEach : CmdLoopBaseCommand
    {
        public string VarName { get; set; }
        public string FieldName { get; set; }
        private Dictionary<string, int> ForIndices { get; set; }

        public static CommandParser GetComandParser()
        {
            return new CommandParser(@"^~ForEach\s+([A-Za-z0-9\$\(\)]+)\s+in\s+([A-Za-z0-9\$\(\)]+)", (rc, m) => CreateForEachCommand(rc, m));
        }

        private static CmdBaseCommand CreateForEachCommand(ReadContext rc, Match m)
        {
            CmdForEach cmd = new CmdForEach(rc);
            cmd.VarName = m.Groups[1].Value.Trim();
            cmd.FieldName = m.Groups[2].Value.Trim();
            return cmd;
        }

        public CmdForEach(ReadContext readcontext) : base(readcontext)
        {
            VarName = string.Empty;
            FieldName = string.Empty;
            ForIndices = new Dictionary<string, int>();
        }

        public override CmdBaseCommand? Run(RunContext rc)
        {
            string expandedVarName = ReplaceVars(rc, VarName);
            string expandedFieldName = ReplaceVars(rc, FieldName);
            try
            {
                LinkAssociatedLoopCommand(rc, "ForEach");
                if (AssociatedLoopCommand != null)
                {
                    if (!IsInitialized.ContainsKey(rc.BaseKey) || !IsInitialized[rc.BaseKey] || !ForIndices.ContainsKey(rc.BaseKey))
                    {
                        ForIndices[rc.BaseKey] = 0;
                        IsInitialized[rc.BaseKey] = true;
                        AssociatedLoopCommand.LoopCounter = 0;
                    }
                    ForIndices[rc.BaseKey]++;
                    string currentfieldvar = $"{expandedFieldName}({ForIndices[rc.BaseKey]})";
                    if (rc.InternalVars.ContainsKey(currentfieldvar))
                    {
                        rc.InternalVars[expandedVarName] = rc.InternalVars[currentfieldvar];
                        return NextCommand;
                    }
                    else
                    {
                        ForIndices[rc.BaseKey] = 0;
                        IsInitialized[rc.BaseKey] = false;
                        return AssociatedLoopCommand.NextCommand;
                    }
                }
                rc.SetError(ReadContext, "ForEach ohne Loop",
                    "Dem ForEach-Befehl kann kein Loop-Befehl auf gleicher Ebene zugeordnet werden. Die Bearbeitung wird abgebrochen.");
            }
            catch (Exception ex)
            {
                rc.SetError(ReadContext, "Verarbeitungsfehler",
                    $"Beim Ausführen des Skriptes ist ein Fehler aufgetreten '{ex.Message}'. Die Ausführung wird abgebrochen."
                    + $"Variablenwerte: expandedVarName='{expandedVarName}' expandedFieldName='{expandedFieldName}'");
                return null;
            }
            return null;
        }
    }
}