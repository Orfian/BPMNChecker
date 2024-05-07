using BPMNChecker.Interfaces.Model;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace BPMNChecker.Model
{
    public class BaseElement : IBaseElement
    {
        public IBaseElement Parent { get; private init; }

        public string this[string attributeName] => throw new NotImplementedException();

        public string ID => this["id"];

        public ImmutableArray<XmlNode> SubNodes => throw new NotImplementedException();

        public BaseElement(IBaseElement parent) {
            this.Parent = parent;
        }
    }
}
