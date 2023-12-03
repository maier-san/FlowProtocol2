using System.Text.RegularExpressions;

namespace FlowProtocol2.Commands
{
    public class CmdInputText : CmdBaseCommand
    {
        public string Key { get; set; }
        public string QuestionText { get; set; }
        public static CommandParser GetComandParser()
        {
            CommandParser cp = new CommandParser(@"^~Input ([A-Za-z0-9]*[']?):(.*)", rs => CreateInputCommand(rs));
            return cp;
        }

        public static CmdInputText CreateInputCommand(ReadContext rs)
        {
            CmdInputText cmd = new CmdInputText(rs);            
            cmd.Key = rs.ExpressionMatch.Groups[1].Value.Trim();
            cmd.QuestionText = rs.ExpressionMatch.Groups[2].Value.Trim();
            return cmd;
        }

        public CmdInputText(ReadContext rs) : base(rs)
        {
            Key = "";
            QuestionText = "";
        }

        public override void Run(ref RunContext rsession)
        {

        }
    }
}