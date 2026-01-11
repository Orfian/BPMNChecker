using System.Windows;
using BPMNModel.Model;
using Point = System.Windows.Point;

namespace BPMNVisualizer.Utility;

public static class Helpers
{
    public static (EventDefinition? Definition, bool IsThrowing) GetEventDefinition(Event e)
    {
        return e switch
        {
            CatchEvent catchEvent => (catchEvent.EventDefinitions.FirstOrDefault(), false),
            ThrowEvent throwEvent => (throwEvent.EventDefinitions.FirstOrDefault(), true),
            _ => (null, e is EndEvent)
        };
    }
    
    public static bool IsDefaultFlow(SequenceFlow flow)
    {
        return flow.SourceRef switch
        {
            Activity a => a.Default?.Id == flow.Id,
            ComplexGateway cg => cg.Default?.Id == flow.Id,
            ExclusiveGateway eg => eg.Default?.Id == flow.Id,
            InclusiveGateway ig => ig.Default?.Id == flow.Id,
            _ => false
        };
    }
    
    public static bool IsConditionalFlow(SequenceFlow flow)
    {
        if (flow.ConditionExpression != null)
        {
            if (flow.SourceRef is Gateway)
            {
                return false;
            }

            return true;
        }

        return false;
    }
    
    public static Vector GetDirection(Point first, Point second)
    {
        var direction = second - first;
        direction.Normalize();
        return direction;
    }
    
    public static double CalculateDistance(Point a, Point b)
    {
        double dx = b.X - a.X;
        double dy = b.Y - a.Y;
        return Math.Sqrt(dx * dx + dy * dy);
    }
    
    public static bool IsUserStart(StartEvent startEvent)
    {
        if (startEvent?.EventDefinitions == null || !startEvent.EventDefinitions.Any())
            return false;

        var def = startEvent.EventDefinitions.First();
        return def is MessageEventDefinition || def is SignalEventDefinition || def is ConditionalEventDefinition || def is TimerEventDefinition;
    }
}