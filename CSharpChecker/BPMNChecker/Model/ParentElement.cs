using BPMNChecker.Interfaces.Model;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace BPMNChecker.Model
{
    public class ParentElement : BaseElement, IParentElement
    {
        public ImmutableArray<IBaseElement> Children { get; private set; }
        public ParentElement(IBaseElement parent) : base(parent)
        {
            Children = [];
        }
    }
}
