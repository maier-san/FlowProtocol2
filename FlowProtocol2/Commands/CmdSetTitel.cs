namespace FlowProtocol2.Commands
{
    using System.Text.RegularExpressions;
    using FlowProtocol2.Core;

    /// <summary>
    /// Implementiert den SetTitel-Befehl
    /// </summary>
    public class CmdSetTitel : CmdBaseCommand
    {
        public string Titel { get; set; }

        public static CommandParser GetComandParser()
        {
            return new CommandParser(@"^~SetTitel (.*)", (rc, m) => CreateSetTitelCommand(rc, m));
        }

        private static CmdBaseCommand CreateSetTitelCommand(ReadContext rc, Match m)
        {
            CmdSetTitel cmd = new CmdSetTitel(rc);
            cmd.Titel = m.Groups[1].Value.Trim();
            return cmd;
        }

        public CmdSetTitel(ReadContext readcontext) : base(readcontext)
        {
            Titel = string.Empty;
        }

        public override CmdBaseCommand? Run(RunContext rc)
        {
            rc.DocumentBuilder.SetTitel(ReplaceVars(rc, Titel));
            return NextCommand;
        }
    }
}