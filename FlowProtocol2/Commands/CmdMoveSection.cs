namespace FlowProtocol2.Commands
{
    using System.Text.RegularExpressions;
    using FlowProtocol2.Core;

    /// <summary>
    /// Implementiert den MoveSection-Befehl
    /// </summary>
    public class CmdMoveSection : CmdBaseCommand
    {
        public string FromSection { get; set; }
        public string ToSection { get; set; }

        public static CommandParser GetComandParser()
        {
            return new CommandParser(@"^~MoveSection (.*)\s*->\s*(.*)", (rc, m) => CreateMoveSectionCommand(rc, m));
        }

        private static CmdBaseCommand CreateMoveSectionCommand(ReadContext rc, Match m)
        {
            CmdMoveSection cmd = new CmdMoveSection(rc);
            cmd.FromSection = m.Groups[1].Value.Trim();
            cmd.ToSection = m.Groups[2].Value.Trim();
            return cmd;
        }

        public CmdMoveSection(ReadContext readcontext) : base(readcontext)
        {
            FromSection = string.Empty;
            ToSection = string.Empty;
        }

        public override CmdBaseCommand? Run(RunContext rc)
        {
            string expandedFromSection = ReplaceVars(rc, FromSection);
            string expandedToSection = ReplaceVars(rc, ToSection);
            rc.DocumentBuilder.MoveSection(expandedFromSection, expandedToSection);
            return NextCommand;
        }
    }
}