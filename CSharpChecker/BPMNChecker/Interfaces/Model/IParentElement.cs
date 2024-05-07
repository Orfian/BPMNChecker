using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace BPMNChecker.Interfaces.Model
{
    public interface IParentElement
    {
        ImmutableArray<IBaseElement> Children { get; }
    }
}
