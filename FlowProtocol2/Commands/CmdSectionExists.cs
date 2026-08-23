namespace FlowProtocol2.Commands
{
    using System.Globalization;
    using System.Text.RegularExpressions;
    using FlowProtocol2.Core;

    /// <summary>
    /// Implementiert den SectionExists-Befehl
    /// </summary>
    /// <remarks>
    /// Erstellt mit NewCC.fp2, Eingabe: ~SectionExists (vVarName) = (xSection)
    /// </remarks>
    public class CmdSectionExists : CmdBaseCommand
    {
        public string VarName { get; set; }
        public string Section { get; set; }

        public static CommandParser GetComandParser()
        {
            return new CommandParser(@"^~SectionExists\s+([A-Za-z0-9\$\(\)]+)\s*=\s*(.+)",
                                     (rc, m) => CreateSectionExistsCommand(rc, m));
        }

        private static CmdBaseCommand CreateSectionExistsCommand(ReadContext rc, Match m)
        {
            CmdSectionExists cmd = new CmdSectionExists(rc);
            cmd.VarName = m.Groups[1].Value.Trim();
            cmd.Section = m.Groups[2].Value.Trim();
            return cmd;
        }

        public CmdSectionExists(ReadContext readcontext) : base(readcontext)
        {
            VarName = string.Empty;
            Section = string.Empty;
        }

        public override CmdBaseCommand? Run(RunContext rc)
        {
            string expandedVarName = ReplaceVars(rc, VarName);
            string expandedSection = ReplaceVars(rc, Section);
            try
            {
                bool sectionexists = rc.DocumentBuilder.Document.Sections
                    .Exists(section => section.Headline == expandedSection);
                rc.InternalVars[expandedVarName] = BoolString(sectionexists);
            }
            catch (Exception ex)
            {
                rc.SetError(ReadContext, "Verarbeitungsfehler",
                    $"Beim Ausführen des Skriptes ist ein Fehler aufgetreten '{ex.Message}'. Die Ausführung wird abgebrochen."
                    + $"Variablenwerte: expandedVarName='{expandedVarName}' expandedSection='{expandedSection}'");
                return null;
            }
            return NextCommand;
        }
    }
}