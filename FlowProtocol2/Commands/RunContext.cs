namespace FlowProtocol2.Commands
{
    using FlowProtocol2.Core;
    public class RunContext
    {
        public Dictionary<string, string> BoundVars;
        public List<string> GivenKeys { get; set; }
        public Dictionary<string, string> InternalVars { get; set; }        
        public List<ErrorElement> ErrorItems { get; set; }
        public FormBuilder FormBuilder {get; set;}
        public DocumentBuilder DocumentBuilder { get; set; }
        public List<IMBaseElement> InputItems => FormBuilder.InputForm.InputItems;
        public string MyBaseURL { get; set; }
        public string MyResultURL { get; set; }
        public string ScriptPath { get; set; }
        public Dictionary<string, ScriptInfo> ScriptRepository { get; set; }
        public Stack<CmdBaseCommand> ReturnStack { get; set; }
        public bool ExecuteNow { get; set; }
        public RunContext()
        {
            BoundVars = new Dictionary<string, string>();
            GivenKeys = new List<string>();
            InternalVars = new Dictionary<string, string>();            
            ErrorItems = new List<ErrorElement>();
            DocumentBuilder = new DocumentBuilder();
            FormBuilder = new FormBuilder();
            MyBaseURL = string.Empty;
            MyResultURL = string.Empty;
            ScriptPath = string.Empty;
            ScriptRepository = new Dictionary<string, ScriptInfo>();
            ReturnStack = new Stack<CmdBaseCommand>();
            ExecuteNow = false;
        }
        public void SetError(ReadContext readcontext, string errorcode, string errortext)
        {
            ErrorItems.Add(new ErrorElement(readcontext, errorcode, errortext));
        }
    }
}