namespace FlowProtocol2.Core
{
    public class OMDocument
    {
        public string Titel { get; set; }
        public List<OMSection> Sections { get; set; }
        public OMDocument()
        {
            Titel = string.Empty;
            Sections = new List<OMSection>();
        }
    }
    public class OMSection
    {
        public string Caption { get; set; }
        public List<OMTextBlock> Textblocks { get; set; }
        public OMSection()
        {
            Caption = string.Empty;
            Textblocks = new List<OMTextBlock>();
        }
    }
    public class OMTextBlock
    {
        public OutputType BlockType { get; set; }
        public List<OMTextLine> TextLines { get; set; }
        public OMTextBlock()
        {
            BlockType = OutputType.None;
            TextLines = new List<OMTextLine>();
        }
    }
    public class OMTextLine
    {
        public List<OMTextElement> TextElements { get; set; }
        public OMTextBlock? SubBlock { get; set; }
        public OMTextLine()
        {
            TextElements = new List<OMTextElement>();
        }
    }
    public class OMTextElement
    {
        public string Text { get; set; }
        public string Link { get; set; }
        public OMTextElement()
        {
            Text = string.Empty;
            Link = string.Empty;
        }
    }
}