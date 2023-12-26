namespace FlowProtocol2.Helper
{
    public class NavLink
    {
        public string ShowText { get; set; }
        public string RelativePath { get; set; }
        public NavLink(string showtext, string relativpath)
        {
            ShowText = showtext;
            RelativePath = relativpath;
        }
    }
}
