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
        public abstract CmdBaseCommand? Run(RunContext rc);
        public void SetNextCommand(CmdBaseCommand nextcommand)
        {
            NextCommand = nextcommand;
            nextcommand.PreviousCommand = this;
        }
        protected static string ReplaceVars(RunContext rc, string input)
        {
            if (input.Contains('$'))
            {
                foreach (var v in rc.InternalVars.OrderByDescending(x => x.Key))
                {
                    input = input.Replace("$" + v.Key, v.Value);
                }
                foreach (var v in rc.BoundVars.OrderByDescending(x => x.Key))
                {
                    input = input.Replace("$" + v.Key, v.Value);
                }
                // Systemvariablen
                input = input.Replace("$NewGuid", Guid.NewGuid().ToString());
                input = input.Replace("$CRLF", "\r\n");
                input = input.Replace("$LF", "\n");
                if (input.Contains("$Chr"))
                {
                    for (int i = 1; i < 255; i++)
                    {
                        input = input.Replace($"$Chr{i:000}", Convert.ToChar(i).ToString());
                    }
                }
            }
            return input;
        }
    }
}