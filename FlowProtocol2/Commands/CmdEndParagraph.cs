namespace FlowProtocol2.Commands
{
    using System.Text.RegularExpressions;
    using FlowProtocol2.Core;

    /// <summary>
    /// Implementiert den EndParagraph-Befehl
    /// </summary>
    public class CmdEndParagraph : CmdBaseCommand
    {
        public static CommandParser GetComandParser()
        {
            return new CommandParser(@"^~EndParagraph", (rc, m) => CreateEndParagraphCommand(rc, m));
        }

        private static CmdBaseCommand CreateEndParagraphCommand(ReadContext rc, Match m)
        {
            CmdEndParagraph cmd = new CmdEndParagraph(rc);
            return cmd;
        }

        public CmdEndParagraph(ReadContext readcontext) : base(readcontext)
        {
        }

        public override CmdBaseCommand? Run(RunContext rc)
        {
            rc.DocumentBuilder.EndParagraph();
            return NextCommand;
        }
    }
}