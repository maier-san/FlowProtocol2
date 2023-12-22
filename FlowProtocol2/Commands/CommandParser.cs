namespace FlowProtocol2.Commands
{
    using System.Text.RegularExpressions;

    public class CommandParser
    {
        public Regex LineExpression;
        public Func<ReadContext, CmdBaseCommand> CommandCreator;
        public CommandParser(string lineexpressionstring, Func<ReadContext, CmdBaseCommand> commandcreator)
        {
            LineExpression = new Regex(lineexpressionstring);
            CommandCreator = commandcreator;
        }
    }
}