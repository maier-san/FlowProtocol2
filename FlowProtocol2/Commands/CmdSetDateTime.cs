namespace FlowProtocol2.Commands
{
    using System.Text.RegularExpressions;
    using FlowProtocol2.Core;

    /// <summary>
    /// Implementiert den SetDateTime-Befehl
    /// </summary>
    public class CmdSetDateTime : CmdBaseCommand
    {
        public string VarName { get; set; }
        public string FormatString { get; set; }

        public static CommandParser GetComandParser()
        {
            return new CommandParser(@"^~SetDateTime ([A-Za-z0-9\$]*)\s*=(.*)", (rc, m) => CreateSetDateTimeCommand(rc, m));
        }

        private static CmdBaseCommand CreateSetDateTimeCommand(ReadContext rc, Match m)
        {
            CmdSetDateTime cmd = new CmdSetDateTime(rc);
            cmd.VarName = m.Groups[1].Value.Trim();
            cmd.FormatString = m.Groups[2].Value.Trim();
            return cmd;
        }

        public CmdSetDateTime(ReadContext readcontext) : base(readcontext)
        {
            VarName = string.Empty;
            FormatString = string.Empty;
        }

        public override CmdBaseCommand? Run(RunContext rc)
        {
            string expandedFormatString = ReplaceVars(rc, FormatString);
            string result = DateTime.Now.ToString(expandedFormatString);
            rc.InternalVars[VarName] = result;
            return NextCommand;
        }
    }
}