using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMNModel.Model
{
    public abstract class AbstractModelVisitor<Result> : IBaseVisitor<Result>
            where Result : class
    {
        private List<ITraversableNode> visitedNodes = new List<ITraversableNode>();

        protected internal virtual Result? DefaultResult => default;

        public virtual Result? VisitOnceChildren([NotNull] ITraversableNode node)
        {
            Result? val = DefaultResult;
            foreach (var child in node.GetChildElements())
            {
                if (!visitedNodes.Contains(child))
                {
                    visitedNodes.Add(child);
                    Result? nextResult = child.Accept(this);
                    val = AggregateResult(val, nextResult);
                }
            }

            return val;
        }

        public virtual Result? VisitOnce([NotNull] ITraversableNode node)
        {
            if (!visitedNodes.Contains(node))
            {
                visitedNodes.Add(node);
                return node.Accept(this);
            }else
            {
                return DefaultResult;
            }
        }
        protected internal virtual Result? AggregateResult(Result? aggregate, Result? nextResult)
        {
            return nextResult;
        }
    }
}
