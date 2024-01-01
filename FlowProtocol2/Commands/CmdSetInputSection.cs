namespace FlowProtocol2.Commands
{
    using System.Text.RegularExpressions;
    using FlowProtocol2.Core;

    /// <summary>
    /// Implementiert den SetInputSection-Befehl
    /// </summary>
    public class CmdSetInputSection : CmdBaseCommand
    {
        public string Section { get; set; }

        public static CommandParser GetComandParser()
        {
            return new CommandParser(@"^~SetInputSection (.*)", (rc, m) => CreateSetInputSectionCommand(rc, m));
        }

        private static CmdBaseCommand CreateSetInputSectionCommand(ReadContext rc, Match m)
        {
            CmdSetInputSection cmd = new CmdSetInputSection(rc);
            cmd.Section = m.Groups[1].Value.Trim();
            return cmd;
        }

        public CmdSetInputSection(ReadContext readcontext) : base(readcontext)
        {
            Section = string.Empty;
        }

        public override CmdBaseCommand? Run(RunContext rc)
        {
            string expandedSection = ReplaceVars(rc, Section);
            rc.InputForm.CurrentSection = expandedSection;
            return NextCommand;
        }
    }
}