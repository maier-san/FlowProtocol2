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
        public string BaseKey { get; set; }
        public string Ignore { get; set; }

        public static CommandParser GetComandParser()
        {
            return new CommandParser(@"^~GoSub ([A-Za-z0-9\$\(\)]*)\s*(; BaseKey=[A-Za-z0-9\$\(\)]*)?(.*)", (rc, m) => CreateGoSubCommand(rc, m));
        }

        private static CmdBaseCommand CreateGoSubCommand(ReadContext rc, Match m)
        {
            CmdGoSub cmd = new CmdGoSub(rc);
            cmd.SubName = m.Groups[1].Value.Trim();
            cmd.BaseKey = m.Groups[2].Value.Trim();
            cmd.Ignore = m.Groups[3].Value.Trim();
            return cmd;
        }

        public CmdGoSub(ReadContext readcontext) : base(readcontext)
        {
            SubName = string.Empty;
            BaseKey = string.Empty;
            Ignore = string.Empty;
        }

        public override CmdBaseCommand? Run(RunContext rc)
        {
            string expandedSubName = ReplaceVars(rc, SubName);            
            CmdDefineSub? sub = GetFirstCommand<CmdDefineSub>(rc, c => c.Name == expandedSubName, c => false);
            if (sub == null)
            {
                rc.SetError(ReadContext, "Sprungziel nicht gefunden",
                    $"Das Sprungziel '{expandedSubName}' konnte nicht gefunden werden. Die Skriptausführung wird abgebrochen.");
                return null;
            }
            string expandedBaseKey = ReplaceVars(rc, BaseKey.Replace("; BaseKey=", string.Empty)).Trim();
            if (NextCommand != null) rc.ReturnStack.Push(new EntryPoint(NextCommand, rc.BaseKey));
            rc.BaseKey = expandedBaseKey;
            return sub.NextCommand;
        }
    }
}