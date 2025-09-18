using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMNModel.Model
{
    public interface IWalker
    {
        void Walk(ITraversableNode node, IBaseListener listener);
    }
}
