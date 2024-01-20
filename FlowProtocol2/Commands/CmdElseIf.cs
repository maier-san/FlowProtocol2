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
            if (!ParentIfCommand.Handled.ContainsKey(rc.BaseKey))
            {
                rc.SetError(ReadContext, "ElseIf ohne bestimmbare If-Bedingung",
                    "Für den zum ElseIf-Befehl gehörenden If-Befehl kann kein Bedingungswert ermittelt werden.");
                return GetNextSameOrHigherLevelCommand();
            }
            if (!ParentIfCommand.Handled[rc.BaseKey])
            {
                bool evaluation = EvaluateExpression(rc, Expression, out ErrorElement? err);
                if (err != null)
                {
                    rc.ErrorItems.Add(err);
                    return null;
                }
                if (evaluation)
                {
                    ParentIfCommand.Handled[rc.BaseKey] = true;
                    return NextCommand;
                }                
            }
            return GetNextSameOrHigherLevelCommand();
        }
    }
}