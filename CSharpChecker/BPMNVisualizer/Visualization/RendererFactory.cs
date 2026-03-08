using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using BPMNModel;
using BPMNModel.Model;
using BPMNVisualizer.Utilities;
using BPMNVisualizer.Utility;
using BPMNVisualizer.Visualization.Renderers;
using Serilog;
using Utility;
using DataObject = BPMNModel.Model.DataObject;
using Point = System.Windows.Point;

namespace BPMNVisualizer.Visualization
{
    public class RendererFactory
    {
        private readonly Canvas _canvas;
        private readonly BrushManager _brushManager;
        private readonly ShapeManager _shapeManager;
        private readonly SvgResourceManager _svgResourceManager;
        /*
        private readonly ILogger _logger;
        private readonly Dictionary<string, Rect> _objectBounds;
        private readonly Dictionary<string, IEnumerable<Point>> _paths;
        private readonly Dictionary<string, BPMNShape> _shapes;
        */
        private readonly ActivityRenderer _activityRenderer;
        private readonly EventRenderer _eventRenderer;
        private readonly GatewayRenderer _gatewayRenderer;
        private readonly DataRenderer _dataRenderer;
        private readonly ConnectionRenderer _connectionRenderer;
        private readonly ParticipantRenderer _participantRenderer;

        public RendererFactory(Canvas canvas)
        {
            _canvas = canvas;
            _brushManager = new BrushManager();
            _shapeManager = new ShapeManager();
            _svgResourceManager = new SvgResourceManager();

            _activityRenderer = new ActivityRenderer(_canvas, _brushManager, _shapeManager, _svgResourceManager);
            _eventRenderer = new EventRenderer(_canvas, _brushManager, _shapeManager, _svgResourceManager);
            _gatewayRenderer = new GatewayRenderer(_canvas, _brushManager, _shapeManager, _svgResourceManager);
            _dataRenderer = new DataRenderer(_canvas, _brushManager, _shapeManager, _svgResourceManager);
            _connectionRenderer = new ConnectionRenderer(_canvas);
            _participantRenderer = new ParticipantRenderer(_canvas, _brushManager, _shapeManager);
        }
        
        public IShapeRenderer GetStructureRenderer(BaseElement element)
        {
            return _participantRenderer;
        }
        
        public IShapeRenderer? GetShapeRenderer(BaseElement element)
        {
            switch (element)
            {
                /*
                case Activity:
                    return new ActivityRenderer(_logger, _canvas, _brushManager, _shapeManager, _svgResourceManager, _objectBounds);
                case Event:
                    return new EventRenderer(_logger, _canvas, _brushManager, _shapeManager, _svgResourceManager, _objectBounds);
                case Gateway:
                    return new GatewayRenderer(_logger, _canvas, _brushManager, _shapeManager, _svgResourceManager, _objectBounds);
                case DataObject:
                    return new DataRenderer(_logger, _canvas, _brushManager, _shapeManager, _svgResourceManager, _objectBounds);
                case DataStoreReference:
                    return new DataRenderer(_logger, _canvas, _brushManager, _shapeManager, _svgResourceManager, _objectBounds);
                case DataInput:
                    return new DataRenderer(_logger, _canvas, _brushManager, _shapeManager, _svgResourceManager, _objectBounds);
                case DataOutput:
                    return new DataRenderer(_logger, _canvas, _brushManager, _shapeManager, _svgResourceManager, _objectBounds);
                    */
                case Activity:
                    return _activityRenderer;
                case Event:
                    return _eventRenderer;
                case Gateway:
                    return _gatewayRenderer;
                case DataStoreReference:
                case DataInput:
                case DataOutput:
                case DataObjectReference:
                    return _dataRenderer;
                case Participant:
                case Lane:
                case Group:
                case TextAnnotation:
                    return _participantRenderer;
                default:
                    SharedVariables.Instance.Logger.Warning("No renderer found for element type: {ElementType}", element.GetType());
                    return null;
                    //throw new BPMNCheckerExceptions($"No renderer found for element type: {element.GetType()}");
            }
        }

        public ConnectionRenderer GetConnectionRenderer(BaseElement element)
        {
            //return new ConnectionRenderer(_logger, _canvas, _paths);
            return _connectionRenderer;
        }
    }
}