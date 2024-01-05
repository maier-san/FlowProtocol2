namespace FlowProtocol2.Commands
{
    using System.Text.RegularExpressions;
    using FlowProtocol2.Core;

    /// <summary>
    /// Implementiert den AddNewKey-Befehl
    /// </summary>
    public class CmdAddNewKey : CmdBaseCommand
    {
        public string Key { get; set; }
        public string Value { get; set; }

        public static CommandParser GetComandParser()
        {
            return new CommandParser(@"^~AddNewKey ([A-Za-z0-9\$\(\)]*)\s*=(.*)", (rc, m) => CreateAddNewKeyCommand(rc, m));
        }

        private static CmdBaseCommand CreateAddNewKeyCommand(ReadContext rc, Match m)
        {
            CmdAddNewKey cmd = new CmdAddNewKey(rc);
            cmd.Key = m.Groups[1].Value.Trim();
            cmd.Value = m.Groups[2].Value.Trim();
            return cmd;
        }

        public CmdAddNewKey(ReadContext readcontext) : base(readcontext)
        {
            Key = string.Empty;
            Value = string.Empty;
        }

        public override CmdBaseCommand? Run(RunContext rc)
        {
            string expandedKey = ReplaceVars(rc, Key);
            string plainKey = expandedKey;
            string expandedValue = ReplaceVars(rc, Value);
            if (!string.IsNullOrEmpty(rc.BaseKey))
            {
                expandedKey = rc.BaseKey + "_" + expandedKey;
            }
            if (!rc.BoundVars.ContainsKey(expandedKey))
            {
                rc.BoundVars[expandedKey] = expandedValue;                
            }
            rc.GivenKeys.Add(expandedKey);
            rc.InternalVars[plainKey] = rc.BoundVars[expandedKey];
            return NextCommand;
        }
    }
}