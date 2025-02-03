namespace FlowProtocol2.Commands
{
    using System.Text.RegularExpressions;
    using FlowProtocol2.Core;

    /// <summary>
    /// Implementiert den DoWhile-Befehl
    /// </summary>
    public class CmdDoWhile : CmdLoopBaseCommand
    {
        public string Expression { get; set; }
        public bool Evaluation { get; set; }

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
            LinkAssociatedLoopCommand(rc, "DoWhile");
            if (AssociatedLoopCommand != null)
            {                
                if (Evaluation)
                {
                    IsInitialized = true;
                    return NextCommand;
                }
                else
                {
                    IsInitialized = false;
                    return AssociatedLoopCommand.NextCommand;
                }
            }
            rc.SetError(ReadContext, "DoWhile ohne Loop",
                "Dem DoWhile-Befehl kann kein Loop-Befehl auf gleicher Ebene zugeordnet werden. Die Bearbeitung wird abgebrochen.");
            return null;
        }
    }
}