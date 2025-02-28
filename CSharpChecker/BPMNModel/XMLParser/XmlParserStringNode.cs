namespace BPMNModel.XMLParser
{
    public class XmlParserStringNode : XmlParserNode
    {
        public XmlParserStringNode(string elementName, string value)
        {
            Value = value;
            ElementName = elementName;
        }

        public string ElementName { get; private set; }
        public string Value { get; private set; }

        public override void DumpNode(StreamWriter writer, string indent)
        {
            writer.WriteLine($"{indent}<{ElementName} = {Value} />");
        }

        public override string ToString()
        {
            return $"\"{Value}\"";
        }
    }
}