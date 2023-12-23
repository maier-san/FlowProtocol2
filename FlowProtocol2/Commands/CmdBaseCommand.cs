using System.Text.RegularExpressions;
using FlowProtocol2.Core;

namespace FlowProtocol2.Commands
{
    public abstract class CmdBaseCommand
    {
        public CmdBaseCommand? NextCommand { get; set; }
        public CmdBaseCommand? PreviousCommand { get; set; }
        public ReadContext ReadContext { get; set; }
        protected CmdBaseCommand(ReadContext readcontext)
        {
            ReadContext = readcontext;
        }
        public abstract CmdBaseCommand? Run(RunContext rc);
        public void SetNextCommand(CmdBaseCommand nextcommand)
        {
            if (nextcommand != this)
            {
                NextCommand = nextcommand;
                nextcommand.PreviousCommand = this;
            }
        }
        protected static string ReplaceVars(RunContext rc, string input)
        {
            if (input.Contains('$'))
            {
                foreach (var v in rc.InternalVars.OrderByDescending(x => x.Key))
                {
                    input = input.Replace("$" + v.Key, v.Value);
                }
                foreach (var v in rc.BoundVars.OrderByDescending(x => x.Key))
                {
                    input = input.Replace("$" + v.Key, v.Value);
                }
                // Systemvariablen
                input = input.Replace("$NewGuid", Guid.NewGuid().ToString());
                input = input.Replace("$CRLF", "\r\n");
                input = input.Replace("$LF", "\n");
                if (input.Contains("$Chr"))
                {
                    for (int i = 1; i < 255; i++)
                    {
                        input = input.Replace($"$Chr{i:000}", Convert.ToChar(i).ToString());
                    }
                }
            }
            return input;
        }

        protected T? GetNextCommand<T>(Func<T, bool> predicate)
            where T : CmdBaseCommand
        {
            CmdBaseCommand? cmdidx = NextCommand;
            while (cmdidx != null)
            {
                T? cmdT = cmdidx as T;
                if (cmdT != null && predicate(cmdT))
                {
                    return cmdT;
                }
                cmdidx = cmdidx.NextCommand;
            }
            return null;
        }

        protected CmdBaseCommand? GetNextSameOrHigherLevelCommand()
        {
            return GetNextCommand<CmdBaseCommand>(c => c.ReadContext.Indent <= this.ReadContext.Indent);
        }

        protected List<T> GetNexCommands<T>(Func<T, bool> predicate, Func<CmdBaseCommand, bool> stopcrit)
            where T : CmdBaseCommand
        {
            List<T> ret = new List<T>();
            CmdBaseCommand? cmdidx = NextCommand;
            while (cmdidx != null && !stopcrit(cmdidx))
            {
                T? cmdT = cmdidx as T;
                if (cmdT != null && predicate(cmdT))
                {
                    ret.Add(cmdT);
                }
                cmdidx = cmdidx.NextCommand;
            }
            return ret;
        }

        protected bool EvaluateExpression(RunContext rc, string expression, out ErrorElement? err)
        {
            err = null;
            foreach (var disterm in expression.Split("||"))
            {
                bool bdis = EvaluateDisTerm(rc, disterm, out err);
                if (err != null) return false;
                if (bdis) return true;
            }
            return false;
        }

        private bool EvaluateDisTerm(RunContext rc, string expression, out ErrorElement? err)
        {
            err = null;
            foreach (var conterm in expression.Split("&&"))
            {
                bool bcon = EvaluateConTerm(rc, conterm, out err);
                if (err != null) return false;
                if (!bcon) return false;
            }
            return true;
        }

        private bool EvaluateConTerm(RunContext rc, string expression, out ErrorElement? err)
        {
            err = null;
            expression = expression.Trim();
            if (expression == "1" || expression == "true") return true;
            if (expression == "0" || expression == "false") return false;
            bool erg = false;
            if (CheckCompSTerm(rc, expression, "==", (x, y) => x == y, out erg, out err)) return erg;
            if (CheckCompSTerm(rc, expression, "!=", (x, y) => x != y, out erg, out err)) return erg;
            if (CheckCompDTerm(rc, expression, "<>", (x, y) => x != y, out erg, out err)) return erg;
            if (CheckCompDTerm(rc, expression, "<=", (x, y) => x <= y, out erg, out err)) return erg;
            if (CheckCompDTerm(rc, expression, ">=", (x, y) => x >= y, out erg, out err)) return erg;
            if (CheckCompDTerm(rc, expression, "<", (x, y) => x < y, out erg, out err)) return erg;
            if (CheckCompDTerm(rc, expression, ">", (x, y) => x > y, out erg, out err)) return erg;
            if (CheckCompSTerm(rc, expression, "~", (x, y) => x.Contains(y), out erg, out err)) return erg;
            if (CheckCompSTerm(rc, expression, "!~", (x, y) => !x.Contains(y), out erg, out err)) return erg;
            err = new ErrorElement(ReadContext, "Ungültiger Vergleichsterm",
                $"Der Ausdruck {expression} kann nicht als Vergleichsterm interpretiert werden.");
            return false;
        }

        private bool CheckCompSTerm(RunContext rc, string expression, string scop, Func<string, string, bool> lcop,
            out bool erg, out ErrorElement? err)
        {
            err = null;
            Regex regCompTerm = new Regex(@"(.*)" + scop + "(.*)");
            if (regCompTerm.IsMatch(expression))
            {
                var m = regCompTerm.Match(expression);
                string wert1 = ReplaceVars(rc, m.Groups[1].Value.Trim());
                string wert2 = ReplaceVars(rc, m.Groups[2].Value.Trim());
                erg = lcop(wert1, wert2);
                return true;
            }
            erg = false;
            return false;
        }
        private bool CheckCompDTerm(RunContext rc, string expression, string scop, Func<double, double, bool> lcop,
            out bool erg, out ErrorElement? err)
        {
            err = null;
            Regex regCompTerm = new Regex(@"(.*)" + scop + "(.*)");
            if (regCompTerm.IsMatch(expression))
            {
                var m = regCompTerm.Match(expression);
                string wert1 = ReplaceVars(rc, m.Groups[1].Value.Trim());
                string wert2 = ReplaceVars(rc, m.Groups[2].Value.Trim());

                bool w1OK = double.TryParse(wert1, out double dblwert1);
                bool w2OK = double.TryParse(wert2, out double dblwert2);
                if (!w1OK)
                {
                    err = new ErrorElement(ReadContext, "Ungültiger numerischer Ausdruck",
                        $"Der Ausdruck {wert1} kann nicht als Gleitkommazahl interpretiert werden.");
                }
                else if (!w2OK)
                {
                    err = new ErrorElement(ReadContext, "Ungültiger numerischer Ausdruck",
                        $"Der Ausdruck {wert2} kann nicht als Gleitkommazahl interpretiert werden.");
                }
                else
                {
                    erg = lcop(dblwert1, dblwert2);
                    return true;
                }
            }
            erg = false;
            return false;
        }
    }
}