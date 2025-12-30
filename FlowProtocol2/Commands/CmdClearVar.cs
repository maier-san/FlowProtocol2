namespace FlowProtocol2.Commands
{
    using System.Text.RegularExpressions;
    using FlowProtocol2.Core;

    /// <summary>
    /// Implementiert den ClearVar-Befehl
    /// </summary>
    public class CmdClearVar : CmdBaseCommand
    {
        public string VarPattern { get; set; }

        public static CommandParser GetComandParser()
        {
            return new CommandParser(@"^~ClearVar ([A-Za-z0-9\$\(\)]*[\*]?)", (rc, m) => CreateClearVarCommand(rc, m));
        }

        private static CmdBaseCommand CreateClearVarCommand(ReadContext rc, Match m)
        {
            CmdClearVar cmd = new CmdClearVar(rc);
            cmd.VarPattern = m.Groups[1].Value.Trim();
            return cmd;
        }

        public CmdClearVar(ReadContext readcontext) : base(readcontext)
        {
            VarPattern = string.Empty;
        }

        public override CmdBaseCommand? Run(RunContext rc)
        {
            string expandedVarPattern = ReplaceVars(rc, VarPattern);
            try
            {
                bool starPattern = expandedVarPattern.EndsWith('*');                
                if (starPattern)
                {
                    // Sternchen am Ende: Lösche alle Variable, die mit dem Muster beginnen
                    string expandedVarPatternNoStar = expandedVarPattern.Replace("*","");
                    List<string> remKeys = rc.InternalVars.Where(c=>c.Key.StartsWith(expandedVarPatternNoStar)).Select(c=>c.Key).ToList();
                    foreach (var k in remKeys)
                    {
                        rc.InternalVars.Remove(k);
                    }
                }
                else
                {
                    // Kein Sternchen am Ende: Lösche genau die angegebene Variable
                    rc.InternalVars.Remove(expandedVarPattern);
                }
            }
            catch (Exception ex)
            {
                rc.SetError(ReadContext, "Verarbeitungsfehler",
                    $"Beim Ausführen des Skriptes ist ein Fehler aufgetreten '{ex.Message}'. Die Ausführung wird abgebrochen."
                    + $"Variablenwerte: expandedVarPattern='{expandedVarPattern}'");
                return null;
            }
            return NextCommand;
        }
    }
}