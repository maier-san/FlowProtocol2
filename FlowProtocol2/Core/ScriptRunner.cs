namespace FlowProtocol2.Core
{
    using FlowProtocol2.Commands;
    public class ScriptRunner
    {       
        public void RunScript(RunContext rc, CmdBaseCommand? startcommand)
        {            
            CmdBaseCommand? cmdNext = startcommand;
            while (cmdNext != null)
            {
                cmdNext = cmdNext.Run(rc);
            }
        }
    }
}