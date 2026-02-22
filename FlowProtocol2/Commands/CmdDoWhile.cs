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
            return new CommandParser(@"^~DoWhile\s+(.*)", (rc, m) => CreateDoWhileCommand(rc, m));
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
            try
            {                
                Evaluation = EvaluateExpression(rc, Expression, out ErrorElement? err);
                if (err != null)
                {
                    rc.ErrorItems.Add(err);
                    return null;
                }
                LinkAssociatedLoopCommand(rc, "DoWhile");
                if (AssociatedLoopCommand == null)
                {
                    rc.SetError(ReadContext, "DoWhile ohne Loop",
                        "Dem DoWhile-Befehl kann kein Loop-Befehl auf gleicher Ebene zugeordnet werden. Die Bearbeitung wird abgebrochen.");
                    return null;
                }
                if (!IsInitialized.ContainsKey(rc.BaseKey) || !IsInitialized[rc.BaseKey])
                {
                    IsInitialized[rc.BaseKey] = true;        
                    AssociatedLoopCommand.LoopCounter = 0;                    
                }                           
                if (Evaluation)
                {              
                    return NextCommand;
                }
                else
                {
                    IsInitialized[rc.BaseKey] = false;
                    return AssociatedLoopCommand.NextCommand;
                }                                                
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