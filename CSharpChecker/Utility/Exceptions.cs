using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility
{
    public class BPMNCheckerExceptions : Exception
    {
        public BPMNCheckerExceptions(string message) : base (message){ }
    }
}
