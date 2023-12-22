namespace FlowProtocol2.Commands
{
    using System.Text.RegularExpressions;
    using FlowProtocol2.Core;

    /// <summary>
    /// Implementiert den SetSection-Befehl
    /// </summary>
    public class CmdSetSection : CmdBaseCommand
    {
        public string Headline { get; set; }

        public static CommandParser GetComandParser()
        {
            return new CommandParser(@"^~SetSection (.*)", (rc, m) => CreateSetSectionCommand(rc, m));
        }

        private static CmdBaseCommand CreateSetSectionCommand(ReadContext rc, Match m)
        {
            CmdSetSection cmd = new CmdSetSection(rc);
            cmd.Headline = m.Groups[1].Value.Trim();
            return cmd;
        }

        public CmdSetSection(ReadContext readcontext) : base(readcontext)
        {
            Headline = string.Empty;
        }

        public override CmdBaseCommand? Run(RunContext rc)
        {
            rc.DocumentBuilder.CurrentSection = Headline;
            return NextCommand;
        }
    }
}