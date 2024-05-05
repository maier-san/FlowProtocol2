namespace FlowProtocol2.Commands
{
    using System.Text.RegularExpressions;
    using FlowProtocol2.Core;

    /// <summary>
    /// Implementiert den CalculateExpression-Befehl
    /// </summary>
    public class CmdCalculateExpression : CmdBaseCommand
    {
        public string VarName { get; set; }
        public string Expression { get; set; }

        public static CommandParser GetComandParser()
        {
            return new CommandParser(@"^~CalculateExpression ([A-Za-z0-9\$\(\)]*)\s*=(.*)", (rc, m) => CreateCalculateExpressionCommand(rc, m));
        }

        private static CmdBaseCommand CreateCalculateExpressionCommand(ReadContext rc, Match m)
        {
            CmdCalculateExpression cmd = new CmdCalculateExpression(rc);
            cmd.VarName = m.Groups[1].Value.Trim();
            cmd.Expression = m.Groups[2].Value.Trim();
            return cmd;
        }

        public CmdCalculateExpression(ReadContext readcontext) : base(readcontext)
        {
            VarName = string.Empty;
            Expression = string.Empty;
        }

        public override CmdBaseCommand? Run(RunContext rc)
        {
            string expandedVarName = ReplaceVars(rc, VarName);
            string expandedExpression = ReplaceVars(rc, Expression).Replace(" ", string.Empty);
            try
            {
                string dezTrenn = (1.1).ToString().Replace("1", string.Empty);
                string RZahl = @"[\-]?[0-9]+\" + dezTrenn + @"?[0-9]*";
                Regex MExpr = new Regex($"({RZahl})([\\*\\/])({RZahl})");
                Regex AExpr = new Regex($"[^\\*\\/]({RZahl})([\\+\\-])({RZahl})[^\\*\\/]");
                Regex KExpr = new Regex($"\\(({RZahl})\\)");
                Match? fm = null;
                do
                {
                    fm = null;
                    string erg = string.Empty;
                    if (KExpr.IsMatch(expandedExpression))
                    {
                        fm = KExpr.Match(expandedExpression);
                        erg = fm.Value.Replace("(", string.Empty).Replace(")", string.Empty);
                    }
                    else if (MExpr.IsMatch(expandedExpression))
                    {
                        fm = MExpr.Match(expandedExpression);
                        (double w1, char wop, double w2) = GetExpressionValues(fm);
                        if (wop == '*')
                        {
                            erg = (w1 * w2).ToString(); ;
                        }
                        else if (wop == '/')
                        {
                            if (w2 != 0)
                            {
                                erg = (w1 / w2).ToString();
                            }
                            else
                            {
                                rc.SetError(ReadContext, "Division durch null",
                                    $"Beim Durchführen einer Berechnung kam es zu einer Division durch null. Die Ausführung wird abgebrochen.");
                                return null;
                            }
                        }
                    }
                    else if (AExpr.IsMatch(expandedExpression))
                    {
                        fm = AExpr.Match(expandedExpression);
                        (double w1, char wop, double w2) = GetExpressionValues(fm);
                        if (wop == '+')
                        {
                            erg = (w1 + w2).ToString();
                        }
                        else if (wop == '-')
                        {
                            erg = (w1 - w2).ToString();
                        }
                    }
                    if (fm != null && !string.IsNullOrEmpty(erg))
                    {
                        expandedExpression = $"{expandedExpression.Substring(0, fm.Index)}{erg}{expandedExpression.Substring(fm.Index + fm.Length)}";
                    }
                } while (fm != null);
                rc.InternalVars[expandedVarName] = expandedExpression;
            }
            catch (Exception ex)
            {
                rc.SetError(ReadContext, "Verarbeitungfehler",
                    $"Beim Ausführen des Skriptes ist ein Fehler aufgetreten '{ex.Message}'. Die Ausführung wird abgebrochen."
                    + $"Variablenwerte: expandedVarName='{expandedVarName}' expandedExpression='{expandedExpression}'");
                return null;
            }
            return NextCommand;
        }

        (double, char, double) GetExpressionValues(Match fm)
        {
            string v1 = fm.Groups[1].Value.Trim();
            string v2 = fm.Groups[3].Value.Trim();
            string op = fm.Groups[2].Value.Trim() + "?";
            bool v1OK = Double.TryParse(v1, out double w1);
            bool v2OK = Double.TryParse(v2, out double w2);
            if (!v1OK || !v2OK) op = "?";
            char wop = op.ToCharArray()[0];
            return (w1, wop, w2);
        }
    }
}