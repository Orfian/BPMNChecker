
using BPMNModel;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Formatting.Json;
using System.Xml.Linq;
using Utility;
using static System.Runtime.InteropServices.JavaScript.JSType;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace Testing
{
    internal class Program
    {
        static void Main(string[] args)
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

//            XDocument doc = ResourcesUtility.LoadResourceAsXDocument($@"diagrams/single_user_task.bpmn");
//            XDocument doc = ResourcesUtility.LoadResourceAsXDocument($@"diagrams/subprocesses.bpmn");
//            XDocument doc = ResourcesUtility.LoadResourceAsXDocument($@"diagrams/MonthlyInvoicing-solution.bpmn");
            XDocument doc = ResourcesUtility.LoadResourceAsXDocument($@"diagrams/Multi-instanceMessagingBetweenProcesses-Doctor.bpmn");
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

        }


    }

}
