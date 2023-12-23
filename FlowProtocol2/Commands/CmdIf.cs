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
        }

        public override CmdBaseCommand? Run(RunContext rc)
        {
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
            if (Evaluation)
            {
                return NextCommand;
            }
            return GetNextSameOrHigherLevelCommand();
        }
    }
}