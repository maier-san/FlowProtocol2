namespace FlowProtocol2.Commands
{
    using FlowProtocol2.Commands;
    using System.Text.RegularExpressions;

    public class CommandParser
    {
        public Regex LineExpression;
        public Func<ReadingSession, CmdBaseCommand> CommandCreator;
        public CommandParser(string lineexpressionstring, Func<ReadingSession, CmdBaseCommand> commandcreator)
        {
            LineExpression = new Regex(lineexpressionstring);
            CommandCreator = commandcreator;
        }
    }
}