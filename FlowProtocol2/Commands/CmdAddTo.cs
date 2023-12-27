namespace FlowProtocol2.Commands
{
    using System.Text.RegularExpressions;
    using FlowProtocol2.Core;

    /// <summary>
    /// Implementiert den AddTo-Befehl
    /// </summary>
    public class CmdAddTo : CmdBaseCommand
    {
        public string VarName { get; set; }
        public string Value { get; set; }

        public static CommandParser GetComandParser()
        {
            return new CommandParser(@"^~AddTo ([A-Za-z0-9$]*)\s*\+=\s*(-?[A-Za-z0-9$]*)", (rc, m) => CreateAddToCommand(rc, m));
        }

        private static CmdBaseCommand CreateAddToCommand(ReadContext rc, Match m)
        {
            CmdAddTo cmd = new CmdAddTo(rc);
            cmd.VarName = m.Groups[1].Value.Trim();
            cmd.Value = m.Groups[2].Value.Trim();
            return cmd;
        }

        public CmdAddTo(ReadContext readcontext) : base(readcontext)
        {
            VarName = string.Empty;
            Value = string.Empty;
        }

        public override CmdBaseCommand? Run(RunContext rc)
        {
            string expandedVarName = ReplaceVars(rc, VarName);
            string expandedValue = ReplaceVars(rc, Value);
            if (!rc.InternalVars.ContainsKey(expandedVarName))
            {
                rc.InternalVars[expandedVarName] = "0";
            }
            bool evOK = Int32.TryParse(expandedValue, out int iValue);
            if (!evOK)
            {
                rc.SetError(ReadContext, "Ungültiger numerischer Ausdruck",
                    $"Der Ausdruck '{expandedValue}' kann nicht als ganze Zahl interpretiert werden.");
            }
            string expandedVarValue = ReplaceVars(rc, rc.InternalVars[expandedVarName]).Trim();
            bool evvOK = Int32.TryParse(expandedVarValue, out int iVarValue);
            if (!evvOK)
            {
                rc.SetError(ReadContext, "Ungültiger numerischer Ausdruck",
                    $"Der Ausdruck '{expandedVarValue}' kann nicht als ganze Zahl interpretiert werden.");
            }
            if (evOK && evvOK)
            {
                rc.InternalVars[expandedVarName] = (iValue + iVarValue).ToString();
            }
            return NextCommand;
        }
    }
}