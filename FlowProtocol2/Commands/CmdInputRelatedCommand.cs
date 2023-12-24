using FlowProtocol2.Commands;

public abstract class CmdInputRelatedCommand : CmdBaseCommand
{
    public CmdInputBaseCommand? ParentInputCommand { get; set; }

    public CmdInputRelatedCommand(ReadContext readcontext) : base(readcontext)
    {

    }
}