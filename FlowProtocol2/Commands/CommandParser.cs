namespace FlowProtocol2.Commands
{
    using System.Text.RegularExpressions;

    public class CommandParser
    {
        public Regex LineExpression;
        public Func<ReadContext, Match, CmdBaseCommand> CommandCreator;
        public CommandParser(string lineexpressionstring, Func<ReadContext, Match, CmdBaseCommand> commandcreator)
        {
            LineExpression = new Regex(lineexpressionstring);
            CommandCreator = commandcreator;
        }
    }
}