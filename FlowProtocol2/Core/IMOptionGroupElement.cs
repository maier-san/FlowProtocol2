namespace FlowProtocol2.Core
{
    public class IMOptionGroupElement : IMBaseElement
    {
        public List<IMOptionValue> Options { get; set; }
        public IMOptionValue? SelectedValue { get; set; }
        public IMOptionGroupElement() : base()
        {
            Options = new List<IMOptionValue>();
        }
    }
}