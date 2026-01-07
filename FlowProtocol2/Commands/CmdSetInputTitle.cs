namespace FlowProtocol2.Commands
{
    using System.Text.RegularExpressions;
    using FlowProtocol2.Core;

    /// <summary>
    /// Implementiert den SetInputTitle-Befehl
    /// </summary>
    public class CmdSetInputTitle : CmdBaseCommand
    {
        public string Title { get; set; }

        public static CommandParser GetComandParser()
        {
            return new CommandParser(@"^~SetInputTitle (.*)", (rc, m) => CreateSetInputTitleCommand(rc, m));
        }

        private static CmdBaseCommand CreateSetInputTitleCommand(ReadContext rc, Match m)
        {
            CmdSetInputTitle cmd = new CmdSetInputTitle(rc);
            cmd.Title = m.Groups[1].Value.Trim();
            return cmd;
        }

        public CmdSetInputTitle(ReadContext readcontext) : base(readcontext)
        {
            Title = string.Empty;
        }

        public override CmdBaseCommand? Run(RunContext rc)
        {
            string expandedTitle = ReplaceVars(rc, Title).Trim();
            rc.InputForm.Title = expandedTitle;
            return NextCommand;
        }
    }
}