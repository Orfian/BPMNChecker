using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using BPMNModel;
using BPMNModel.Model;
using BPMNVisualizer.Utility;
using Point = System.Windows.Point;

namespace BPMNVisualizer.Simulation;

public class TokenManager
{
    private readonly ModelRoot _model;
    private readonly List<BPMNToken> _tokens;
    
    internal readonly Canvas _canvas;

    public TokenManager(Canvas canvas)
    {
        _canvas = canvas;
        _model = SharedVariables.Instance.Model;
        _tokens = SharedVariables.Instance.Tokens;
    }

    public BPMNToken AddToken(BaseElement startElement, Rect position)
    {
        var token = new BPMNToken
        {
            CurrentElement = startElement,
            Visual = new Ellipse
            {
                Width = 20,
                Height = 20,
                Fill = Brushes.Green,
                Stroke = Brushes.Black,
                StrokeThickness = 1,
                RenderTransform = new TranslateTransform()
            }
        };
        _tokens.Add(token);
        token.Owner = this;
        var center = new Point(position.X + position.Width / 2, position.Y + position.Height / 2);
        Canvas.SetLeft(token.Visual, center.X - token.Visual.Width / 2);
        Canvas.SetTop(token.Visual, center.Y - token.Visual.Height / 2);
        _canvas.Children.Add(token.Visual);
        
        return token;
    }

    public void MoveToken(BPMNToken token, BaseElement target, SequenceFlow currentFlow, IEnumerable<Point>? path, Rect position)
    {
        var points = path.ToList();
        if (path == null || points.Count < 2)
        {
            token.CurrentElement = target;
            token.CurrentSequenceFlow = currentFlow;
            var center = new Point(position.X + position.Width / 2, position.Y + position.Height / 2);
            Canvas.SetLeft(token.Visual, center.X - token.Visual.Width / 2);
            Canvas.SetTop(token.Visual, center.Y - token.Visual.Height / 2);
        }
        else
        {
            AnimateToken(token, target, currentFlow, points);
        }
    }
    
    public List<BPMNToken> GetAllTokens()
    {
        return _tokens.ToList();
    }
    
    public void RemoveToken(BPMNToken token)
    {
        var canvas = token.Owner?._canvas ?? _canvas;
        canvas.Children.Remove(token.Visual);
        _tokens.Remove(token);
    }
    
    public void ClearAllTokens()
    {
        foreach (var token in _tokens)
        {
            var canvas = token.Owner?._canvas ?? _canvas;
            canvas.Children.Remove(token.Visual);
        }
        _tokens.Clear();
    }
    
    public void SetTokenWaiting(BPMNToken token, bool waiting)
    {
        if (token.Visual == null) return;
        token.IsWaiting = waiting;
        token.Visual.Fill = waiting ? Brushes.Orange : Brushes.Green;
    }

    public void SetTokenPosition(BPMNToken token, Point position)
    {
        if (token.Visual == null) return;
        Canvas.SetLeft(token.Visual, position.X - token.Visual.Width / 2);
        Canvas.SetTop(token.Visual, position.Y - token.Visual.Height / 2);
    }
    
    public IEnumerable<SequenceFlow> GetOutgoingFlows(BaseElement element)
    {
        return _model.AllObjectsWithIds.Values
            .OfType<SequenceFlow>()
            .Where(flow => flow.SourceRef?.Id == element.Id);
    }

    public IEnumerable<SequenceFlow> GetIncomingFlows(BaseElement element)
    {
        return  _model.AllObjectsWithIds.Values
            .OfType<SequenceFlow>()
            .Where(flow => flow.TargetRef?.Id == element.Id);
    }
    
    public BaseElement? GetTargetElement(SequenceFlow flow)
    {
        var targetRef = flow.TargetRef;
        if (targetRef == null)
            return null;
        
        if (_model.AllObjectsWithIds.TryGetValue(targetRef.Id, out var target))
            return target as BaseElement;
        
        return null;
    }
    
