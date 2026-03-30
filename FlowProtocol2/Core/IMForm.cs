using FlowProtocol2.Commands;

namespace FlowProtocol2.Core
{
    public class IMForm
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public List<IMBaseElement> InputItems { get; private set; }
        public string CurrentSection { get; set; }
        private string LastSection { get; set; }
        
        /// <summary>
        /// Flag wird auf true gesetzt, wenn ein TextArea-Element mit UploadFilter hinzugefügt wird.
        /// </summary>
        public bool NeedsUploadFunction { get; set; }
        
        public IMForm()
        {
            Title = string.Empty;
            Description = string.Empty;
            InputItems = new List<IMBaseElement>();
            CurrentSection = string.Empty;
            LastSection = string.Empty;
            NeedsUploadFunction = false;
        }

        public void AddInputItem(IMBaseElement inputitem)
        {
            if (CurrentSection != LastSection)
            {
                inputitem.Section = CurrentSection;
                LastSection = CurrentSection;
            }
            InputItems.Add(inputitem);
            
            // Setze NeedsUploadFunction auf true, wenn ein TextArea mit UploadFilter hinzugefügt wird
            var textArea = inputitem as IMTextAreaElement;
            if (textArea != null && !string.IsNullOrEmpty(textArea.UploadFilter))
            {
                NeedsUploadFunction = true;
            }
        }
    }

    public abstract class IMBaseElement
    {
        public string Prompt { get; set; }
        public string Key { get; set; }
        public string Section { get; set; }
        public IMHelpInfoBlock HelpInfoBlock { get; set; }


        public IMBaseElement()
        {
            Prompt = string.Empty;
            Key = string.Empty;
            Section = string.Empty;
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
            AddHelpText(text, string.Empty, false);
        }
        public void AddHelpText(string text, string link, bool isonwhitelist)
        {
            if (!string.IsNullOrEmpty(text))
            {
                if (CurrentHelpInfoLine == null)
                {
                    CurrentHelpInfoLine = new IMHelpInfoLine();
                    HelpInfoLines.Add(CurrentHelpInfoLine);
                }
                IMHelpTextElement newTextElement = new IMHelpTextElement();
                newTextElement.Text = text;
                newTextElement.Link = link;
                newTextElement.IsOnWhitelist = isonwhitelist;
                CurrentHelpInfoLine.TextElements.Add(newTextElement);
            }
        }
    }

    public class IMHelpInfoLine
    {
        public List<IMHelpTextElement> TextElements { get; set; }
        public IMHelpInfoLine()
        {
            TextElements = new List<IMHelpTextElement>();
        }
    }

    public class IMHelpTextElement : IMBaseElement
    {
        public string Text { get; set; }
        public string TrimText => Text.Trim();
        public string LinkText
        {
            get
            {
                if (IsOnWhitelist || (!string.IsNullOrWhiteSpace(Link) && Text.Contains(Link.Replace("https://", string.Empty))))
                {
                    return Text.Trim();
                }
                else
                {
                    return $"{Text.Trim()} ({Link})";
                }
            }
        }
        public string LeadingSpace
        {
            get
            {
                if (TrimText.Length < Text.Length) return " "; else return string.Empty;
            }
        }
        public string Link { get; set; }
        public bool IsOnWhitelist { get; set; }
        public IMHelpTextElement() : base()
        {
            Text = string.Empty;
            Link = string.Empty;
            IsOnWhitelist = false;
        }
    }
}