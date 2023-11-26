namespace FlowProtocol2.Commands
{
    using FlowProtocol2.Commands;
    using System.Text.RegularExpressions;
    public class ReadingSession
    {
        public string ScriptFilePath { get; set; }
        public int Indent {get; set;}
        public Match ExpressionMatch { get; set; }
        public CmdBaseCommand? PreviousCommand {get; set;}

        public ReadingSession(string scriptfilepath, int indent, Match expressionmatch, CmdBaseCommand? previouscommand)
        {
            ScriptFilePath = scriptfilepath;
            Indent = indent;
            ExpressionMatch = expressionmatch;
            PreviousCommand = previouscommand;
        }
    }
}