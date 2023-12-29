using System.Text.RegularExpressions;
using FlowProtocol2.Core;

namespace FlowProtocol2.Commands
{
    public class CmdInputText : CmdInputBaseCommand
    {
        public string Key { get; set; }
        public string Promt { get; set; }

        public static CommandParser GetComandParser()
        {
            return new CommandParser(@"^~Input ([A-Za-z0-9\$\(\)]*):(.*)", (rc, m) => CreateInputTextCommand(rc, m));
        }

        public static CmdInputText CreateInputTextCommand(ReadContext rc, Match m)
        {
            CmdInputText cmd = new CmdInputText(rc);
            cmd.Key = m.Groups[1].Value.Trim();
            cmd.Promt = m.Groups[2].Value.Trim();
            return cmd;
        }

        public CmdInputText(ReadContext readcontext) : base(readcontext)
        {
            Key = string.Empty;
            Promt = string.Empty;
        }

        public override CmdBaseCommand? Run(RunContext rc)
        {
            var inputtext = new IMTextInputElement();
            string expandedKey = ReplaceVars(rc, Key).Trim();
            string plainKey = expandedKey;
            if (!string.IsNullOrEmpty(rc.BaseKey))
            {
                expandedKey = rc.BaseKey + "_" + expandedKey;
            }
            inputtext.Key = expandedKey;
            inputtext.Promt = ReplaceVars(rc, Promt).Trim();
            if (rc.BoundVars.ContainsKey(expandedKey) && !string.IsNullOrEmpty(rc.BoundVars[expandedKey]))
            {
                rc.GivenKeys.Add(expandedKey);
            }
            else
            {
                rc.BoundVars[expandedKey] = string.Empty;
                rc.InputForm.AddInputItem(inputtext);
                AssociatedInputElement = inputtext;
            }
            if (rc.BoundVars.ContainsKey(expandedKey))
            {
                rc.InternalVars[plainKey] = rc.BoundVars[expandedKey];
            }
            return NextCommand;
        }
    }
}