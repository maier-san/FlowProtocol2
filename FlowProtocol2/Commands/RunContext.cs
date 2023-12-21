namespace FlowProtocol2.Commands
{
    using FlowProtocol2.Commands;
    using FlowProtocol2.Core;
    using System.Text.RegularExpressions;
    public class RunContext
    {
        public Dictionary<string, string> BoundVars;
        public List<string> GivenKeys { get; set; }
        public Dictionary<string, string> InternalVars { get; set; }
        public List<InputBaseElement> InputItems { get; set; }
        public List<ErrorElement> ErrorItems { get; set; }
        public DocumentBuilder DocumentBuilder { get; set; }
        public RunContext()
        {
            BoundVars = new Dictionary<string, string>();
            GivenKeys = new List<string>();
            InternalVars = new Dictionary<string, string>();
            InputItems = new List<InputBaseElement>();
            ErrorItems = new List<ErrorElement>();
            DocumentBuilder = new DocumentBuilder();
        }
    }
}