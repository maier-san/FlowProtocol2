namespace FlowProtocol2.Commands
{
    using System.Text.RegularExpressions;
    using FlowProtocol2.Core;

    /// <summary>
    /// Implementiert den DynamicOptionGroup-Befehl
    /// </summary>
    public class CmdDynamicOptionGroup : CmdInputBaseCommand
    {
        public string Key { get; set; }
        public string VarName { get; set; }
        public string Promt { get; set; }
        private int InputIndex { get; set; }

        public static CommandParser GetComandParser()
        {
            return new CommandParser(@"^~DynamicOptionGroup ([A-Za-z0-9\$\(\)]*[']?):\s*([A-Za-z0-9\$\(\)]*)\s*;(.*)", (rc, m) => CreateDynamicOptionGroupCommand(rc, m));
        }

        private static CmdBaseCommand CreateDynamicOptionGroupCommand(ReadContext rc, Match m)
        {
            CmdDynamicOptionGroup cmd = new CmdDynamicOptionGroup(rc);
            cmd.Key = m.Groups[1].Value.Trim();
            cmd.VarName = m.Groups[2].Value.Trim();
            cmd.Promt = m.Groups[3].Value.Trim();
            return cmd;
        }

        public CmdDynamicOptionGroup(ReadContext readcontext) : base(readcontext)
        {
            Key = string.Empty;
            VarName = string.Empty;
            Promt = string.Empty;
        }
        public override CmdBaseCommand? Run(RunContext rc)
        {
            string expandedKey = ReplaceVars(rc, Key);
            string expandedVarName = ReplaceVars(rc, VarName);
            string expandedPromt = ReplaceVars(rc, Promt);
            try
            {
                IMOptionGroupElement ogroup = new IMOptionGroupElement();
                string plainKey = expandedKey;
                if (string.IsNullOrEmpty(expandedKey))
                {
                    expandedKey = "'";
                }
                if (!string.IsNullOrEmpty(rc.BaseKey))
                {
                    expandedKey = rc.BaseKey + "_" + expandedKey;
                }
                if (expandedKey.EndsWith("'"))
                {
                    if (InputIndex == 0)
                    {
                        InputIndex = GetPreviousCommands<CmdInputBaseCommand>(c => true, c => false).Count + 1;
                    }
                    expandedKey = expandedKey.Replace("'", "_" + InputIndex.ToString());
                }
                ogroup.Key = expandedKey;
                ogroup.Promt = ReplaceVars(rc, Promt).Trim();
                string selectedKey = string.Empty;
                if (rc.BoundVars.ContainsKey(expandedKey))
                {
                    selectedKey = rc.BoundVars[expandedKey];
                }
                string existingSelectedKey = string.Empty;
                int idx = 1;
                while (rc.InternalVars.ContainsKey($"{expandedVarName}({idx})"))
                {
                    IMOptionValue ov = new IMOptionValue(ogroup);
                    ov.Key = idx.ToString();
                    ov.Promt = ReplaceVars(rc, rc.InternalVars[$"{expandedVarName}({idx})"]);
                    ogroup.Options.Add(ov);
                    if (!string.IsNullOrEmpty(selectedKey) && ov.Key == selectedKey)
                    {
                        existingSelectedKey = ov.Key;
                    }
                    idx++;
                }
                if (idx == 1)
                {
                    return NextCommand;
                }
                if (!string.IsNullOrEmpty(existingSelectedKey))
                {
                    rc.GivenKeys.Add(expandedKey);
                }
                else
                {
                    rc.BoundVars[expandedKey] = string.Empty;
                    rc.InputForm.AddInputItem(ogroup);
                    AssociatedInputElement = ogroup;
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
                    + $"Variablenwerte: expandedKey='{expandedKey}' expandedVarName='{expandedVarName}' expandedPromt='{expandedPromt}'");
                return null;
            }
            return NextCommand;
        }
    }
}