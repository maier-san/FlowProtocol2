namespace FlowProtocol2.Commands
{
    using System.Globalization;
    using System.Text.RegularExpressions;
    using FlowProtocol2.Core;

    /// <summary>
    /// Implementiert den SetCulture-Befehl
    /// </summary>
    public class CmdSetCulture : CmdBaseCommand
    {
        public string Culture { get; set; }

        public static CommandParser GetComandParser()
        {
            return new CommandParser(@"^~SetCulture ([A-Za-z0-9\$\(\)]*)\s*", (rc, m) => CreateSetCultureCommand(rc, m));
        }

        private static CmdBaseCommand CreateSetCultureCommand(ReadContext rc, Match m)
        {
            CmdSetCulture cmd = new CmdSetCulture(rc);
            cmd.Culture = m.Groups[1].Value.Trim();
            return cmd;
        }

        public CmdSetCulture(ReadContext readcontext) : base(readcontext)
        {
            Culture = string.Empty;
        }

        public override CmdBaseCommand? Run(RunContext rc)
        {
            string expandedCulture = ReplaceVars(rc, Culture);
            try
            {
                rc.Culture = new CultureInfo(Culture, false);
            }
            catch (Exception ex)
            {
                rc.SetError(ReadContext, "Verarbeitungfehler",
                    $"Beim Ausführen des Skriptes ist ein Fehler aufgetreten '{ex.Message}'. Die Ausführung wird abgebrochen."
                    + $"Variablenwerte: expandedCulture='{expandedCulture}'");
                return null;
            }
            return NextCommand;
        }
    }
}