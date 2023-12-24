namespace FlowProtocol2.Core
{
    public abstract class InputBaseElement
    {
        public string Promt { get; set; }
        public string Key { get; set; }
        public List<HelpInfoLine> HelpInfoLines { get; set; }
        public HelpInfoLine? CurrentHelpInfoLine { get; set; }

        public InputBaseElement()
        {
            Promt = string.Empty;
            Key = string.Empty;
            HelpInfoLines = new List<HelpInfoLine>();
        }

        public void AddHelpLine(string text, string link)
        {
            CurrentHelpInfoLine = new HelpInfoLine();
            HelpInfoLines.Add(CurrentHelpInfoLine);
            AddHelpText(text, link);
        }
        public void AddHelpText(string text, string link)
        {
            if (CurrentHelpInfoLine != null)
            {
                OMTextElement te = new OMTextElement();
                te.Text = text;
                te.Link = link;
                CurrentHelpInfoLine.TextElements.Add(te);
            }
        }
    }

    public class HelpInfoLine
    {
        public List<OMTextElement> TextElements { get; set; }
        public HelpInfoLine()
        {
            TextElements = new List<OMTextElement>();
        }
    }
}