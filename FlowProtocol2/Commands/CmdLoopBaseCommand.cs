namespace FlowProtocol2.Commands
{
    using System.Text.RegularExpressions;
    using FlowProtocol2.Core;

    /// <summary>
    /// Basisklasse für Schleifen-Befehle
    /// </summary>
    public abstract class CmdLoopBaseCommand : CmdBaseCommand
    {
        public CmdLoop? AssociatedLoopCommand { get; set; }
        public bool IsInitialized { get; set; }
                
        public CmdLoopBaseCommand(ReadContext readcontext) : base(readcontext)
        {
            AssociatedLoopCommand = null;
            IsInitialized = false;
        }

        protected void LinkAssociatedLoopCommand(RunContext rc, string commandname)
        {
            if (AssociatedLoopCommand == null)
            {
                AssociatedLoopCommand = GetNextCommand<CmdLoop>(
                    c => c.Indent == this.Indent,
                    c => c.Indent < this.Indent);
            }
            if (AssociatedLoopCommand == null)
            {
                rc.SetError(ReadContext, $"{commandname} ohne Loop",
                    $"Dem {commandname}-Befehl kann kein Loop-Befehl auf gleicher Ebene zugeordnet werden.");
                return;
            }
            AssociatedLoopCommand.ParentStartLoopCommand = this;
        }        
    }
}