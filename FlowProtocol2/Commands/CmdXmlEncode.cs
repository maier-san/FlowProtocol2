namespace FlowProtocol2.Commands
{
    using System.Text;
    using System.Text.RegularExpressions;
    using FlowProtocol2.Core;

    /// <summary>
    /// Implementiert den XmlEncode-Befehl
    /// </summary>
    public class CmdXmlEncode : CmdBaseCommand
    {
        public string VarName { get; set; }
        public string Text { get; set; }

        public static CommandParser GetComandParser()
        {
            return new CommandParser(@"^~XmlEncode ([A-Za-z0-9\$\(\)]*)\s*=(.*)", (rc, m) => CreateXmlEncodeCommand(rc, m));
        }

        private static CmdBaseCommand CreateXmlEncodeCommand(ReadContext rc, Match m)
        {
            CmdXmlEncode cmd = new CmdXmlEncode(rc);
            cmd.VarName = m.Groups[1].Value.Trim();
            cmd.Text = m.Groups[2].Value.Trim();
            return cmd;
        }

        public CmdXmlEncode(ReadContext readcontext) : base(readcontext)
        {
            VarName = string.Empty;
            Text = string.Empty;
        }

        public override CmdBaseCommand? Run(RunContext rc)
        {
            string expandedVarName = ReplaceVars(rc, VarName);
            string expandedText = ReplaceVars(rc, Text);
            try
            {
                if (string.IsNullOrEmpty(expandedText))
                {
                    rc.InternalVars[expandedVarName] = string.Empty;
                }
                else
                {
                    StringBuilder encoded = new StringBuilder(expandedText.Length);
                    foreach (char c in expandedText)
                    {
                        switch (c)
                        {
                            case '&':
                                encoded.Append("&amp;");
                                break;
                            case '<':
                                encoded.Append("&lt;");
                                break;
                            case '>':
                                encoded.Append("&gt;");
                                break;
                            case '\"':
                                encoded.Append("&quot;");
                                break;
                            case '\'':
                                encoded.Append("&apos;");
                                break;
                            default:
                                encoded.Append(c);
                                break;
                        }
                    }
                    rc.InternalVars[expandedVarName] = encoded.ToString();
                }
            }
            catch (Exception ex)
            {
                rc.SetError(ReadContext, "Verarbeitungfehler",
                    $"Beim Ausführen des Skriptes ist ein Fehler aufgetreten '{ex.Message}'. Die Ausführung wird abgebrochen."
                    + $"Variablenwerte: expandedVarName='{expandedVarName}' expandedText='{expandedText}'");
                return null;
            }
            return NextCommand;
        }
    }
}