    public bool IsReachable(BaseElement start, BaseElement target)
    {
        if (start == null || target == null)
            return false;

        if (start.Id == target.Id)
            return true;

        var visited = new HashSet<string>();
        var queue = new Queue<BaseElement>();
        queue.Enqueue(start);

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();

            if (visited.Contains(current.Id))
                continue;

            visited.Add(current.Id);

            var outgoingFlows = GetOutgoingFlows(current);
            if (outgoingFlows == null || !outgoingFlows.Any())
                continue;

            foreach (var flow in outgoingFlows)
            {
                var next = GetTargetElement(flow);
                if (next == null) continue;

                if (next.Id == target.Id)
                    return true;

                if (!visited.Contains(next.Id))
                    queue.Enqueue(next);
            }
        }

        return false;
    }
    
    public void AnimateToken(BPMNToken token, BaseElement target, SequenceFlow currentFlow, List<Point> points)
    {
        var first = points[0];
        Canvas.SetLeft(token.Visual, first.X - token.Visual.Width / 2);
        Canvas.SetTop(token.Visual, first.Y - token.Visual.Height / 2);

        // compute path length to determine duration (pixels per second)
        double totalLength = 0.0;
        for (int i = 1; i < points.Count; i++)
        {
            totalLength += (points[i] - points[i - 1]).Length;
        }

        const double pixelsPerSecond = 300.0; // tune this for speed
        double seconds = Math.Max(0.2, totalLength / pixelsPerSecond);

        var xFrames = new DoubleAnimationUsingKeyFrames();
        var yFrames = new DoubleAnimationUsingKeyFrames();
        var totalDuration = TimeSpan.FromSeconds(seconds);
        xFrames.Duration = yFrames.Duration = new Duration(totalDuration);

        // Build cumulative lengths to compute proportional key times (makes speed uniform)
        var cumulative = new double[points.Count];
        cumulative[0] = 0;
        for (int i = 1; i < points.Count; i++)
            cumulative[i] = cumulative[i - 1] + (points[i] - points[i - 1]).Length;

        for (int i = 0; i < points.Count; i++)
        {
            var p = points[i];
            double x = p.X - token.Visual.Width / 2;
            double y = p.Y - token.Visual.Height / 2;

            double t = cumulative[i] / cumulative[^1]; // 0..1
            var keyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(seconds * t));

            // linear frames — replace with SplineDoubleKeyFrame + KeySpline if you want easing
            var xKey = new LinearDoubleKeyFrame(x, keyTime);
            var yKey = new LinearDoubleKeyFrame(y, keyTime);

            xFrames.KeyFrames.Add(xKey);
            yFrames.KeyFrames.Add(yKey);
        }

        // Storyboard so we can handle Completed
        var sb = new Storyboard { Duration = xFrames.Duration };
        Storyboard.SetTarget(xFrames, token.Visual);
        Storyboard.SetTargetProperty(xFrames, new PropertyPath("(Canvas.Left)"));
        Storyboard.SetTarget(yFrames, token.Visual);
        Storyboard.SetTargetProperty(yFrames, new PropertyPath("(Canvas.Top)"));

        sb.Children.Add(xFrames);
        sb.Children.Add(yFrames);

        sb.Completed += (s, e) =>
        {
            // ensure final snap to last point
            var last = points[^1];
            Canvas.SetLeft(token.Visual, last.X - token.Visual.Width / 2);
            Canvas.SetTop(token.Visual, last.Y - token.Visual.Height / 2);
        };

        sb.Begin();
        token.CurrentElement = target;
        token.CurrentSequenceFlow = currentFlow;
    }

    public Polygon AddArrowIndicator(Point position, Vector direction)
    {
        Vector orthogonal = new Vector(-direction.Y, direction.X);

        var arrow = new Polygon
        {
            Points = new PointCollection
            {
                position + (direction * 20),
                position + (orthogonal * 10),
                position - (orthogonal * 10)
            },
            Stroke = Brushes.Black,
            StrokeThickness = 1,
            Fill = Brushes.Yellow,
        };

        _canvas.Children.Add(arrow);
    
        return arrow;
    }
    
    public void ShowGatewayChoiceIndicators(GatewayChoice gatewayChoice)
    {
        if (gatewayChoice.OutgoingFlows == null) return;

        foreach (var flow in gatewayChoice.OutgoingFlows)
        {
            if (flow.Id == null || !SharedVariables.Instance.Paths.TryGetValue(flow.Id, out var path))
                continue;
            
            var points = path?.ToList();
            if (points == null || points.Count < 2) continue;

            var first = points.First();
            var second = points.Skip(1).FirstOrDefault();
            var direction = Helpers.GetDirection(first, second);
            var arrow = AddArrowIndicator(first, direction);
            
            var indicator = new Indicator
            {
                Visual = arrow,
                Flow = flow,
                Selected = false
            };
            
            gatewayChoice.Indicators.Add(indicator);
            
            arrow.MouseDown += (s, e) =>
            {
                UpdatePendingGatewayChoices(indicator, gatewayChoice);
            };
            
            arrow.MouseEnter += (s, e) =>
            {
                SetHoverIndicatorColor(arrow, indicator.Selected, true);
            };
            
            arrow.MouseLeave += (s, e) =>
            {
                SetHoverIndicatorColor(arrow, indicator.Selected, false);
            };
        }
    }
    
    public void ShowEventTriggerIndicator(EventTrigger eventTrigger)
    {
        var position = Helpers.GetElementCenter(eventTrigger.Event);
        var arrow = AddArrowIndicator(position, new Vector(1, 0));
        
        var indicator = new Indicator
        {
            Visual = arrow,
            Flow = null,
            Selected = false
        };
        
        eventTrigger.Indicator = indicator;
        
        arrow.MouseDown += (s, e) =>
        {
            eventTrigger.Indicator.Selected = !eventTrigger.Indicator.Selected;
            SetIndicatorColor(arrow, eventTrigger.Indicator.Selected);
        };
        
        arrow.MouseEnter += (s, e) =>
        {
            SetHoverIndicatorColor(arrow, eventTrigger.Indicator.Selected, true);
        };
        
        arrow.MouseLeave += (s, e) =>
        {
            SetHoverIndicatorColor(arrow, eventTrigger.Indicator.Selected, false);
        };
    }
    
    public Polygon SetIndicatorColor(Polygon indicator, bool selected)
    {
        var newColor = selected ? Brushes.LawnGreen : Brushes.Yellow;
        indicator.Fill = newColor;
        
        return indicator;
    }
    
    public Polygon SetHoverIndicatorColor(Polygon indicator, bool selected, bool hover)
    {
        var newColor = selected ? Brushes.LawnGreen : Brushes.Yellow;
        if (hover)
        {
            newColor = selected ? Brushes.LimeGreen : Brushes.Gold;
        }
        indicator.Fill = newColor;
        
        return indicator;
    }
    
    public void RemoveChoiceIndicator(Polygon indicator)
    {
        _canvas.Children.Remove(indicator);
    }
    
    public void UpdatePendingGatewayChoices(Indicator triggerIndicator, GatewayChoice gatewayChoice)
    {
        if (triggerIndicator.Selected && gatewayChoice.Gateway is not ComplexGateway && gatewayChoice.Gateway is not EventBasedGateway)
        {
            bool selected = false;
            foreach (var indicator in gatewayChoice.Indicators)
            {
                if (indicator.Selected && indicator != triggerIndicator)
                {
                    selected = true;
                    break;
                }
            }

            if (!selected)
                return;
        }
        
        triggerIndicator.Selected = !triggerIndicator.Selected;
        SetIndicatorColor(triggerIndicator.Visual, triggerIndicator.Selected);
        
        if (!gatewayChoice.MultiSelect)
        {
            foreach (var indicator in gatewayChoice.Indicators)
            {
                if (indicator != triggerIndicator && indicator.Selected)
                {
                    indicator.Selected = false;
                    SetIndicatorColor(indicator.Visual, false);
                }
            }
        }
        else if (gatewayChoice.DefaultFlow != null)
        {
            var defaultIndicator = gatewayChoice.Indicators
                .FirstOrDefault(ind => ind.Flow == gatewayChoice.DefaultFlow);
            
            if (defaultIndicator == null)
                return;
            
            if (triggerIndicator.Flow == defaultIndicator.Flow)
            {
                foreach (var indicator in gatewayChoice.Indicators)
                {
                    if (indicator != triggerIndicator && indicator.Selected)
                    {
                        indicator.Selected = false;
                        SetIndicatorColor(indicator.Visual, false);
                    }
                }
            }
            else
            {
                defaultIndicator.Selected = false;
                SetIndicatorColor(defaultIndicator.Visual, false);
            }
        }
    }
}
