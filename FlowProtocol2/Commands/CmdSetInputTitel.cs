namespace FlowProtocol2.Commands
{
    using System.Text.RegularExpressions;
    using FlowProtocol2.Core;

    /// <summary>
    /// Implementiert den SetInputTitel-Befehl
    /// </summary>
    public class CmdSetInputTitel : CmdBaseCommand
    {
        public string Titel { get; set; }

        public static CommandParser GetComandParser()
        {
            return new CommandParser(@"^~SetInputTitel (.*)", (rc, m) => CreateSetInputTitelCommand(rc, m));
        }

        private static CmdBaseCommand CreateSetInputTitelCommand(ReadContext rc, Match m)
        {
            CmdSetInputTitel cmd = new CmdSetInputTitel(rc);
            cmd.Titel = m.Groups[1].Value.Trim();
            return cmd;
        }

        public CmdSetInputTitel(ReadContext readcontext) : base(readcontext)
        {
            Titel = string.Empty;
        }

        public override CmdBaseCommand? Run(RunContext rc)
        {
            rc.InputForm.Titel = ReplaceVars(rc, Titel).Trim();
            return NextCommand;
        }
    }
}