using System.Windows.Controls;
using BPMNModel;
using BPMNModel.Model;
using BPMNVisualizer.Utilities;
using BPMNVisualizer.Utility;
using BPMNVisualizer.Visualization.Renderers;
using Serilog;
using Utility;

namespace BPMNVisualizer.Visualization
{
    public class RendererFactory
    {
        private readonly ILogger _logger;
        private readonly Canvas _canvas;
        private readonly BrushManager _brushManager;
        private readonly ShapeManager _shapeManager;
        private readonly SvgResourceManager _svgResourceManager;

        public RendererFactory(ILogger logger, Canvas canvas)
        {
            _logger = logger;
            _canvas = canvas;
            _brushManager = new BrushManager();
            _shapeManager = new ShapeManager();
            _svgResourceManager = new SvgResourceManager(_logger);
        }

        public IShapeRenderer GetStructureRenderer(BaseElement element)
        {
            return new ParticipantRenderer(_logger, _canvas, _brushManager, _shapeManager);
        }
        

        public IShapeRenderer? GetShapeRenderer(BaseElement element)
        {
            switch (element)
            {
                case Activity:
                    return new ActivityRenderer(_logger, _canvas, _brushManager, _shapeManager, _svgResourceManager);
                case Event:
                    return new EventRenderer(_logger, _canvas, _brushManager, _shapeManager, _svgResourceManager);
                case Gateway:
                    return new GatewayRenderer(_logger, _canvas, _brushManager, _shapeManager, _svgResourceManager);
                case DataObject:
                    return new DataRenderer(_logger, _canvas, _brushManager, _shapeManager, _svgResourceManager);
                case DataStoreReference:
                    return new DataRenderer(_logger, _canvas, _brushManager, _shapeManager, _svgResourceManager);
                case DataInput:
                    return new DataRenderer(_logger, _canvas, _brushManager, _shapeManager, _svgResourceManager);
                case DataOutput:
                    return new DataRenderer(_logger, _canvas, _brushManager, _shapeManager, _svgResourceManager);

                default:
                    _logger.Warning("No renderer found for element type: {ElementType}", element.GetType());
                    return null;
                    //throw new BPMNCheckerExceptions($"No renderer found for element type: {element.GetType()}");
            }
        }

        public ConnectionRenderer GetConnectionRenderer(BaseElement element)
        {
            return new ConnectionRenderer(_logger, _canvas, _shapeManager);
        }
    }
}