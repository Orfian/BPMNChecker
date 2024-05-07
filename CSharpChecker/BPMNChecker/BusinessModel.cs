using System.Reflection.Metadata;
using System.Xml;

namespace BPMNChecker
{
    public class BusinessModel
    {
        public XmlDocument Document { get; private init; }
        public BusinessModel(XmlDocument doc)
        {
            Document = new XmlDocument();
            Document.LoadXml(doc.OuterXml);


        }
    }
}
