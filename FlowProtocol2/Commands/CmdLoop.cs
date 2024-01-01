using System.Text.RegularExpressions;
using FlowProtocol2.Core;

namespace FlowProtocol2.Commands
{
    /// <summary>
    /// Implementiert den Loop-Befehl
    /// </summary>
    public class CmdLoop : CmdBaseCommand
    {
        public CmdLoopBaseCommand? ParentDoWhileCommand { get; set; }
        private int StopCounter { get; set; }
        private const int MaximalLoopCount = 1000;

        public static CommandParser GetComandParser()
        {
            return new CommandParser(@"^~Loop", (rc, m) => CreateLoopCommand(rc, m));
        }

        private static CmdBaseCommand CreateLoopCommand(ReadContext rc, Match m)
        {
            CmdLoop cmd = new CmdLoop(rc);
            return cmd;
        }

        public CmdLoop(ReadContext readcontext) : base(readcontext)
        {
            ParentDoWhileCommand = null;
            StopCounter = 0;
        }

        public override CmdBaseCommand? Run(RunContext rc)
        {
            StopCounter++;
            if (ParentDoWhileCommand == null)
            {
                rc.SetError(ReadContext, "Loop ohne Schleifenbeginn",
                    "Dem Loop-Befehl kann kein Schleifenbeginn-Befehl auf gleicher Ebene zugeordnet werden.");
            }
            if (StopCounter > MaximalLoopCount)
            {
                rc.SetError(ReadContext, "Maximale Anzahl Schleifendurchläufe erreicht",
                    $"Die maximale Anzahl von {MaximalLoopCount} direkten Schleifendurchläufen wurde erreicht. Die Schleife wird beendet.");
                return NextCommand;
            }
            return ParentDoWhileCommand;
        }
    }
}