namespace FlowProtocol2.Core
{
    public class IMForm
    {
        public string Titel { get; set; }
        public List<IMBaseElement> InputItems { get; set; }
        public IMForm()
        {
            Titel = string.Empty;
            InputItems = new List<IMBaseElement>();
        }
    }

    public abstract class IMBaseElement
    {
        public string Promt { get; set; }
        public string Key { get; set; }
        public List<IMHelpInfoLine> HelpInfoLines { get; set; }
        public IMHelpInfoLine? CurrentHelpInfoLine { get; set; }

        public IMBaseElement()
        {
            Promt = string.Empty;
            Key = string.Empty;
            HelpInfoLines = new List<IMHelpInfoLine>();
        }
    }

    public class IMHelpInfoLine
    {
        public List<OMTextElement> TextElements { get; set; }
        public IMHelpInfoLine()
        {
            TextElements = new List<OMTextElement>();
        }
    }

    public class IMTextElement : IMBaseElement
    {
        public IMTextElement() : base()
        {
        }
    }

    public class IMOptionGroupElement : IMBaseElement
    {
        public List<IMOptionValue> Options { get; set; }
        public IMOptionValue? SelectedValue { get; set; }
        public IMOptionGroupElement() : base()
        {
            Options = new List<IMOptionValue>();
        }
    }

    public class IMOptionValue
    {
        public string Key { get; set; }
        public string UniqueKey => ParentOptionGroup.Key + "_" + Key;
        public string Promt { get; set; }
        private IMOptionGroupElement ParentOptionGroup { get; set; }
        public IMOptionValue(IMOptionGroupElement parentOptionGroup)
        {
            ParentOptionGroup = parentOptionGroup;
            Key = string.Empty;
            Promt = string.Empty;
        }
    }
}