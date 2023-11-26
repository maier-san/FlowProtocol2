namespace FlowProtocol2.Commands
{    

    public abstract class CmdBaseCommand
    {
        public CmdBaseCommand? NextCommand {get; set;}

        public abstract void Run(ref RunningSession rsession);    
    }
}