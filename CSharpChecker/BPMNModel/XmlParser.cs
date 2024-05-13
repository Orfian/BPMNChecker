using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using Utility;

namespace BPMNModel
{
    public class XmlParser
    {
        private ILogger logger;

        private Generator generator;

        public XmlParserNode? Root { get; private set; }

        public List<string> Errors { get; } = new List<string>();

        public bool HasErrors { get =>  Errors.Any(); }

        private XmlParser(ILogger logger, Generator generator)
        {
            this.logger = logger;
            this.generator = generator;
        }

        private static string GetNiceMessage(XElement element, string text)
        {
            if (element is IXmlLineInfo info && info.HasLineInfo())
            {
                return $"{element.Name.LocalName}[{info.LineNumber}:{info.LinePosition}] - {text}";
            }
            else
            {
                return $"{element.Name.LocalName}[---] - {text}";
            }
        }


        private XmlParserNode? LoadAndCheck(XElement element, RootElement type)
        {
            logger.Debug(GetNiceMessage(element, "Starting to load."));

            if (element.Name.LocalName != type.Name)
            {
                Errors.Add(GetNiceMessage(element, $"Expecting element {type.Name}."));
                return null;
            }

            var processedAttributes = new List<XmlParserAttribute>();

            var allAttributes = type.Type.GetAllAttributes();

            Log.Debug(GetNiceMessage(element, $"  All attributes: "));
            foreach (var attribute in allAttributes)
            {
                Log.Debug(GetNiceMessage(element, $"    {attribute }"));
            }

            foreach (var attribute in allAttributes)
            {
                var (xmlAttribute, message) = attribute.CreateAndCheck(element);
                if (xmlAttribute != null)
                {
                    Log.Debug(GetNiceMessage(element, $"  Created attribute: {xmlAttribute.Name} = {xmlAttribute.Value}"));
                    processedAttributes.Add(xmlAttribute);
                }
                if (message != null)
                {
                    Errors.Add(GetNiceMessage(element, message));
                }
            }

            var idAttribute = processedAttributes.First(x => x.Name == "id");

            if (idAttribute == null)
            {
                Errors.Add(GetNiceMessage(element, $"every element should have id."));
                return null;
            }

            XmlParserNode node = new XmlParserNode(idAttribute.Value, type.Type);

            if (type.InnerComplexType is not null)
            {
                var allElements = type.InnerComplexType.GetAllElements();

                Log.Debug(GetNiceMessage(element, $"  All elements: "));
                foreach (var item in allElements)
                {
                    Log.Debug(GetNiceMessage(element, $"    {item}"));
                }

                foreach (var item in element.Elements())
                {
                    if (generator.Elements.ContainsKey(item.Name.LocalName))
                    {
                        var createdNode = LoadAndCheck(item, generator.Elements[item.Name.LocalName]);

                    }else
                    {
                        Errors.Add(GetNiceMessage(element, $"Skiping element: {item.Name.LocalName}"));
                    }
                }


            }

            logger.Debug(GetNiceMessage(element, "Finished loading."));
            return node;
        }


        public static XmlParser Parse(ILogger logger, Generator generator, XDocument document)
        {
            if (document.Root == null) throw new BPMNCheckerExceptions($"File has no root element.");

            XmlParser parser = new XmlParser(logger, generator);
            parser.Root = parser.LoadAndCheck(document.Root, generator.Elements["definitions"]);

            return parser;
        }
    }
}
