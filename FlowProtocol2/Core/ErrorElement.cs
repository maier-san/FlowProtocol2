namespace FlowProtocol2.Core
{
    using FlowProtocol2.Commands;
    public class ErrorElement
    {
        ReadContext ReadContext {get; set;}
        string ErrorText {get; set;}
        string ErrorCode {get; set;}

        public ErrorElement(ReadContext readcontext, string errortext, string errorcode)
        {
            ReadContext = readcontext;
            ErrorText = errortext;
            ErrorCode = errorcode;
        }

    }
}