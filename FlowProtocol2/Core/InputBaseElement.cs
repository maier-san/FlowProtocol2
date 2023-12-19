namespace FlowProtocol2.Core
{
    using FlowProtocol2.Commands;
    public abstract class InputBaseElement
    {
        public string Promt {get; set;}
        public string Key {get; set;}

        public InputBaseElement()
        {
            Promt = string.Empty;
            Key = string.Empty;            
        }
    }
}