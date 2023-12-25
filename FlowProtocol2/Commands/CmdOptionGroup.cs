namespace FlowProtocol2.Commands
{
    using System.Text.RegularExpressions;
    using FlowProtocol2.Core;

    /// <summary>
    /// Implementiert den Option-Befehl
    /// </summary>
    public class CmdOptionGroup : CmdInputBaseCommand
    {
        public string Key { get; set; }
        public string Promt { get; set; }
        public CmdOptionValue? SelectedOptionCommand { get; set; }

        public static CommandParser GetComandParser()
        {
            return new CommandParser(@"^\?([A-Za-z0-9$]*[']?):(.*)", (rc, m) => CreateOptionGroupCommand(rc, m));
        }

        private static CmdBaseCommand CreateOptionGroupCommand(ReadContext rc, Match m)
        {
            CmdOptionGroup cmd = new CmdOptionGroup(rc);
            cmd.Key = m.Groups[1].Value.Trim();
            cmd.Promt = m.Groups[2].Value.Trim();
            return cmd;
        }

        public CmdOptionGroup(ReadContext readcontext) : base(readcontext)
        {
            Key = string.Empty;
            Promt = string.Empty;
            SelectedOptionCommand = null;
        }

        public override CmdBaseCommand? Run(RunContext rc)
        {
            CmdOptionValue? firstOptionValue = GetNextCommand<CmdOptionValue>(c => true, c => c.Indent < this.Indent);
            if (firstOptionValue == null)
            {
                rc.SetError(ReadContext, "Option ohne Werte",
                    "Für die Optionsgruppe wurde kein Wert angegeben. Die Ausführung wird abgebrochen.");
                return null;
            }
            IMOptionGroupElement ogroup = new IMOptionGroupElement();
            string expandedKey = ReplaceVars(rc, Key).Trim();
            if (expandedKey.EndsWith("'"))
            {
                expandedKey = expandedKey.Replace("'", "_" + ReadContext.LineNumber.ToString());
            }
            ogroup.Key = expandedKey;
            ogroup.Promt = ReplaceVars(rc, Promt).Trim();

            string selectedKey = string.Empty;
            if (rc.BoundVars.ContainsKey(expandedKey))
            {
                selectedKey = rc.BoundVars[expandedKey];
            }

            CmdOptionValue? xOption = null;
            var allOptions = GetNexCommands<CmdOptionValue>(
                    c => c.Indent == firstOptionValue.Indent,
                    c => c.Indent < firstOptionValue.Indent);
            foreach (var idxo in allOptions)
            {
                IMOptionValue ov = new IMOptionValue(ogroup);
                ov.Key = idxo.Key;
                ov.Promt = ReplaceVars(rc, idxo.Promt);
                ogroup.Options.Add(ov);
                if (!string.IsNullOrEmpty(selectedKey) && ov.Key == selectedKey)
                {
                    SelectedOptionCommand = idxo;
                }
                if (ov.Key == "x")
                {
                    xOption = idxo;
                }
                idxo.ParentOptionGroupCommand = this;
            }
            if (SelectedOptionCommand == null)
            {
                SelectedOptionCommand = xOption;
            }
            if (SelectedOptionCommand != null)
            {
                rc.GivenKeys.Add(expandedKey);
            }
            else
            {
                rc.BoundVars[expandedKey] = string.Empty;
                rc.InputItems.Add(ogroup);
                AssociatedInputElement = ogroup;
            }
            return NextCommand;
        }
    }
}