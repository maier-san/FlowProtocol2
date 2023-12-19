namespace FlowProtocol2.Commands
{
    using FlowProtocol2.Commands;
    using FlowProtocol2.Core;
    using System.Text.RegularExpressions;
    public class RunContext
    {
        public Dictionary<string, string> BoundVars;
        public List<InputBaseElement> InputItems = new List<InputBaseElement>();
        //Todo ListOfError
        //TodO ListOFOutPuts
        public RunContext(Dictionary<string, string> boundvars)
        {
            BoundVars = boundvars;
        }
    }
}