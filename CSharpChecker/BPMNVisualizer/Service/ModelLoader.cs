using BPMNModel;
using BPMNModel.XMLParser;
using Serilog;
using System.Xml.Linq;
using BPMNModel.Camunda;
using BPMNModel.Model;
using BPMNModel.XMLElements;
using Utility;


namespace BPMNVisualizer.Services
{
    public class ModelLoader
    {
        private readonly ILogger _logger;

        public ModelLoader(ILogger logger)
        {
            _logger = logger;
        }

        public ModelRoot LoadModel(string filePath)
        {
            XDocument doc = ResourcesUtility.LoadResourceAsXDocument(filePath);
            var generator = Generator.CreateGenerator(_logger);
            CamundaExtensions.EnrichGenerator(_logger, generator, true);

            var parser = XmlParser.Parse(_logger, generator, doc, false, true);

            if (parser.HasErrors)
            {
                LogParserErrors(parser);
                return null;
            }

            LogParserWarnings(parser);
            return parser.Root != null ? Factory.ProcessModel(_logger, parser) : null;
        }

        private void LogParserErrors(XmlParser parser)
        {
            _logger.Error("\nERRORS:");
            foreach (var error in parser.Errors) _logger.Error(error);
        }

        private void LogParserWarnings(XmlParser parser)
        {
            if (!parser.HasWarnings) return;

            _logger.Warning("\nWARNINGS:");
            foreach (var warning in parser.Warnings) _logger.Warning(warning);
        }
    }
}