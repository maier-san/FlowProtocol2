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
            HelpInfoLines.Add(CurrentHelpInfoLine);
            AddHelpText(text, string.Empty);
        }
        public void AddHelpText(string text, string link)
        {
            if (CurrentHelpInfoLine != null && !string.IsNullOrEmpty(text)) 
            {
                IMHelpTextElement newTextElement = new IMHelpTextElement();
                newTextElement.Text = text;
                newTextElement.Link = link;
                CurrentHelpInfoLine.TextElements.Add(newTextElement);
            }
        }
    }

    public class IMHelpInfoLine
    {
        public List<IMHelpTextElement> TextElements {get; set;}
        public IMHelpInfoLine()
        {
            TextElements = new List<IMHelpTextElement>();
        }
    }

    public class IMHelpTextElement : IMBaseElement
    {
        public string Text {get; set;}
        public string TrimText => Text.Trim();
        public string LeadingSpace
        {
            get
            {
                if (TrimText.Length < Text.Length) return " "; else return string.Empty;
            }
        }
        public string Link {get; set;}
        public IMHelpTextElement() : base()
        {
            Text = string.Empty;
            Link = string.Empty;
        }
    }
}