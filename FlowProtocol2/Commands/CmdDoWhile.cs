namespace FlowProtocol2.Commands
{
    using System.Text.RegularExpressions;
    using FlowProtocol2.Core;

    /// <summary>
    /// Implementiert den DoWhile-Befehl
    /// </summary>
    public class CmdDoWhile : CmdBaseCommand
    {
        public string Expression { get; set; }
        public bool Evaluation { get; set; }
        public CmdLoop? AssociatedLoopCommand { get; set; }

        public static CommandParser GetComandParser()
        {
            return new CommandParser(@"^~DoWhile (.*)", (rc, m) => CreateDoWhileCommand(rc, m));
        }

        private static CmdBaseCommand CreateDoWhileCommand(ReadContext rc, Match m)
        {
            CmdDoWhile cmd = new CmdDoWhile(rc);
            cmd.Expression = m.Groups[1].Value.Trim();
            return cmd;
        }

        public CmdDoWhile(ReadContext readcontext) : base(readcontext)
        {
            Expression = string.Empty;
            Evaluation = false;
            AssociatedLoopCommand = null;
        }

        public override CmdBaseCommand? Run(RunContext rc)
        {
            Evaluation = EvaluateExpression(rc, Expression, out ErrorElement? err);
            if (err != null)
            {
                rc.ErrorItems.Add(err);
                return null;
            }
            if (AssociatedLoopCommand == null)
            {
                AssociatedLoopCommand = GetNextCommand<CmdLoop>(
                    c => c.Indent == this.Indent,
                    c => c.Indent < this.Indent);
            }
            if (AssociatedLoopCommand != null)
            {
                AssociatedLoopCommand.ParentDoWhileCommand = this;
                if (Evaluation)
                {
                    return NextCommand;
                }
                else
                {
                    return AssociatedLoopCommand.NextCommand;
                }
            }
            rc.SetError(ReadContext, "WhileDo ohne Loop",
                "Dem WhileDo-Befehl kann kein Loop-Befehl auf gleicher Ebene zugeordnet werden. Die Bearbeitung wird abgebrochen.");
            return null;
        }
    }
}