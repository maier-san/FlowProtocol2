namespace FlowProtocol2.Commands
{
    using FlowProtocol2.Commands;
    using FlowProtocol2.Core;
    using System.Text.RegularExpressions;
    public class RunContext
    {
        public Dictionary<string, string> BoundVars;
        public List<string> GivenKeys = new List<string>();
        public Dictionary<string, string> InternalVars = new Dictionary<string, string>();
        public List<ErrorElement> ErrorItem = new List<ErrorElement>();
        public List<InputBaseElement> InputItems = new List<InputBaseElement>();
        public List<OutputElement> OutputItems = new List<OutputElement>();
        public RunContext()
        {
            BoundVars = new Dictionary<string, string>();
        }
    }
}