using System.Text.RegularExpressions;
using FlowProtocol2.Core;

namespace FlowProtocol2.Commands
{
    public class CmdInputText : CmdBaseCommand
    {
        public string Key { get; set; }
        public string Promt { get; set; }

        public InputTextElement? InputText;
        public static CommandParser GetComandParser()
        {
            CommandParser cp = new CommandParser(@"^~Input ([A-Za-z0-9]*[']?):(.*)", rs => CreateInputCommand(rs));
            return cp;
        }

        public static CmdInputText CreateInputCommand(ReadContext rc)
        {
            CmdInputText cmd = new CmdInputText(rc);            
            cmd.Key = rc.ExpressionMatch.Groups[1].Value.Trim();
            cmd.Promt = rc.ExpressionMatch.Groups[2].Value.Trim();
            return cmd;
        }

        public CmdInputText(ReadContext rs) : base(rs)
        {
            Key = "";
            Promt = "";
        }

        public override CmdBaseCommand? Run(ref RunContext rc)
        {
            InputText = new InputTextElement();
            InputText.Key = Key;
            InputText.Promt = Promt;
            rc.InputItems.Add(InputText);
            return this.NextCommand;
        }
    }
}