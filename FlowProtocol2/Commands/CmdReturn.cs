namespace FlowProtocol2.Commands
{
    using System.Text.RegularExpressions;
    using FlowProtocol2.Core;

    /// <summary>
    /// Implementiert den Return-Befehl
    /// </summary>
    public class CmdReturn : CmdBaseCommand
    {
        public static CommandParser GetComandParser()
        {
            return new CommandParser(@"^~Return", (rc, m) => CreateReturnCommand(rc, m));
        }

        private static CmdBaseCommand CreateReturnCommand(ReadContext rc, Match m)
        {
            CmdReturn cmd = new CmdReturn(rc);
            return cmd;
        }

        public CmdReturn(ReadContext readcontext) : base(readcontext)
        {
        }

        public override CmdBaseCommand? Run(RunContext rc)
        {
            return null;
        }
    }
}