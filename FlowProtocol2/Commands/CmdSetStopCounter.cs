namespace FlowProtocol2.Commands
{
    using System.Text.RegularExpressions;
    using FlowProtocol2.Core;

    /// <summary>
    /// Implementiert den SetStopCounter-Befehl
    /// </summary>
    public class CmdSetStopCounter : CmdBaseCommand
    {
        public string LoopStop { get; set; }
        public string CommandStop { get; set; }

        public static CommandParser GetComandParser()
        {
            return new CommandParser(@"^~SetStopCounter ([A-Za-z0-9\$\(\)]*)\s*;\s*([A-Za-z0-9\$\(\)]*)", (rc, m) => CreateSetStopCounterCommand(rc, m));
        }

        private static CmdBaseCommand CreateSetStopCounterCommand(ReadContext rc, Match m)
        {
            CmdSetStopCounter cmd = new CmdSetStopCounter(rc);
            cmd.LoopStop = m.Groups[1].Value.Trim();
            cmd.CommandStop = m.Groups[2].Value.Trim();
            return cmd;
        }

        public CmdSetStopCounter(ReadContext readcontext) : base(readcontext)
        {
            LoopStop = string.Empty;
            CommandStop = string.Empty;
        }

        public override CmdBaseCommand? Run(RunContext rc)
        {
            string expandedLoopStop = ReplaceVars(rc, LoopStop);
            bool loopOK = Int32.TryParse(expandedLoopStop, out int loopstop);
            if (loopOK)
            {
                rc.LoopStopCounter = loopstop;
            }
            else
            {
                rc.SetError(ReadContext, "Ungültiger numerischer Ausdruck",
                        $"Der Ausdruck '{expandedLoopStop}' kann nicht als ganze Zahl interpretiert werden.");
            }
            string expandedCommandStop = ReplaceVars(rc, CommandStop);
            bool comOK = Int32.TryParse(expandedCommandStop, out int commandstop);
            if (comOK)
            {
                rc.CommandStopCounter = commandstop;
            }
            else
            {
                rc.SetError(ReadContext, "Ungültiger numerischer Ausdruck",
                        $"Der Ausdruck '{expandedCommandStop}' kann nicht als ganze Zahl interpretiert werden.");
            }
            return NextCommand;
        }
    }
}