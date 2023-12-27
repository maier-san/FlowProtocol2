namespace FlowProtocol2.Commands
{
    using System.Text.RegularExpressions;

    /// <summary>
    /// Implementiert den SetSection-Befehl
    /// </summary>
    public class CmdSetSection : CmdBaseCommand
    {
        public string Section { get; set; }

        public static CommandParser GetComandParser()
        {
            return new CommandParser(@"^~SetSection (.*)", (rc, m) => CreateSetSectionCommand(rc, m));
        }

        private static CmdBaseCommand CreateSetSectionCommand(ReadContext rc, Match m)
        {
            CmdSetSection cmd = new CmdSetSection(rc);
            cmd.Section = m.Groups[1].Value.Trim();
            return cmd;
        }

        public CmdSetSection(ReadContext readcontext) : base(readcontext)
        {
            Section = string.Empty;
        }

        public override CmdBaseCommand? Run(RunContext rc)
        {
            string expandedSection = ReplaceVars(rc, Section).Trim();
            rc.DocumentBuilder.CurrentSection = expandedSection;
            return NextCommand;
        }
    }
}