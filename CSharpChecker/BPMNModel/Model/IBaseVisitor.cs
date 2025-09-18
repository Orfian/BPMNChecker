using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMNModel.Model
{
    public interface IBaseVisitor<out TResult> where TResult : class
    {
        TResult? VisitOnce([NotNull] ITraversableNode node);
        TResult? VisitOnceChildren([NotNull] ITraversableNode node);
    }
}
