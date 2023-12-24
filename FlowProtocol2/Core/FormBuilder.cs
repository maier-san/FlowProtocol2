namespace FlowProtocol2.Core
{
    public class FormBuilder
    {  
        public IMForm InputForm {get; private set;}
        public FormBuilder()
        {
            InputForm = new IMForm();
        }
        public void AddInputItem(IMBaseElement inputitem)
        {
            InputForm.InputItems.Add(inputitem);
        }
    }
}