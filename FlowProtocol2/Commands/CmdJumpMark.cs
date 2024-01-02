namespace FlowProtocol2.Commands
{
    using System.Text.RegularExpressions;
    using FlowProtocol2.Core;

    /// <summary>
    /// Implementiert den JumpMark-Befehl
    /// </summary>
    public class CmdJumpMark : CmdBaseCommand
    {
        public string Mark { get; set; }

        public static CommandParser GetComandParser()
        {
            return new CommandParser(@"^~JumpMark ([A-Za-z0-9]*)", (rc, m) => CreateJumpMarkCommand(rc, m));
        }

        private static CmdBaseCommand CreateJumpMarkCommand(ReadContext rc, Match m)
        {
            CmdJumpMark cmd = new CmdJumpMark(rc);
            cmd.Mark = m.Groups[1].Value.Trim();
            return cmd;
        }

        public CmdJumpMark(ReadContext readcontext) : base(readcontext)
        {
            Mark = string.Empty;
        }

        public override CmdBaseCommand? Run(RunContext rc)
        {
            return NextCommand;
        }
    }
}