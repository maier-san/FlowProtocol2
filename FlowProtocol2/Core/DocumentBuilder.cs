namespace FlowProtocol2.Core
{
    public class DocumentBuilder
    {
        public OMDocument Document { get; private set; }
        public string CurrentSection { get; set; }
        private OMTextBlock? CurrentTextBlock {get; set;}
        private OMTextLine? CurrentTextline { get; set; }
        public DocumentBuilder()
        {
            Document = new OMDocument();
            CurrentSection = string.Empty;
        }
        public void SetTitel(string titel)
        {
            Document.Titel = titel;
        }
        /// <summary>
        /// Fügt eine neue Textzeile zum Dokument hinzu
        /// </summary>
        /// <param name="l">Level, auf dem die Zeile hinzugefügt werden soll (1 oder 2).</param>
        /// <param name="t">Typ des Textblocks</param>
        /// <param name="text">Der Text der dargestellt werden soll.</param>
        public void AddNewTextLine(Level l, OutputType t, string text)
        {
            if (t == OutputType.None) return;
            var section = Document.Sections.Find(s => s.Headline == CurrentSection);
            if (section == null)
            {
                section = new OMSection();
                section.Headline = CurrentSection;
                Document.Sections.Add(section);
            }
            var lastBlock = section.Textblocks.LastOrDefault();
            if (l == Level.Level1)
            {
                if (lastBlock == null || lastBlock.BlockType != t || lastBlock.Closed)
                {
                    lastBlock = new OMTextBlock();
                    lastBlock.BlockType = t;
                    lastBlock.NumerationType = "1";
                    section.Textblocks.Add(lastBlock);
                }                
                OMTextLine newtextline = new OMTextLine();
                lastBlock.TextLines.Add(newtextline);
                OMTextElement newtextelement = new OMTextElement();
                newtextelement.Text = text;
                newtextline.TextElements.Add(newtextelement);
                CurrentTextBlock = lastBlock;
                CurrentTextline = newtextline;
            }
            else if (l == Level.Level2 && lastBlock != null)
            {
                var lastTextline = lastBlock.TextLines.LastOrDefault();
                if (lastTextline != null)
                {
                    var lastSubblock = lastTextline.Subblocks.LastOrDefault();
                    if (lastSubblock == null || lastSubblock.BlockType != t)
                    {
                        lastSubblock = new OMTextBlock();
                        lastSubblock.BlockType = t;
                        lastSubblock.NumerationType = "a";
                        lastTextline.Subblocks.Add(lastSubblock);
                    }
                    OMTextLine newSubTextline = new OMTextLine();
                    lastSubblock.TextLines.Add(newSubTextline);
                    OMTextElement newtextelement = new OMTextElement();
                    newtextelement.Text = text;
                    newSubTextline.TextElements.Add(newtextelement);
                    CurrentTextline = newSubTextline;
                }
            }
        }

        /// <summary>
        /// Fügt ein Textelement zur letzten Textzeile hinzu.
        /// </summary>
        /// <param name="text">Der Text der dargestellt werden soll.</param>
        /// <param name="link">URL eines Links, falls der Text als Link dargestellt werden soll. string.Empty falls nicht.</param>
        /// <param name="codeformat">True, wenn das Textelement als Code formatiert werden soll.</param>
        public void AddNewTextElement(string text, string link, bool codeformat)
        {
            if (CurrentTextline != null)
            {
                OMTextElement newtextelement;
                var lasttextelement = CurrentTextline.TextElements.LastOrDefault();
                if (lasttextelement != null && string.IsNullOrEmpty(lasttextelement.Text))
                {
                    newtextelement = lasttextelement;
                }
                else
                {
                    newtextelement = new OMTextElement();
                }
                newtextelement.Text = text;
                newtextelement.Link = link;
                newtextelement.Codeformat = codeformat;
                CurrentTextline.TextElements.Add(newtextelement);
            }
        }

        public void EndParagraph()
        {
            if (CurrentTextBlock != null)
            {
                CurrentTextBlock.Closed = true;
            }
        }
    }
}