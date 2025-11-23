using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using BPMNModel;
using BPMNModel.Model;
using Point = System.Windows.Point;

namespace BPMNVisualizer.Simulation;

public class TokenManager
{
    private readonly Canvas _canvas;
    private readonly List<BPMNToken> _tokens = new();

    public TokenManager(Canvas canvas)
    {
        _canvas = canvas;
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
        _canvas.Children.Remove(token.Visual);
        _tokens.Remove(token);
    }
    
    public void ClearTokens()
    {
        foreach (var token in _tokens)
        {
            _canvas.Children.Remove(token.Visual);
        }
        _tokens.Clear();
    }
    
    public void SetTokenWaiting(BPMNToken token, bool waiting)
    {
        token.IsWaiting = waiting;
        token.Visual.Fill = waiting ? Brushes.Orange : Brushes.Green;
    }
    
    public IEnumerable<SequenceFlow> GetOutgoingFlows(ModelRoot model, BaseElement element)
    {
        return model.AllObjectsWithIds.Values
            .OfType<SequenceFlow>()
            .Where(flow => flow.SourceRef?.Id == element.Id);
    }

    public IEnumerable<SequenceFlow> GetIncomingFlows(ModelRoot model, BaseElement element)
    {
        return  model.AllObjectsWithIds.Values
            .OfType<SequenceFlow>()
            .Where(flow => flow.TargetRef?.Id == element.Id);
    }
    
    public BaseElement? GetTargetElement(ModelRoot model, SequenceFlow flow)
    {
        var targetRef = flow.TargetRef;
        if (targetRef == null)
            return null;
        
        if (model.AllObjectsWithIds.TryGetValue(targetRef.Id, out var target))
            return target as BaseElement;
        
        return null;
    }

    public IEnumerable<BPMNToken> Tokens => _tokens;

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
}
