using FlowProtocol2.Core;

namespace FlowProtocol2.Commands
{
    public class CmdInputText : CmdBaseCommand
    {
        public string Key { get; set; }
        public string Promt { get; set; }
        
        public static CommandParser GetComandParser()
        {
            return new CommandParser(@"^~Input ([A-Za-z0-9]*[']?):(.*)", rc => CreateInputTextCommand(rc));
        }

        public static CmdInputText CreateInputTextCommand(ReadContext rc)
        {
            CmdInputText cmd = new CmdInputText(rc);
            cmd.Key = rc.ExpressionMatch.Groups[1].Value.Trim();
            cmd.Promt = rc.ExpressionMatch.Groups[2].Value.Trim();
            return cmd;
        }

        public CmdInputText(ReadContext readcontext) : base(readcontext)
        {
            Key = string.Empty;
            Promt = string.Empty;
        }

        public override CmdBaseCommand? Run(RunContext rc)
        {
            var inputtext = new InputTextElement();            
            inputtext.Key = Key;
            inputtext.Promt = ReplaceVars(rc, Promt);            
            if (rc.BoundVars.ContainsKey(Key))
            {
                rc.GivenKeys.Add(Key);
            }
            else
            {
                rc.BoundVars[Key] = string.Empty;
                rc.InputItems.Add(inputtext);
            }            
            return this.NextCommand;
        }
    }
}