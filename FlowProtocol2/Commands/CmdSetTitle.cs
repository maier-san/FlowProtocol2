namespace FlowProtocol2.Commands
{
    using System.Text.RegularExpressions;

    /// <summary>
    /// Implementiert den SetTitle-Befehl
    /// </summary>
    public class CmdSetTitle : CmdBaseCommand
    {
        public string Title { get; set; }

        public static CommandParser GetComandParser()
        {
            return new CommandParser(@"^~SetTitle (.*)", (rc, m) => CreateSetTitleCommand(rc, m));
        }

        private static CmdBaseCommand CreateSetTitleCommand(ReadContext rc, Match m)
        {
            CmdSetTitle cmd = new CmdSetTitle(rc);
            cmd.Title = m.Groups[1].Value.Trim();
            return cmd;
        }

        public CmdSetTitle(ReadContext readcontext) : base(readcontext)
        {
            Title = string.Empty;
        }

        public override CmdBaseCommand? Run(RunContext rc)
        {
            rc.DocumentBuilder.SetTitle(ReplaceVars(rc, Title));
            return NextCommand;
        }
    }
}