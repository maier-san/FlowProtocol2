namespace FlowProtocol2.Commands
{
    using System.Text.RegularExpressions;
    public class ReadContext
    {
        public string ScriptFilePath { get; set; }
        public int Indent { get; set; }
        public int LineNumber { get; set; }
        public string CodeLine { get; set; }
        public Match? ExpressionMatch { get; set; }

        public ReadContext(string scriptfilepath, int indent, int linenumber, string codeline)
        {
            ScriptFilePath = scriptfilepath;
            Indent = indent;
            LineNumber = linenumber;
            CodeLine = codeline;
        }
    }
}