namespace FlowProtocol2.Commands
{
    using System.Text.RegularExpressions;
    using FlowProtocol2.Core;

    /// <summary>
    /// Implementiert den Split-Befehl
    /// </summary>
    public class CmdSplit : CmdBaseCommand
    {
        public string VarName { get; set; }
        public string Value { get; set; }
        public string SplitChar { get; set; }

        public static CommandParser GetComandParser()
        {
            return new CommandParser(@"^~Split ([A-Za-z0-9\$\(\)]*)\s*=(.*)\|(.*)", (rc, m) => CreateSplitCommand(rc, m));
        }

        private static CmdBaseCommand CreateSplitCommand(ReadContext rc, Match m)
        {
            CmdSplit cmd = new CmdSplit(rc);
            cmd.VarName = m.Groups[1].Value.Trim();
            cmd.Value = m.Groups[2].Value.Trim();
            cmd.SplitChar = m.Groups[3].Value.Trim();
            return cmd;
        }

        public CmdSplit(ReadContext readcontext) : base(readcontext)
        {
            VarName = string.Empty;
            Value = string.Empty;
            SplitChar = string.Empty;
        }

        public override CmdBaseCommand? Run(RunContext rc)
        {
            string expandedVarName = ReplaceVars(rc, VarName);
            if (!string.IsNullOrEmpty(expandedVarName))
            {
                string expandedValue = ReplaceVars(rc, Value);
                string expandedSplitChar = ReplaceVars(rc, SplitChar);
                var splitlist = expandedValue.Split(expandedSplitChar).ToList();
                int index = 0;
                foreach (var idx in splitlist)
                {
                    index++;
                    rc.InternalVars[$"{expandedVarName}({index})"] = idx.Trim();
                }
                index++;
                rc.InternalVars.Remove($"{expandedVarName}({index})");
            }
            return NextCommand;
        }
    }
}