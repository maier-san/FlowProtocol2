namespace FlowProtocol2.Helper
{
    public class BreadcrumbItem
    {
        public string Name { get; set; }
        public string Path { get; set; }

        public BreadcrumbItem(string name, string path)
        {
            Name = name;
            Path = path;
        }
    }
}
