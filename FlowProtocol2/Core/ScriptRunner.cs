namespace FlowProtocol2.Core
{
    using FlowProtocol2.Commands;
    public class ScriptRunner
    {
        public void RunScript(RunContext rc, CmdBaseCommand? startcommand)
        {
            CmdBaseCommand? cmdNext = startcommand;
            int stepcount = 0;
            ReadContext lastReadContext = new ReadContext("nosourcefile", 0, 0, "nocodeline");
            try
            {
                while (cmdNext != null && !rc.ExecuteNow 
                        && stepcount < rc.CommandStopCounter && rc.ExampleMaxReplaceLengthExceeded == string.Empty)
                {
                    stepcount++;
                    lastReadContext = cmdNext.ReadContext;
                    cmdNext = cmdNext.Run(rc);
                    if (cmdNext == null && !rc.ExecuteNow && rc.ReturnStack.Any())
                    {
                        var ep = rc.ReturnStack.Pop();
                        cmdNext = ep.EntryCommand;
                        rc.BaseKey = ep.BaseKey;
                    }
                }
                if (stepcount >= rc.CommandStopCounter && cmdNext != null)
                {
                    rc.SetError(cmdNext.ReadContext, "Maximale Schrittzahl überschritten",
                        $"Die maximale Schrittzahl von {rc.CommandStopCounter} wurde überschritten. Die Ausführung wurde abgebrochen.");
                }
                if (rc.ExampleMaxReplaceLengthExceeded != string.Empty)
                {
                    rc.SetError(lastReadContext, "Maximale Zeichenkettenlänge überschritten",
                        $"Eine Zeichenkette hat nach Variablenersetzung die maximale Länge von {rc.MaxReplaceLength} Zeichen überschritten. Inhalt: {rc.ExampleMaxReplaceLengthExceeded}... . Die Ausführung wurde abgebrochen.");
                }
            }
            catch (Exception ex)
            {
                rc.SetError(lastReadContext, "Unbehandelte Ausnahme bei der Ausführung",
                    $"Bei der Ausführung trat eine unbehandelte Ausnahme auf: {ex.Message}. Die Ausführung wurde abgebrochen.");
            }
        }
    }
}