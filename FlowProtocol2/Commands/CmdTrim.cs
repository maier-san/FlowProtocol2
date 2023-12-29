namespace FlowProtocol2.Commands
{
    using System.Text.RegularExpressions;
    using FlowProtocol2.Core;

    /// <summary>
    /// Implementiert den Trim-Befehl
    /// </summary>
    public class CmdTrim : CmdBaseCommand
    {
        public string VarName { get; set; }
        public string Text { get; set; }
        

        public static CommandParser GetComandParser()
        {
            return new CommandParser(@"^~Trim ([A-Za-z0-9\$\(\)]*)\s*=(.*)", (rc, m) => CreateTrimCommand(rc, m));
        }

        private static CmdBaseCommand CreateTrimCommand(ReadContext rc, Match m)
        {
            CmdTrim cmd = new CmdTrim(rc);
            cmd.VarName = m.Groups[1].Value.Trim();
            cmd.Text = m.Groups[2].Value.Trim();
            // ToDo: Weitere Eigenschaften auslesen
            return cmd;
        }

        public CmdTrim(ReadContext readcontext) : base(readcontext)
        {
            VarName = string.Empty;
            Text = string.Empty;            
        }

        public override CmdBaseCommand? Run(RunContext rc)
        {
            string expandedVarName = ReplaceVars(rc, VarName);
            string expandedText = ReplaceVars(rc, Text);
            rc.InternalVars[expandedVarName] = expandedText.Trim();
            return NextCommand;
        }
    }
}