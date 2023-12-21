namespace FlowProtocol2.Core
{
    public class DocumentBuilder
    {
        public OMDocument Document { get; private set; }
        public string CurrentSection { get; set; }
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
            var section = Document.Sections.Find(s => s.Caption == CurrentSection);
            if (section == null)
            {
                section = new OMSection();
                Document.Sections.Add(section);
            }
            var lastBlock = section.Textblocks.Last();
            if (l == Level.Level1)
            {
                if (lastBlock == null || lastBlock.BlockType != t)
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
                OMTextElement newtextelement = new OMTextElement();
                newtextelement.Text = text;
                newtextelement.Link = link;
                lastTextline.TextElements.Add(newtextelement);
            }
            else if (l == Level.Level2 && lastBlock != null)
            {
                var lastTextline = lastBlock.TextLines.Last();
                if (lastTextline != null)
                {
                    var lastSubblock = lastTextline.Subblocks.Last();
                    if (lastSubblock == null || lastSubblock.BlockType != t)
                    {
                        lastSubblock = new OMTextBlock();
                        lastSubblock.BlockType = t;
                        lastTextline.Subblocks.Add(lastSubblock);
                    }
                    var lastSubTextline = lastSubblock.TextLines.Last();
                    if (lastSubTextline == null)
                    {
                        lastSubTextline = new OMTextLine();
                        lastSubblock.TextLines.Add(lastSubTextline);
                    }
                    OMTextElement newtextelement = new OMTextElement();
                    newtextelement.Text = text;
                    newtextelement.Link = link;
                    lastSubTextline.TextElements.Add(newtextelement);
                }
            }
        }
    }
}