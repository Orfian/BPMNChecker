using Serilog;
using System.Xml.Linq;
using Utility;

namespace BPMNModel.XMLElements
{
    public class Generator
    {
        private ILogger logger;

        private Generator(ILogger logger)
        {
            this.logger = logger;
        }

        /// <summary>
        /// Gets the camunda types based on element names, names are in format like: camunda:field
        /// </summary>
        /// <value>
        /// (camunda:name - small letter at the begining , processed camunda types)
        /// </value>
        public Dictionary<string, CamundaElementType> CamundaTypes { get; } = new();

        public Dictionary<string, RootElement> Elements { get; } = new();
        public Dictionary<string, ElementType> Types { get; } = new();

        public static Generator CreateGenerator(ILogger logger)
        {
            XNamespace xs = XNamespace.Get("http://www.w3.org/2001/XMLSchema");
            void Load(XDocument document, Generator result, string prefix)
            {
                if (document.Root is null) throw new BPMNCheckerExceptions("Application error.");

                foreach (var type in document.Root.Elements(xs + "complexType"))
                {
                    var name = ElementType.GetExpectedAttribute(type, "name");

                    var fullName = prefix + name;

                    //TODO: Removing diagram element from definition.
                    /*
                     if (name == "tDefinitions")
                    {
                        var diagram = type.Descendants().FirstOrDefault(e => e.Name.LocalName == "element" && e.Attribute("ref")?.Value.StartsWith("bpmndi") == true);
                        diagram?.Remove();
                    }
                    */

                    result.Types.Add(fullName, ComplexType.Create(fullName, type));
                }

                foreach (var type in document.Root.Elements(xs + "simpleType"))
                {
                    var name = ElementType.GetExpectedAttribute(type, "name");
                    var fullName = prefix + name;
                    result.Types.Add(fullName, SimpleType.Create(xs, fullName, type));
                }
                foreach (var element in document.Root.Elements(xs + "element"))
                {
                    var name = ElementType.GetExpectedAttribute(element, "name");
                    var fullName = prefix + name;
                    var type = ElementType.GetExpectedAttribute(element, "type");
                    var groupAttribute = element.Attribute("substitutionGroup");

                    var innerType = result.Types[type] as ComplexType;

                    if (innerType == null) throw new BPMNCheckerExceptions($"Files with XSD definitions are broken ({fullName} has a type {type} which should be complex).");

                    result.Elements.Add(fullName, new RootElement(fullName, innerType, groupAttribute?.Value));
                }
            }

            Generator result = new Generator(logger);

            XDocument dc = ResourcesUtility.LoadResourceAsXDocument($@"definitions/DC.xsd");
            Load(dc, result, "dc:");

            XDocument di = ResourcesUtility.LoadResourceAsXDocument($@"definitions/DI.xsd");
            Load(di, result, "di:");

            XDocument bpmndi = ResourcesUtility.LoadResourceAsXDocument($@"definitions/BPMNDI.xsd");
            Load(bpmndi, result, "bpmndi:");

            XDocument semantics = ResourcesUtility.LoadResourceAsXDocument($@"definitions/Semantic.xsd");
            Load(semantics, result, "");

            XDocument bpmn20 = ResourcesUtility.LoadResourceAsXDocument($@"definitions/BPMN20.xsd");
            Load(bpmn20, result, "");

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