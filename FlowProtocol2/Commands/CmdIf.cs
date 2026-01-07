namespace FlowProtocol2.Commands
{
    using System.Text.RegularExpressions;
    using FlowProtocol2.Core;

    /// <summary>
    /// Implementiert den If-Befehl
    /// </summary>
    public class CmdIf : CmdBaseCommand
    {
        public string Expression { get; set; }
        public bool Evaluation { get; set; }
        /// <summary>
        /// Verwaltet das Ergebnis der If- und ElseIf-Bedingungen mit Bezug zur BaseKey-Eigenschaft (wg. Rekursionen)
        /// </summary>
        public Dictionary<string, bool> Handled { get; set; }

        public static CommandParser GetComandParser()
        {
            return new CommandParser(@"^~If (.*)", (rc, m) => CreateIfCommand(rc, m));
        }

        private static CmdBaseCommand CreateIfCommand(ReadContext rc, Match m)
        {
            CmdIf cmd = new CmdIf(rc);
            cmd.Expression = m.Groups[1].Value.Trim();
            return cmd;
        }

        public CmdIf(ReadContext readcontext) : base(readcontext)
        {
            Expression = string.Empty;
            Evaluation = false;
            Handled = new Dictionary<string, bool>();
        }

        public override CmdBaseCommand? Run(RunContext rc)
        {
            try
            {
                Handled[rc.BaseKey] = false;
                Evaluation = EvaluateExpression(rc, Expression, out ErrorElement? err);
                if (err != null)
                {
                    rc.ErrorItems.Add(err);
                    return null;
                }
                CmdElse? associatedElseCommand = GetNextCommand<CmdElse>(
                    c => c.Indent == this.Indent,
                    c => c.Indent < this.Indent || (c.Indent == this.Indent && c is CmdIf));
                if (associatedElseCommand != null)
                {
                    associatedElseCommand.ParentIfCommand = this;
                }
                var associatedElseIfCommands = GetNextCommands<CmdElseIf>(
                    c => c.Indent == this.Indent,
                    c => c.Indent < this.Indent || (c.Indent == this.Indent && (c is CmdIf || c is CmdElse)));
                foreach (var c in associatedElseIfCommands)
                {
                    c.ParentIfCommand = this;
                }
                if (Evaluation)
                {
                    Handled[rc.BaseKey] = true;
                    return NextCommand;
                }
                return GetNextSameOrHigherLevelCommand();
            }
            catch (Exception ex)
            {
                rc.SetError(ReadContext, "Verarbeitungsfehler",
                    $"Beim Ausführen des Skriptes ist ein Fehler aufgetreten '{ex.Message}'. Die Ausführung wird abgebrochen."
                    + $"Variablenwerte: Expression='{Expression}'");
                return null;
            }
        }
    }
}