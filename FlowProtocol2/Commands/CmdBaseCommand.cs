namespace FlowProtocol2.Commands
{    
    public abstract class CmdBaseCommand
    {
        public CmdBaseCommand? NextCommand {get; set;}
        public CmdBaseCommand? PreviousCommand {get; set;}
        protected ReadContext ReadContext {get; set;}        
        protected CmdBaseCommand(ReadContext readcontext)
        {
            ReadContext = readcontext;
        }
        public abstract CmdBaseCommand? Run(ref RunContext rc); 
        public void SetNextCommand(CmdBaseCommand nextcommand)
        {
            NextCommand = nextcommand;
            nextcommand.PreviousCommand = this;
        }
    }
}