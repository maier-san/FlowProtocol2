using System.Text;

namespace FlowProtocol2.Core
{
    public class OMDocument
    {
        public string Title { get; set; }
        public List<OMSection> Sections { get; set; }
        public OMDocument()
        {
            Title = string.Empty;
            Sections = new List<OMSection>();
        }
    }
    public class OMSection
    {
        public string Headline { get; set; }
        public List<OMTextBlock> Textblocks { get; set; }
        public OMSection()
        {
            Headline = string.Empty;
            Textblocks = new List<OMTextBlock>();
        }
    }
    public class OMTextBlock
    {
        public OutputType BlockType { get; set; }
        public List<OMTextLine> TextLines { get; set; }
        public string NumerationType { get; set; }
        public bool Closed { get; set; }
        public string  ID { get; set; }

        /// <summary>
        /// Gibt den im Block enthaltenen Text als String zurück.
        /// </summary>
        public string Codeblock
        {
            get
            {
                bool notfirstline = false;
                StringBuilder codegenerator = new StringBuilder(string.Empty);
                foreach (var tl in TextLines)
                {
                    if (notfirstline) codegenerator.Append('\n');
                    foreach (var te in tl.TextElements)
                    {
                        codegenerator.Append(te.Text);
                        notfirstline = true;
                    }
                }
                return codegenerator.ToString();
            }
        }
        public OMTextBlock()
        {
            BlockType = OutputType.None;
            NumerationType = "1";
            TextLines = new List<OMTextLine>();
            Closed = false;
            ID = string.Empty;
        }
    }
    public class OMTextLine
    {
        public List<OMTextElement> TextElements { get; set; }
        public List<OMTextBlock> Subblocks { get; set; }
        public OMTextLine()
        {
            TextElements = new List<OMTextElement>();
            Subblocks = new List<OMTextBlock>();
        }
    }
    public class OMTextElement
    {

        public string Text { get; set; }
        public string TrimText => Text.Trim();
        public string LeadingSpace
        {
            get
            {
                if (TrimText.Length < Text.Length) return " "; else return string.Empty;
            }
        }
        public string Link { get; set; }
        public bool Codeformat { get; set; }
        public OMTextElement()
        {
            Text = string.Empty;
            Link = string.Empty;
        }
    }
}