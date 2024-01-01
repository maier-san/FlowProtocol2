namespace FlowProtocol2.Commands
{
    using System.Text.RegularExpressions;
    using FlowProtocol2.Core;

    /// <summary>
    /// Implementiert den End-Befehl
    /// </summary>
    public class CmdEnd : CmdBaseCommand
    {

        public static CommandParser GetComandParser()
        {
            return new CommandParser(@"^~End", (rc, m) => CreateEndCommand(rc, m));
        }

        private static CmdBaseCommand CreateEndCommand(ReadContext rc, Match m)
        {
            CmdEnd cmd = new CmdEnd(rc);
            return cmd;
        }

        public CmdEnd(ReadContext readcontext) : base(readcontext)
        {
        }

        public override CmdBaseCommand? Run(RunContext rc)
        {
            rc.ReturnStack.Clear();
            return null;
        }
    }
}