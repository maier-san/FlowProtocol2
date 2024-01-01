namespace FlowProtocol2.Commands
{
    using System.Text.RegularExpressions;
    using FlowProtocol2.Core;

    /// <summary>
    /// Implementiert den ElseIf-Befehl
    /// </summary>
    public class CmdElseIf : CmdBaseCommand
    {
        public string Expression { get; set; }
        public CmdIf? ParentIfCommand { get; set; }

        public static CommandParser GetComandParser()
        {
            return new CommandParser(@"^~ElseIf (.*)", (rc, m) => CreateElseIfCommand(rc, m));
        }

        private static CmdBaseCommand CreateElseIfCommand(ReadContext rc, Match m)
        {
            CmdElseIf cmd = new CmdElseIf(rc);
            cmd.Expression = m.Groups[1].Value.Trim();
            return cmd;
        }

        public CmdElseIf(ReadContext readcontext) : base(readcontext)
        {
            Expression = string.Empty;
            ParentIfCommand = null;
        }

        public override CmdBaseCommand? Run(RunContext rc)
        {
            string expandedExpression = ReplaceVars(rc, Expression);
            if (ParentIfCommand == null)
            {
                rc.SetError(ReadContext, "ElseIf ohne If",
                    "Dem ElseIf-Befehl kann kein If-Befehl zugeordnet werden. Prüfen Sie die Einrückung.");
                return GetNextSameOrHigherLevelCommand();
            }
            if (!ParentIfCommand.Handled)
            {
                bool evaluation = EvaluateExpression(rc, Expression, out ErrorElement? err);
                if (err != null)
                {
                    rc.ErrorItems.Add(err);
                    return null;
                }
                if (evaluation)
                {
                    ParentIfCommand.Handled = true;
                    return NextCommand;
                }                
            }
            return GetNextSameOrHigherLevelCommand();
        }
    }
}