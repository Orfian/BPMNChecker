using System.Xml.Linq;

using BPMNModel.Camunda;
using BPMNModel.Model;
using BPMNModel.XMLElements;
using BPMNModel.XMLParser;

using Serilog;
using Serilog.Events;
using Serilog.Formatting.Json;
using Serilog.Sinks.File;
using Utility;

namespace UnitTests;

[TestClass]
public class DiagramLoading
{
    [TestMethod]
    public void LoadWithNoDefinition()
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

        CamundaExtensions.EnrichGenerator(Log.Logger, generator, true);

        XDocument doc = ResourcesUtility.LoadResourceAsXDocument($@"diagrams/ModelsAndDiagrams/CallActivity.bpmn");

        var parser = XmlParser.Parse(Log.Logger, generator, doc, true);

        if (parser.HasErrors)
        {
            foreach (var error in parser.Errors)
            {
                Log.Error(error);
            }
        }

        var model = Factory.ProcessModel(Log.Logger, parser);
    }

    [TestMethod]
    public void PrefixTest()
    {
        // otestovat tagy type <semantic:element> a <semantic:complexType>
    }
}
