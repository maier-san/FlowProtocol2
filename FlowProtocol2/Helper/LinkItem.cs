namespace FlowProtocol2.Helper
{
    public class LinkItem
    {
        public string ShowText { get; set; }
        public string Link { get; set; }

        public LinkItem(string showtext, string link)
        {
            ShowText = showtext;
            Link = link;
        }
    }
}