using FlowProtocol2.Commands;

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
        public IMHelpInfoBlock HelpInfoBlock { get; set; }


        public IMBaseElement()
        {
            Promt = string.Empty;
            Key = string.Empty;
            HelpInfoBlock = new IMHelpInfoBlock();
        }
    }
    public class IMHelpInfoBlock
    {
        public List<IMHelpInfoLine> HelpInfoLines { get; private set; }
        private IMHelpInfoLine? CurrentHelpInfoLine { get; set; }

        public IMHelpInfoBlock()
        {
            HelpInfoLines = new List<IMHelpInfoLine>();
        }
        public void AddHelpLine(string text)
        {
            CurrentHelpInfoLine = new IMHelpInfoLine();
            CurrentHelpInfoLine.Text = text;
            HelpInfoLines.Add(CurrentHelpInfoLine);
        }
    }

    public class IMHelpInfoLine
    {
        public string Text { get; set; }
        public IMHelpInfoLine()
        {
            Text = string.Empty;
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