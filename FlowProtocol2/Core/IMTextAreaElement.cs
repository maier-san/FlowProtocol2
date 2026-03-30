namespace FlowProtocol2.Core
{
    public class IMTextAreaElement : IMBaseElement
    {
        public IMTextAreaElement()
        {
            ShowLines = 5;
            UploadFilter = string.Empty;
        }

        public int ShowLines { get; set; }
        public string UploadFilter { get; set; }
    }
}
