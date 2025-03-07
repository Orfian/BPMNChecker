using BPMNChecker;
using BPMNModel.Camunda;
using BPMNModel.Model;
using BPMNModel.XMLElements;
using BPMNModel.XMLParser;

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

            CamundaExtensions.EnrichGenerator(Log.Logger, generator, true, @"d:\toPython");
            //CamundaExtensions.EnrichGenerator(Log.Logger, generator, true);

            //XDocument doc = ResourcesUtility.LoadResourceAsXDocument($@"diagrams/single_user_task.bpmn");
            //XDocument doc = ResourcesUtility.LoadResourceAsXDocument($@"diagrams/subprocesses.bpmn");
            //XDocument doc = ResourcesUtility.LoadResourceAsXDocument($@"diagrams/MonthlyInvoicing-solution.bpmn");
            //XDocument doc = ResourcesUtility.LoadResourceAsXDocument($@"diagrams/Multi-instanceMessagingBetweenProcesses-Doctor.bpmn");

            //XDocument doc = ResourcesUtility.LoadResourceAsXDocument($@"diagrams/httpConnector.bpmn");

            //XDocument doc = ResourcesUtility.LoadResourceAsXDocument($@"diagrams/httpConnectorPOST.bpmn");
            //XDocument doc = ResourcesUtility.LoadResourceAsXDocument($@"diagrams/all_tasks.bpmn");
            //XDocument doc = ResourcesUtility.LoadResourceAsXDocument($@"diagrams/all_icons.bpmn");
            //XDocument doc = ResourcesUtility.LoadResourceAsXDocument($@"diagrams/BookHolidaySagaPatternV2.bpmn");
            //XDocument doc = ResourcesUtility.LoadResourceAsXDocument($@"diagrams/CamundaModeler_almost_all_set.bpmn");
            XDocument doc = ResourcesUtility.LoadResourceAsXDocument($@"diagrams/CamundaModeler_almost_all_set_color.bpmn");
            //XDocument doc = ResourcesUtility.LoadResourceAsXDocument($@"diagrams/timing.bpmn");
            var parser = XmlParser.Parse(Log.Logger, generator, doc, false);

            if (parser.HasErrors)
            {
                Log.Error("\nERRORS:");
                foreach (var error in parser.Errors)
                {
                    Log.Error(error);
                }
            }
            else
            {
                if (parser.HasWarnings)
                {
                    Log.Warning("\nWARNINGS:");
                    foreach (var warning in parser.Warnings)
                    {
                        Log.Warning(warning);
                    }
                }

                parser.DumpXML("processedXML.xml");

                if (parser.Root is not null)
                {
                    var model = Factory.ProcessModel(Log.Logger, parser);
                    model.DumpModel("processedCMOF.cmof");

                    var analyzer = new StaticAnalysis();
                    analyzer.ProcessAnalysis(model);

                }
            }
        }
    }
}