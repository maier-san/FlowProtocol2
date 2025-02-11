namespace FlowProtocol2.Commands
{
    using System.Text.RegularExpressions;
    using FlowProtocol2.Core;

    /// <summary>
    /// Implementiert den RegExReplace-Befehl
    /// </summary>
    public class CmdRegExReplace : CmdBaseCommand
    {
        public string VarName { get; set; }
        public string Text { get; set; }
        public string Expression { get; set; }
        public string ReplaceBy { get; set; }

        public static CommandParser GetComandParser()
        {
            return new CommandParser(@"^~RegExReplace ([A-Za-z0-9\$\(\)]*)\s*=(.*)\|(.*)->(.*)", (rc, m) => CreateRegExReplaceCommand(rc, m));
        }

        private static CmdBaseCommand CreateRegExReplaceCommand(ReadContext rc, Match m)
        {
            CmdRegExReplace cmd = new CmdRegExReplace(rc);
            cmd.VarName = m.Groups[1].Value.Trim();
            cmd.Text = m.Groups[2].Value.Trim();
            cmd.Expression = m.Groups[3].Value.Trim();
            cmd.ReplaceBy = m.Groups[4].Value.Trim();
            return cmd;
        }

        public CmdRegExReplace(ReadContext readcontext) : base(readcontext)
        {
            VarName = string.Empty;
            Text = string.Empty;
            Expression = string.Empty;
            ReplaceBy = string.Empty;
        }

        public override CmdBaseCommand? Run(RunContext rc)
        {
            string expandedVarName = ReplaceVars(rc, VarName);
            string expandedText = ReplaceVars(rc, Text);
            string expandedExpression = Regex.Escape(ReplaceVars(rc, Expression));
            string expandedReplaceBy = ReplaceVars(rc, ReplaceBy);
            try
            {
                rc.InternalVars[expandedVarName] = Regex.Replace(expandedText, expandedExpression, expandedReplaceBy);
            }
            catch (Exception ex)
            {
                rc.SetError(ReadContext, "Verarbeitungfehler",
                    $"Beim Ausführen des Skriptes ist ein Fehler aufgetreten '{ex.Message}'. Die Ausführung wird abgebrochen."
                    + $"Variablenwerte: expandedVarName='{expandedVarName}' expandedText='{expandedText}' expandedExpression='{expandedExpression}' expandedReplaceBy='{expandedReplaceBy}'");
                return null;
            }
            return NextCommand;
        }
    }
}