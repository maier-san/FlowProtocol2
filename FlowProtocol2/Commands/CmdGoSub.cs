namespace FlowProtocol2.Commands
{
    using System.Text.RegularExpressions;
    using FlowProtocol2.Core;

    /// <summary>
    /// Implementiert den GoSub-Befehl
    /// </summary>
    public class CmdGoSub : CmdBaseCommand
    {
        public string SubName { get; set; }

        public static CommandParser GetComandParser()
        {
            return new CommandParser(@"^~GoSub ([A-Za-z0-9\$\(\)]*)", (rc, m) => CreateGoSubCommand(rc, m));
        }

        private static CmdBaseCommand CreateGoSubCommand(ReadContext rc, Match m)
        {
            CmdGoSub cmd = new CmdGoSub(rc);
            cmd.SubName = m.Groups[1].Value.Trim();
            return cmd;
        }

        public CmdGoSub(ReadContext readcontext) : base(readcontext)
        {
            SubName = string.Empty;
        }

        public override CmdBaseCommand? Run(RunContext rc)
        {
            string expandedSubName = ReplaceVars(rc, SubName);
            CmdDefineSub? sub = GetNextCommand<CmdDefineSub>(c => true, c => false);
            if (sub == null)
            {
                rc.SetError(ReadContext, "Sprungziel nicht gefunden",
                    $"Das Sprungziel '{expandedSubName}' konnte nicht gefunden werden. Die Skriptausführung wird abgebrochen.");
                return null;
            }
            if (NextCommand != null) rc.ReturnStack.Push(new EntryPoint(NextCommand, rc.BaseKey));
            return sub;
        }
    }
}