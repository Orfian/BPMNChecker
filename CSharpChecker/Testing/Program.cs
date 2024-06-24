using BPMNModel;
using BPMNModel.Camunda;
using BPMNModel.Model;

//using BPMNModel.Model;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Json;
using System.Xml.Linq;
using Utility;

namespace Testing
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                           // add console as logging target
                           .WriteTo.Console(restrictedToMinimumLevel: LogEventLevel.Debug)
                           // add a logging target for warnings and higher severity  logs
                           // structured in JSON format
                           .WriteTo.File(new JsonFormatter(), "important.json", LogEventLevel.Warning)
                           // add a rolling file for all logs
                           .WriteTo.File("all.logs",
                                         restrictedToMinimumLevel: LogEventLevel.Verbose,
                                         rollingInterval: RollingInterval.Day)
                           .MinimumLevel.Verbose()
            .CreateLogger();

            var generator = Generator.CreateGenerator(Log.Logger);

            CamundaExtensions.EnrichGenerator(Log.Logger, generator);

            return;
            /*
            int count = 0;
            using var writer = new StreamWriter(@"d:\extracted.types");
            writer.WriteLine("extracted = {");

            foreach(var item in generator.Types) {
                if (item.Value is ComplexType c) {
                    (List<BPMNModel.Attribute>  Attributes, List<string> Elements) GetAll(ComplexType current)
                    {
                        List<BPMNModel.Attribute> attributes = new();
                        List<string> elements = new();
                        if (current.ParentType is not null)
                        {
                            var parent = GetAll(current.ParentType);
                            attributes.AddRange(parent.Attributes);
                            elements.AddRange(parent.Elements);
                        }
                        attributes.AddRange(current.Attributes);
                        var currentElements = current.InnerElement?.InnerElements.Select(x => $"\"{x.Name}\"").ToList();

                        if (currentElements is not null && currentElements.Any()) elements.AddRange(currentElements);

                        return (attributes, elements);
                    }
                    var (allAttributes, allElements) = GetAll(c);

                    var required = allAttributes.Where(x => x.Use == AttributeUse.Required).Select(x => $"\"{x.Name}\"").ToList();
                    var optional = allAttributes.Where(x => x.Use == AttributeUse.Optional).Select(x => $"\"{x.Name}\"").ToList();
                    writer.WriteLine($"\t\"{c.Name.Substring(1)}\": ([{string.Join(", ", required)}], [{string.Join(", ", optional)}], [{string.Join(", ", allElements ?? [])}]),");
                    count++;
                }
            }
            writer.WriteLine("\t}\n");

            return;
            */

            //XDocument doc = ResourcesUtility.LoadResourceAsXDocument($@"diagrams/single_user_task.bpmn");
            //XDocument doc = ResourcesUtility.LoadResourceAsXDocument($@"diagrams/subprocesses.bpmn");
            //XDocument doc = ResourcesUtility.LoadResourceAsXDocument($@"diagrams/MonthlyInvoicing-solution.bpmn");
            //XDocument doc = ResourcesUtility.LoadResourceAsXDocument($@"diagrams/Multi-instanceMessagingBetweenProcesses-Doctor.bpmn");
            //XDocument doc = ResourcesUtility.LoadResourceAsXDocument($@"diagrams/all_tasks.bpmn");
            //XDocument doc = ResourcesUtility.LoadResourceAsXDocument($@"diagrams/all_icons.bpmn");
            //XDocument doc = ResourcesUtility.LoadResourceAsXDocument($@"diagrams/BookHolidaySagaPatternV2.bpmn");
            XDocument doc = ResourcesUtility.LoadResourceAsXDocument($@"diagrams/CamundaModeler_almost_all_set.bpmn");
            Log.Logger.Debug("Opening file: diagrams/single_user_task.bpmn");

            var parser = XmlParser.Parse(Log.Logger, generator, doc);

            if (parser.HasErrors)
            {
                Log.Debug("\nERRORS:");
                foreach (var error in parser.Errors)
                {
                    Log.Error(error);
                }
            }
            else
            {
                parser.DumpXML("processedXML.xml");

                if (parser.Root is not null)
                {
                    var model = Factory.ProcessModel(Log.Logger, parser);
                    model.DumpModel("processedCMOF.cmof");
                }
            }
        }
    }
}