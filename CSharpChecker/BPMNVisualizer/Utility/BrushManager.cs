using BPMNModel.Model;
using System.Windows.Media;
using System.Collections.Concurrent;
using Task = BPMNModel.Model.Task;

namespace BPMNVisualizer.Utility
{
    public class BrushManager
    {
        private readonly ConcurrentDictionary<Color, SolidColorBrush> _brushCache = new();
        private readonly ConcurrentDictionary<string, SolidColorBrush> _semanticCache = new();

        public BrushManager()
        {
            InitializeSemanticBrushes();
        }

        private void InitializeSemanticBrushes()
        {
            // Event brushes
            _semanticCache.TryAdd("Event.Default", Brushes.LightGray);
            _semanticCache.TryAdd("Event.Start", Brushes.LightGreen);
            _semanticCache.TryAdd("Event.End", Brushes.LightCoral);
            _semanticCache.TryAdd("Event.Boundary", Brushes.LightGoldenrodYellow);
            _semanticCache.TryAdd("Event.Intermediate.Catch", new SolidColorBrush(Brushes.LightGreen.Color) { Opacity = 0.25 });
            _semanticCache.TryAdd("Event.Intermediate.Throw", new SolidColorBrush(Brushes.LightCoral.Color) { Opacity = 0.25 });

            // Task brushes
            _semanticCache.TryAdd("Task.Default", Brushes.LightGray);
            _semanticCache.TryAdd("Task.Send", Brushes.LightSlateGray);
            _semanticCache.TryAdd("Task.Receive", Brushes.SteelBlue);
            _semanticCache.TryAdd("Task.User", Brushes.LightSkyBlue);
            _semanticCache.TryAdd("Task.Manual", Brushes.CadetBlue);
            _semanticCache.TryAdd("Task.BusinessRule", Brushes.SlateGray);
            _semanticCache.TryAdd("Task.Service", Brushes.LightSteelBlue);
            _semanticCache.TryAdd("Task.Script", Brushes.DarkGray);

            // Gateway brushes
            _semanticCache.TryAdd("Gateway.Default", Brushes.LightGray);
            _semanticCache.TryAdd("Gateway.Exclusive", Brushes.LightCoral);
            _semanticCache.TryAdd("Gateway.Parallel", Brushes.LightGreen);
            _semanticCache.TryAdd("Gateway.EventBased", Brushes.LightGoldenrodYellow);
            _semanticCache.TryAdd("Gateway.Complex", Brushes.LightBlue);
        }

        public SolidColorBrush GetBrush(Color color)
        {
            return _brushCache.GetOrAdd(color, c => new SolidColorBrush(c));
        }

        public SolidColorBrush GetEventBrush(Event eventElement)
        {
            return eventElement switch
            {
                StartEvent => _semanticCache["Event.Start"],
                EndEvent => _semanticCache["Event.End"],
                BoundaryEvent => _semanticCache["Event.Boundary"],
                IntermediateCatchEvent => _semanticCache["Event.Intermediate.Catch"],
                IntermediateThrowEvent => _semanticCache["Event.Intermediate.Throw"],
                _ => _semanticCache["Event.Default"]
            };
        }

        public SolidColorBrush GetActivityBrush(Activity activity)
        {
            return activity switch
            {
                SendTask => _semanticCache["Task.Send"],
                ReceiveTask => _semanticCache["Task.Receive"],
                UserTask => _semanticCache["Task.User"],
                ManualTask => _semanticCache["Task.Manual"],
                BusinessRuleTask => _semanticCache["Task.BusinessRule"],
                ServiceTask => _semanticCache["Task.Service"],
                ScriptTask => _semanticCache["Task.Script"],
                _ => _semanticCache["Task.Default"]
            };
        }

        public SolidColorBrush GetGatewayBrush(Gateway gateway)
        {
            return gateway switch
            {
                ExclusiveGateway => _semanticCache["Gateway.Exclusive"],
                InclusiveGateway => _semanticCache["Gateway.Exclusive"],
                ParallelGateway => _semanticCache["Gateway.Parallel"],
                EventBasedGateway => _semanticCache["Gateway.EventBased"],
                ComplexGateway => _semanticCache["Gateway.Complex"],
                _ => _semanticCache["Gateway.Default"]
            };
        }
    }
}