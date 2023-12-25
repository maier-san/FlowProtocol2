using FlowProtocol2.Core;

namespace FlowProtocol2.Commands
{
    public abstract class CmdInputBaseCommand : CmdBaseCommand
    {
        public IMBaseElement? AssociatedInputElement { get; set; }

        public CmdInputBaseCommand(ReadContext readcontext) : base(readcontext)
        {

        }
    }
}