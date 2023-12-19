namespace FlowProtocol2.Core
{
    using FlowProtocol2.Commands;
    public class ScriptRunner
    {       
        public void RunScript(CmdBaseCommand? startcommand, Dictionary<string, string> boundvars)
        {
            RunContext rc = new RunContext(boundvars);
            CmdBaseCommand? cmdNext = startcommand;
            while (cmdNext != null)
            {
                cmdNext = cmdNext.Run(ref rc);
            }
        }
    }
}