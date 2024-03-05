namespace FlowProtocol2.Commands
{
    using System.Text.RegularExpressions;
    using FlowProtocol2.Core;

    /// <summary>
    /// Implementiert den EvalCond-Befehl
    /// </summary>
    public class CmdEvalExpression : CmdBaseCommand
    {
        public string VarName { get; set; }
        public string Negation { get; set; }
        public string Expression { get; set; }

        public static CommandParser GetComandParser()
        {
            return new CommandParser(@"^~EvalExpression ([A-Za-z0-9\$\(\)]*)\s*(!?)=(.*)", (rc, m) => CreateEvalCondCommand(rc, m));
        }

        private static CmdBaseCommand CreateEvalCondCommand(ReadContext rc, Match m)
        {
            CmdEvalExpression cmd = new CmdEvalExpression(rc);
            cmd.VarName = m.Groups[1].Value.Trim();
            cmd.Negation = m.Groups[2].Value;
            cmd.Expression = m.Groups[3].Value.Trim();
            return cmd;
        }

        public CmdEvalExpression(ReadContext readcontext) : base(readcontext)
        {
            VarName = string.Empty;
            Negation = string.Empty;
            Expression = string.Empty;
        }

        public override CmdBaseCommand? Run(RunContext rc)
        {
            string expandedVarName = ReplaceVars(rc, VarName);
            bool evaluation = EvaluateExpression(rc, Expression, out ErrorElement? err);
            if (err != null)
            {
                rc.ErrorItems.Add(err);
                return null;
            }
            if (Negation == "!") evaluation = !evaluation;
            rc.InternalVars[expandedVarName] = BoolString(evaluation);
            return NextCommand;
        }
    }
}