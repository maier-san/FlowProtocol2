namespace FlowProtocol2.Core
{
    public class DocumentBuilder
    {
        public OMDocument Document {get; private set;}
        public string CurrentSection {get; set;}
        public DocumentBuilder()
        {
            Document = new OMDocument();
            CurrentSection = string.Empty;
        }
        public void SetTitel(string titel)
        {
            Document.Titel = titel;
        }
        public void AddTextElement(Level l, OutputType t, string text, string link)
        {
            var section = Document.Sections.Find(s=>s.Caption==CurrentSection);
            if (section==null)
            {
                section = new OMSection();
                Document.Sections.Add(section);
            }
            var lastBlock = section.Textblocks.Last();
            if (lastBlock==null || lastBlock.BlockType != t)
            {
                lastBlock = new OMTextBlock();
                lastBlock.BlockType = t;
                section.Textblocks.Add(lastBlock);
            }
            var lastTextline = lastBlock.TextLines.Last();
            if (lastTextline == null)
            {
                lastTextline = new OMTextLine();
                lastBlock.TextLines.Add(lastTextline);
            }
            // Level-Unterscheidung
        }    
    }
}