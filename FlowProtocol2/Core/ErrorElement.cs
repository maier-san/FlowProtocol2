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


        /* This is needed to compare objects of this type - is used for UnitTests primary but can be also useful in other Contexts */
        public override bool Equals(object? obj)
        {
            if(typeof(ErrorElement) != obj?.GetType())
            {
                return false;
            }

            var typedObj = (ErrorElement)obj;

            return (typedObj.ErrorCode == ErrorCode) && (typedObj.ErrorText == ErrorText) && (typedObj.ReadContext.Equals(ReadContext));
        }

        //Needs to be done properly (TODO)
        public override int GetHashCode()
        {
            return ReadContext.GetHashCode() + ErrorText.GetHashCode() + ErrorCode.GetHashCode();
        }

        public override string ToString()
        {
            return $"Error {ErrorCode ?? "No Error Code"}: {ErrorText ?? "No Error Text"}";
        }

    }
}