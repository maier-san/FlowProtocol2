namespace FlowProtocol2.Core
{
    using FlowProtocol2.Commands;
    public class ErrorElement
    {
        public ReadContext ReadContext { get; set; }
        public string ErrorText { get; set; }
        public string ErrorCode { get; set; }

        public ErrorElement(ReadContext readcontext, string errorcode, string errortext)
        {
            ReadContext = readcontext;
            ErrorText = errortext;
            ErrorCode = errorcode;
        }

    }
}