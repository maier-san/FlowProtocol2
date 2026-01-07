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
            // Leerzeichen innen entfernen und außen dazu, damit alle regulären Ausdrücke richtig arbeiten können.
            string expandedExpression = " " + ReplaceVars(rc, Expression).Replace(" ", string.Empty) + " ";
            string lastexpandedExpression = string.Empty;
            try
            {
                // Dezimaltrennzeichen ermitteln
                string dezTrenn = (1.1).ToString().Replace("1", string.Empty);
                // Ausdruck für eine Zahl einschließlich der Möglichkeit der Exponentialdarstellung
                string RZahl = @"[\-]?[0-9]+\" + dezTrenn + @"?[0-9]*(?:E\-?[0-9][0-9])?";
                // Operator-Ausdruck
                Regex OExpr = new Regex($"(sqrt|sin|cos|tan|exp|ln)\\(({RZahl})\\)");
                // Zahl in Klammern
                Regex KExpr = new Regex($"\\(({RZahl})\\)");
                // Potenzierung
                Regex EExpr = new Regex($"({RZahl})(\\^)({RZahl})");
                // Punkt-Ausdruck mit "*", "/" oder "%" ohne angrenzende Potenzierung
                Regex MExpr = new Regex($"[^\\^0-9]({RZahl})([\\*\\/%])({RZahl})[^\\^0-9]");
                // Strich-Ausdruck mit "+" oder "-" ohne angrenzende Potenzierung oder Punkt-Rechnung
                Regex AExpr = new Regex($"[^\\^\\*\\/%0-9]({RZahl})([\\+\\-])({RZahl})[^\\^\\*\\/%0-9]");
                Match? fm = null;
                do
                {
                    fm = null;
                    int repindex = 0;
                    int replength = 0;
                    string erg = string.Empty;
                    if (OExpr.IsMatch(expandedExpression))
                    {
                        fm = OExpr.Match(expandedExpression);
                        repindex = fm.Groups[0].Index;
                        replength = fm.Groups[0].Length;
                        (string fname, double w1) = GetFunctionValues(fm);
                        switch (fname)
                        {
                            case "sqrt": erg = (Math.Sqrt(w1)).ToString(); break;
                            case "sin": erg = (Math.Sin(w1)).ToString(); break;
                            case "cos": erg = (Math.Cos(w1)).ToString(); break;
                            case "tan": erg = (Math.Tan(w1)).ToString(); break;
                            case "exp": erg = (Math.Exp(w1)).ToString(); break;
                            case "ln": erg = (Math.Log(w1)).ToString(); break;
                        }
                    }
                    else if (KExpr.IsMatch(expandedExpression))
                    {
                        fm = KExpr.Match(expandedExpression);
                        repindex = fm.Groups[0].Index;
                        replength = fm.Groups[0].Length;
                        erg = fm.Value.Replace("(", string.Empty).Replace(")", string.Empty);
                    }
                    else if (EExpr.IsMatch(expandedExpression))
                    {
                        fm = EExpr.Match(expandedExpression);
                        repindex = fm.Groups[1].Index;
                        replength = fm.Groups[1].Length + fm.Groups[2].Length + fm.Groups[3].Length;
                        (double w1, char wop, double w2) = GetExpressionValues(fm);
                        if (wop == '^')
                        {
                            erg = Math.Pow(w1, w2).ToString();
                        }
                    }
                    else if (MExpr.IsMatch(expandedExpression))
                    {
                        fm = MExpr.Match(expandedExpression);
                        repindex = fm.Groups[1].Index;
                        replength = fm.Groups[1].Length + fm.Groups[2].Length + fm.Groups[3].Length;
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
                                    $"Beim Durchführen einer Berechnung kam es zu einer Division durch null im Ausdruck {expandedExpression}. Die Ausführung wird abgebrochen.");
                                return null;
                            }
                        }
                        else if (wop == '%')
                        {
                            if (w2 != 0)
                            {
                                erg = (w1 % w2).ToString();
                            }
                            else
                            {
                                rc.SetError(ReadContext, "Modulo-Rechnung durch null",
                                    $"Beim Durchführen einer Berechnung kam es zu einer Modulo-Rechnung durch null im Ausdruck {expandedExpression}. Die Ausführung wird abgebrochen.");
                                return null;
                            }
                        }
                    }
                    else if (AExpr.IsMatch(expandedExpression))
                    {
                        fm = AExpr.Match(expandedExpression);
                        repindex = fm.Groups[1].Index;
                        replength = fm.Groups[1].Length + fm.Groups[2].Length + fm.Groups[3].Length;
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
                    lastexpandedExpression = expandedExpression;
                    if (repindex >= 0 && replength > 0 && !string.IsNullOrEmpty(erg))
                    {
                        expandedExpression = $"{expandedExpression[..repindex]}{erg}{expandedExpression[(repindex + replength)..]}";
                    }
                } while (lastexpandedExpression != expandedExpression);
                rc.InternalVars[expandedVarName] = expandedExpression.Trim();
            }
            catch (Exception ex)
            {
                rc.SetError(ReadContext, "Verarbeitungsfehler",
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
            bool v1OK = double.TryParse(v1, out double w1);
            bool v2OK = double.TryParse(v2, out double w2);
            if (!v1OK || !v2OK) op = "?";
            char wop = op.ToCharArray()[0];
            return (w1, wop, w2);
        }

        (string, double) GetFunctionValues(Match fm)
        {
            string fname = fm.Groups[1].Value.Trim();
            string v1 = fm.Groups[2].Value.Trim();
            bool v1OK = Double.TryParse(v1, out double w1);
            if (!v1OK) fname = "?";
            return (fname, w1);
        }
    }
}