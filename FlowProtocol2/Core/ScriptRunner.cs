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
            while (cmdNext != null && !rc.ExecuteNow && stepcount < _maxStepCount)
            {
                stepcount++;
                cmdNext = cmdNext.Run(rc);
                if (cmdNext == null && !rc.ExecuteNow && rc.ReturnStack.Any())
                {
                    cmdNext = rc.ReturnStack.Pop();
                }
            }
            if (stepcount >= _maxStepCount && cmdNext != null)
            {
                rc.SetError(cmdNext.ReadContext, "Maximale Schrittzahl überschritten",
                    $"Die maximale Schrittzahl von {_maxStepCount} wurde überschritten. Die Ausführung wurde abgebrochen.");
            }
        }
    }
}