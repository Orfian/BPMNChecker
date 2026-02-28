using SharpVectors.Converters;
using Serilog;
using System.Collections.Concurrent;
using BPMNModel.Model;
using BPMNVisualizer.Utility;
using Task = BPMNModel.Model.Task;
using Utility;

namespace BPMNVisualizer.Utilities
{
    public class SvgResourceManager
    {
        private readonly ILogger _logger;
        private readonly string _basePath;
        private readonly ConcurrentDictionary<string, Uri> _uriCache = new();

        public SvgResourceManager(string basePath = "Assets")
        {
            _logger = SharedVariables.Instance.Logger;
            _basePath = basePath;
        }

        public SvgViewbox? GetSvg(string category, string fileName)
        {
            if (string.IsNullOrEmpty(fileName)) return null;

            var cacheKey = $"{category}/{fileName}";
            return _uriCache.GetOrAdd(cacheKey, key =>
            {
                try
                {
                    return new Uri($"pack://application:,,,/{_basePath}/{key}.svg");
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, $"Failed to create URI for SVG resource: {cacheKey}");
                    //return null;
                    throw new BPMNCheckerExceptions($"Failed to create URI for SVG resource: {cacheKey}", ex);
                }
            }) is Uri uri ? new SvgViewbox { Source = uri } : null;
        }

        public SvgViewbox? GetSubProcessIcon(SubProcess subProcess)
        {
            return GetSvg("Activities", "subprocess");
        }
        
        //TEMPORARY
        public SvgViewbox? GetSubProcessIcon(CallActivity subProcess)
        {
            return GetSvg("Activities", "subprocess");
        }

        public SvgViewbox? GetTaskIcon(Task task)
        {
            var fileName = task switch
            {
                SendTask => "send",
                ReceiveTask => "receive",
                UserTask => "user",
                ManualTask => "manual",
                BusinessRuleTask => "business-rule",
                ServiceTask => "service",
                ScriptTask => "script",
                _ => null
            };

            return fileName != null ? GetSvg("Activities/Tasks", fileName) : null;
        }

        public SvgViewbox? GetGatewayIcon(Gateway gateway)
        {
            var fileName = gateway switch
            {
                ExclusiveGateway => "exclusive",
                InclusiveGateway => "inclusive",
                ParallelGateway => "parallel",
                EventBasedGateway => "event",
                ComplexGateway => "complex",
                _ => null
            };

            return fileName != null ? GetSvg("Gateways", fileName) : null;
        }

        public SvgViewbox? GetEventIcon(Event evt)
        {
            var (definition, isThrowing) = Helpers.GetEventDefinition(evt);
            if (definition == null) return null;

            var fileName = definition switch
            {
                MessageEventDefinition => "message",
                TimerEventDefinition => "timer",
                ErrorEventDefinition => "error",
                EscalationEventDefinition => "escalation",
                ConditionalEventDefinition => "condition",
                LinkEventDefinition => "link",
                SignalEventDefinition => "signal",
                CompensateEventDefinition => "compensation",
                TerminateEventDefinition => "termination",
                CancelEventDefinition => "cancel",
                _ => null
            };

            if (fileName == null) return null;
            if (isThrowing) fileName += "-end";
            
            return GetSvg("Events", fileName);
        }
        
        public SvgViewbox? GetMarker(string markerType)
        {
            var fileName = markerType switch
            {
                "SubProcess" => "subprocess",
                "AdHoc" => "adhoc",
                "Sequential-MultiInstance" => "sequential",
                "Parallel-MultiInstance" => "parallel",
                "Loop" => "loop",
                "Compensation" => "compensation",
                _ => null
            };

            return fileName != null ? GetSvg("Activities", fileName) : null;
        }

        public SvgViewbox? GetDataIcon(BaseElement elementType)
        {
            var fileName = elementType switch
            {
                DataObject => "data-object",
                DataStoreReference => "data-store",
                DataInput => "data-input",
                DataOutput => "data-output",
                _ => throw new BPMNCheckerExceptions($"No icon found for data element type: {elementType}")
            };

            return GetSvg("Data", fileName);
        }
    }
}