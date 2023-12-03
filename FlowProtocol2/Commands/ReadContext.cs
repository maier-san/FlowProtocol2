namespace FlowProtocol2.Commands
{
    using FlowProtocol2.Commands;
    using System.Text.RegularExpressions;
    public class ReadContext
    {
        public string ScriptFilePath { get; set; }
        public int Indent {get; set;}
        public Match ExpressionMatch { get; set; }

        public ReadContext(string scriptfilepath, int indent, Match expressionmatch)
        {
            ScriptFilePath = scriptfilepath;
            Indent = indent;
            ExpressionMatch = expressionmatch;
        }
    }
}