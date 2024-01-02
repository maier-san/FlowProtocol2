namespace FlowProtocol2.Commands
{
    using System.Text.RegularExpressions;
    using FlowProtocol2.Core;

    /// <summary>
    /// Implementiert den GoTo-Befehl
    /// </summary>
    public class CmdGoTo : CmdBaseCommand
    {
        public string JumpMark { get; set; }

        public static CommandParser GetComandParser()
        {
            return new CommandParser(@"^~GoTo ([A-Za-z0-9\$\(\)]*)", (rc, m) => CreateGoToCommand(rc, m));
        }

        private static CmdBaseCommand CreateGoToCommand(ReadContext rc, Match m)
        {
            CmdGoTo cmd = new CmdGoTo(rc);
            cmd.JumpMark = m.Groups[1].Value.Trim();
            return cmd;
        }

        public CmdGoTo(ReadContext readcontext) : base(readcontext)
        {
            JumpMark = string.Empty;
        }

        public override CmdBaseCommand? Run(RunContext rc)
        {
            string expandedJumpMark = ReplaceVars(rc, JumpMark);
            CmdJumpMark? jump = GetFirstCommand<CmdJumpMark>(c => c.Mark == expandedJumpMark, c => false);
            if (jump == null)
            {
                rc.SetError(ReadContext, "Sprungziel nicht gefunden",
                    $"Das Sprungziel '{expandedJumpMark}' konnte nicht gefunden werden. Die Skriptausführung wird abgebrochen.");
                return null;
            }
            return jump;
        }
    }
}