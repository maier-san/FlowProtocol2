using FlowProtocol2.Core;

namespace FlowProtocol2.Core
{
    public class InputOptionGroupElement : InputBaseElement
    {
        public List<OptionValue> Options { get; set; }
        public OptionValue? SelectedValue { get; set; }
        public InputOptionGroupElement() : base()
        {
            Options = new List<OptionValue>();
        }
    }

    public class OptionValue
    {
        public string Key { get; set; }
        public string Promt { get; set; }
        public OptionValue()
        {
            Key = string.Empty;
            Promt = string.Empty;
        }
    }
}