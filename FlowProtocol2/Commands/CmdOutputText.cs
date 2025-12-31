namespace FlowProtocol2.Commands
{
    using System.Text.RegularExpressions;
    using FlowProtocol2.Core;

    public class CmdOutputText : CmdBaseCommand
    {
        public string Section { get; set; }
        public string LevelKey { get; set; }
        public string BlockFormatKey { get; set; }
        public string Text { get; set; }
        public static CommandParser GetComandParser()
        {
            return new CommandParser(@"^(@[^>]*)?>(>|\.)?([_\*\#\|\s]?)(.*)", (rc, m) => CreateOutputTextCommand(rc, m));
        }

        private static CmdBaseCommand CreateOutputTextCommand(ReadContext rc, Match m)
        {
            CmdOutputText cmd = new CmdOutputText(rc);
            cmd.Section = m.Groups[1].Value.Trim();
            cmd.LevelKey = m.Groups[2].Value.Trim();
            cmd.BlockFormatKey = m.Groups[3].Value.Trim();
            cmd.Text = m.Groups[4].Value;
            return cmd;
        }

        public CmdOutputText(ReadContext readcontext) : base(readcontext)
        {
            Section = string.Empty;
            LevelKey = ">";
            BlockFormatKey = string.Empty;
            Text = string.Empty;
        }

        public override CmdBaseCommand? Run(RunContext rc)
        {
            string expandedSection = ReplaceVars(rc, Section).Trim();
            string expandedText = ReplaceVars(rc, Text);            
            try
            {
                if (expandedSection.StartsWith('@'))
                {
                    expandedSection = expandedSection[1..];
                }
                if (!string.IsNullOrEmpty(expandedSection))
                {
                    rc.DocumentBuilder.CurrentSection = expandedSection;
                }
                Level l = Level.Level1;
                OutputType ot = OutputType.Enumeration;
                switch (LevelKey)
                {
                    case ">": l = Level.Level1; ot = OutputType.Enumeration; break;
                    case "": l = Level.Level2; ot = OutputType.Listing; break;
                    case ".": l = Level.Level3; ot = OutputType.Listing; break;
                }
                switch (BlockFormatKey)
                {
                    case "*": ot = OutputType.Listing; break;
                    case "#": ot = OutputType.Enumeration; break;
                    case "|": ot = OutputType.Code; break;
                    case "_": ot = OutputType.Paragraph; break;
                }                
                if (ot != OutputType.Code) expandedText = expandedText.Trim(); else expandedText = expandedText.TrimEnd();
                rc.DocumentBuilder.AddNewTextLine(l, ot, expandedText);
            }
            catch (Exception ex)
            {
                rc.SetError(ReadContext, "Verarbeitungsfehler",
                    $"Beim Ausführen des Skriptes ist ein Fehler aufgetreten '{ex.Message}'. Die Ausführung wird abgebrochen."
                    + $"Variablenwerte: expandedSection='{expandedSection}' LevelKey='{LevelKey}' BlockFormatKey='{BlockFormatKey}' expandedText='{expandedText}'");
                return null;
            }
            return NextCommand;
        }
    }
}