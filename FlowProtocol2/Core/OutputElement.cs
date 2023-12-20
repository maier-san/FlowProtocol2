namespace FlowProtocol2.Core
{
    using FlowProtocol2.Commands;
    public class OutputElement
    {
        public string Text {get; set;}
        public string Section4{get; set;}
        public string Link {get; set;}
        public OutputType Type {get; set;}    

        public OutputElement()
        {
            Text = string.Empty;
            Section4 = string.Empty;
            Link = string.Empty;
        }
    }

    public enum OutputType
    {
        None,
        Continuation,
        Paragraph,        
        Code,
        Enumeration,
        SubEnumeration,
        Listing,
        SubListing        
    }
}