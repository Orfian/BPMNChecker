using BPMNModel.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMNChecker
{
    internal class StepTwo : BaseModelListener
    {
        override public void EnterEndEvent([NotNull] EndEvent context)
        {
            Console.WriteLine($"Entering EndEvent with id: {context.Id} by listener");
        }
        override public void ExitEndEvent([NotNull] EndEvent context)
        {
            Console.WriteLine($"Exiting EndEvent with id: {context.Id} by listener");
        }
    }
}
