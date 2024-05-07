
using Serilog;
using System.Xml;
using System.Xml.Linq;
using Utility;

namespace BPMNModel
{
    public class Generator
    {

        private ILogger logger;

        public Dictionary<string, ElementType> Types { get; } = new();

        public Dictionary<string, RootElement> Elements { get; } = new();

        private Generator(ILogger logger)
        {
            this.logger = logger;
        }


        public static Generator CreateGenerator(ILogger logger)
        {
            XNamespace xs = XNamespace.Get("http://www.w3.org/2001/XMLSchema");
            void Load(XDocument document, Generator result)
            {
                if (document.Root is null) throw new BPMNCheckerExceptions("Application error.");

                foreach (var type in document.Root.Elements(xs + "complexType"))
                {
                    var name = ElementType.GetExpectedAttribute(type, "name");

                    //TODO: Removing diagram element from definition.
                    if (name == "tDefinitions")
                    {
                        var diagram = type.Descendants().FirstOrDefault(e => e.Name.LocalName == "element" && e.Attribute("ref")?.Value.StartsWith("bpmndi") == true);
                        diagram?.Remove();
                    }
                   
                    result.Types.Add(name, ComplexType.Create(type));
                }

                foreach (var type in document.Root.Elements(xs + "simpleType"))
                {
                    var name = ElementType.GetExpectedAttribute(type, "name");
                    result.Types.Add(name, SimpleType.Create(xs, type));
                }
                foreach (var element in document.Root.Elements(xs + "element"))
                {
                    var name = ElementType.GetExpectedAttribute(element, "name");
                    var type = ElementType.GetExpectedAttribute(element, "type");
                    var groupAttribute = element.Attribute("substitutionGroup");

                    var innerType = result.Types[type] as ComplexType;

                    if (innerType == null) throw new BPMNCheckerExceptions($"File Semantic.xsd is broken ({name} has a type {type} which should be complex).");

                    result.Elements.Add(name, new RootElement(name, innerType, groupAttribute?.Value));
                }
            }

            Generator result = new Generator(logger);

            XDocument semantics = ResourcesUtility.LoadResourceAsXDocument($@"definitions/Semantic.xsd");
            
            Load(semantics, result);

            XDocument bpmn20 = ResourcesUtility.LoadResourceAsXDocument($@"definitions/BPMN20.xsd");

            Load(bpmn20, result);


            foreach (var type in result.Types)
            {
                if (type.Value is ComplexType ct)
                {
                    ct.FullLoad(xs, result.Types, result.Elements);
                }
            }

            return result;

        }


    }
}
