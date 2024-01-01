namespace FlowProtocol2.Commands
{
    using System.Text.RegularExpressions;
    using FlowProtocol2.Core;

    /// <summary>
    /// Implementiert den DefineSub-Befehl
    /// </summary>
    public class CmdDefineSub : CmdBaseCommand
    {
        public string Name { get; set; }

        public static CommandParser GetComandParser()
        {
            return new CommandParser(@"^~DefineSub ([A-Za-z0-9]*)", (rc, m) => CreateDefineSubCommand(rc, m));
        }

        private static CmdBaseCommand CreateDefineSubCommand(ReadContext rc, Match m)
        {
            CmdDefineSub cmd = new CmdDefineSub(rc);
            cmd.Name = m.Groups[1].Value.Trim();
            return cmd;
        }

        public CmdDefineSub(ReadContext readcontext) : base(readcontext)
        {
            Name = string.Empty;
        }

        public override CmdBaseCommand? Run(RunContext rc)
        {
            return NextCommand;
        }
    }
}