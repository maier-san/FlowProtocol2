namespace FlowProtocol2.Commands
{
    using FlowProtocol2.Core;
    public class RunContext
    {
        public Dictionary<string, string> BoundVars;
        public List<string> GivenKeys { get; set; }
        public Dictionary<string, string> InternalVars { get; set; }
        public List<ErrorElement> ErrorItems { get; set; }
        public IMForm InputForm { get; set; }
        public DocumentBuilder DocumentBuilder { get; set; }
        public List<IMBaseElement> InputItems => InputForm.InputItems;
        public string MyBaseURL { get; set; }
        public string MyResultURL { get; set; }
        public string ScriptPath {get; set;}
        public string CurrentScriptPath { get; set; }
        public Dictionary<string, ScriptInfo> ScriptRepository { get; set; }
        public Stack<EntryPoint> ReturnStack { get; set; }
        public bool ExecuteNow { get; set; }
        public string BaseKey { get; set; }
        public RunContext()
        {
            BoundVars = new Dictionary<string, string>();
            GivenKeys = new List<string>();
            InternalVars = new Dictionary<string, string>();
            ErrorItems = new List<ErrorElement>();
            DocumentBuilder = new DocumentBuilder();
            InputForm = new IMForm();
            MyBaseURL = string.Empty;
            MyResultURL = string.Empty;
            ScriptPath = string.Empty;
            CurrentScriptPath = string.Empty;
            ScriptRepository = new Dictionary<string, ScriptInfo>();
            ReturnStack = new Stack<EntryPoint>();
            ExecuteNow = false;
            BaseKey = string.Empty;
        }
        public void SetError(ReadContext readcontext, string errorcode, string errortext)
        {
            ErrorItems.Add(new ErrorElement(readcontext, errorcode, errortext));
        }
    }

    public class EntryPoint
    {
        public CmdBaseCommand EntryCommand {get; set;}
        public string BaseKey {get;set;}
        public EntryPoint(CmdBaseCommand entrycommand, string basekey)
        {
            EntryCommand = entrycommand;
            BaseKey = basekey;
        }
    }
}