namespace FlowProtocol2.Core
{
    public class IMTextAreaElement : IMBaseElement
    {
        public IMTextAreaElement()
        {
            ShowLines = 5;
        }

        public int ShowLines { get; set; }
    }
}
