namespace FlowProtocol2.Commands
{
    using System.Text.RegularExpressions;
    using FlowProtocol2.Core;

    /// <summary>
    /// Implementiert den RegExMatch-Befehl
    /// </summary>
    public class CmdRegExMatch : CmdBaseCommand
    {
        public string VarName { get; set; }
        public string Text { get; set; }
        public string Expression { get; set; }

        public static CommandParser GetComandParser()
        {
            return new CommandParser(@"^~RegExMatch ([A-Za-z0-9\$\(\)]*)\s*=([^\|]*)\|(.*)", (rc, m) => CreateRegExMatchCommand(rc, m));
        }

        private static CmdBaseCommand CreateRegExMatchCommand(ReadContext rc, Match m)
        {
            CmdRegExMatch cmd = new CmdRegExMatch(rc);
            cmd.VarName = m.Groups[1].Value.Trim();
            cmd.Text = m.Groups[2].Value.Trim();
            cmd.Expression = m.Groups[3].Value;
            return cmd;
        }

        public CmdRegExMatch(ReadContext readcontext) : base(readcontext)
        {
            VarName = string.Empty;
            Text = string.Empty;
            Expression = string.Empty;
        }

        public override CmdBaseCommand? Run(RunContext rc)
        {
            string expandedVarName = ReplaceVars(rc, VarName);
            string expandedText = ReplaceVars(rc, Text);
            string expandedExpression = ReplaceVars(rc, Expression);
            try
            {
                Regex userExp = new Regex(expandedExpression);
                if (userExp.IsMatch(expandedText))
                {
                    rc.InternalVars[expandedVarName + "(0)"] = "true";
                    Match m = userExp.Match(expandedText);
                    for (var g = 1; g < m.Groups.Count; g++)
                    {
                        rc.InternalVars[expandedVarName + "(" + g.ToString() + ")"] = m.Groups[g].Value;
                    }
                    rc.InternalVars.Remove(expandedVarName + "(" + m.Groups.Count + ")");
                }
                else
                {
                    rc.InternalVars[expandedVarName + "(0)"] = "false";
                    rc.InternalVars.Remove(expandedVarName + "(1)");
                }
            }
            catch (Exception ex)
            {
                rc.SetError(ReadContext, "RegEx-Ausnahme",
                    $"Beim Verarbeiten des regulären Ausdrucks '{expandedExpression}' kam es zur Ausnahme '{ex.Message}'. Die Skriptausführung wird abgebrochen.");
                return null;
            }
            return NextCommand;
        }
    }
}