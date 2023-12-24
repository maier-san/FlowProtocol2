using FlowProtocol2.Core;

namespace FlowProtocol2.Commands
{
    public abstract class CmdInputBaseCommand : CmdBaseCommand
    {
        public IMBaseElement? AssociatedInputElement { get; set; }

        public CmdInputBaseCommand(ReadContext readcontext) : base(readcontext)
        {

        }

        protected void LinkAllRelatedCommands()
        {
            var relcommands = GetNexCommands<CmdInputRelatedCommand>(c => true,
                c => c.Indent < this.Indent || c is CmdInputBaseCommand);
            foreach (var idx in relcommands)
            {
                idx.ParentInputCommand = this;
            }
        }
    }
}