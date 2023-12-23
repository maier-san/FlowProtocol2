namespace FlowProtocol2.Commands
{
    using System.Text.RegularExpressions;
    using FlowProtocol2.Core;

    /// <summary>
    /// Implementiert den Else-Befehl
    /// </summary>
    public class CmdElse : CmdBaseCommand
    {
        public CmdIf? ParentIfCommand {get; set;}

        public static CommandParser GetComandParser()
        {
            return new CommandParser(@"^~Else", (rc, m) => CreateElseCommand(rc, m));
        }

        private static CmdBaseCommand CreateElseCommand(ReadContext rc, Match m)
        {
            CmdElse cmd = new CmdElse(rc);
            return cmd;
        }

        public CmdElse(ReadContext readcontext) : base(readcontext)
        {
            ParentIfCommand = null;
        }

        public override CmdBaseCommand? Run(RunContext rc)
        {
            if (ParentIfCommand == null)
            {
                rc.SetError(ReadContext, "Else ohne If",
                    "Dem Else-Befehl kann kein If-Befehl zugeordnet werden. Prüfen Sie die Einrückung.");
                return GetNextSameOrHigherLevelCommand();
            }
            if (!ParentIfCommand.Evaluation)
            {
                return NextCommand;
            }
            return GetNextSameOrHigherLevelCommand();
        }
    }
}