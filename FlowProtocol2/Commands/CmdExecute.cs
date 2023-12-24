namespace FlowProtocol2.Commands
{
    using System.Text.RegularExpressions;
    using FlowProtocol2.Core;

    /// <summary>
    /// Implementiert den Execute-Befehl
    /// </summary>
    public class CmdExecute : CmdBaseCommand
    {
        public static CommandParser GetComandParser()
        {
            return new CommandParser(@"^~Execute", (rc, m) => CreateExecuteCommand(rc, m));
        }

        private static CmdBaseCommand CreateExecuteCommand(ReadContext rc, Match m)
        {
            CmdExecute cmd = new CmdExecute(rc);
            return cmd;
        }

        public CmdExecute(ReadContext readcontext) : base(readcontext)
        {

        }

        public override CmdBaseCommand? Run(RunContext rc)
        {
            if (rc.InputItems.Any())
            {
                rc.ExecuteNow = true;
            }
            return NextCommand;
        }
    }
}