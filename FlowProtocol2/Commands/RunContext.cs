namespace FlowProtocol2.Commands
{
    using FlowProtocol2.Core;
    public class RunContext
    {
        public Dictionary<string, string> BoundVars;
        public List<string> GivenKeys { get; set; }
        public Dictionary<string, string> InternalVars { get; set; }
        public List<InputBaseElement> InputItems { get; set; }
        public List<ErrorElement> ErrorItems { get; set; }
        public DocumentBuilder DocumentBuilder { get; set; }
        public string MyBaseURL { get; set; }
        public string MyResultURL { get; set; }
        public RunContext()
        {
            BoundVars = new Dictionary<string, string>();
            GivenKeys = new List<string>();
            InternalVars = new Dictionary<string, string>();
            InputItems = new List<InputBaseElement>();
            ErrorItems = new List<ErrorElement>();
            DocumentBuilder = new DocumentBuilder();
            MyBaseURL = string.Empty;
            MyResultURL = string.Empty;
        }
        public void SetError(ReadContext readcontext, string errorcode, string errortext)
        {
            ErrorItems.Add(new ErrorElement(readcontext, errorcode, errortext));
        }
    }
}