namespace FlowProtocol2.Commands
{
    using System.Text.RegularExpressions;
    public class ReadContext
    {
        public string ScriptFilePath { get; set; }
        public int Indent { get; set; }
        public int LineNumber { get; set; }
        public string CodeLine { get; set; }
        public Match? ExpressionMatch { get; set; }

        public ReadContext(string scriptfilepath, int indent, int linenumber, string codeline)
        {
            ScriptFilePath = scriptfilepath;
            Indent = indent;
            LineNumber = linenumber;
            CodeLine = codeline;
        }

        /* This is needed to compare objects of this type - is used for UnitTests primary but can be also useful in other Contexts */
     
        public override bool Equals(object? obj)
        {
            if (typeof(ReadContext) != obj?.GetType())
            {
                return false;
            }

            var typedObj = (ReadContext)obj;

            return (typedObj.ScriptFilePath == ScriptFilePath) && (typedObj.Indent == Indent) && (typedObj.LineNumber == LineNumber) && (typedObj.CodeLine == CodeLine);
        }

        //ExpressionMatch is currently not part of the HashCode
        public override int GetHashCode()
        {
            return ScriptFilePath.GetHashCode() + Indent.GetHashCode() + LineNumber.GetHashCode() + CodeLine.GetHashCode();
        }

        //ExpressionMatch is currently not part of the ToString-Method
        public override string ToString()
        {
            return "ReadContext: " + ScriptFilePath + " " + Indent + " " + LineNumber + " " + CodeLine;
        }
    }
}