using BPMNModel.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMNChecker
{
    internal class StepOne : BaseModelVisitor<string>
    {
        public override string? VisitEndEvent([NotNull] EndEvent context)
        {
            Console.WriteLine($"EndEvent found with id: {context.Id} by visitor.");
            return base.VisitEndEvent(context);
        }
    }
}
