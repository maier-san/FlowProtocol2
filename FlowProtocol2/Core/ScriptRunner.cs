namespace FlowProtocol2.Core
{
    using FlowProtocol2.Commands;
    public class ScriptRunner
    {
        public void RunScript(RunContext rc, CmdBaseCommand? startcommand)
        {
            const int _maxStepCount = 20000;
            CmdBaseCommand? cmdNext = startcommand;
            int stepcount = 0;
            while (cmdNext != null && stepcount < _maxStepCount)
            {
                stepcount++;
                cmdNext = cmdNext.Run(rc);
                if (cmdNext == null && rc.ReturnStack.Any())
                {
                    cmdNext = rc.ReturnStack.Pop();
                }
            }
            if (cmdNext != null && stepcount >= _maxStepCount)
            {
                rc.SetError(cmdNext.ReadContext, "Maximale Schrittzahl überschritten",
                    $"Die maximale Schrittzahl von {_maxStepCount} wurde überschritten. Die Ausführung wurde abgebrochen.");
            }
        }
    }
}