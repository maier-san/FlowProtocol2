namespace FlowProtocol2.Commands
{
    using System.Text.RegularExpressions;
    using FlowProtocol2.Core;

    /// <summary>
    /// Implementiert den SetInputDescription-Befehl
    /// </summary>
    public class CmdSetInputDescription : CmdBaseCommand
    {
        public string Description { get; set; }

        public static CommandParser GetComandParser()
        {
            return new CommandParser(@"^~SetInputDescription (.*)", (rc, m) => CreateSetInputDescriptionCommand(rc, m));
        }

        private static CmdBaseCommand CreateSetInputDescriptionCommand(ReadContext rc, Match m)
        {
            CmdSetInputDescription cmd = new CmdSetInputDescription(rc);
            cmd.Description = m.Groups[1].Value.Trim();
            return cmd;
        }

        public CmdSetInputDescription(ReadContext readcontext) : base(readcontext)
        {
            Description = string.Empty;
        }

        public override CmdBaseCommand? Run(RunContext rc)
        {
            string expandedDescription = ReplaceVars(rc, Description);
            rc.InputForm.Description = expandedDescription;
            return NextCommand;
        }
    }
}