using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace BPMNChecker.Interfaces.Model
{
    public interface IBaseElement
    {
        public IBaseElement Parent { get; }

        string ID { get; }

        ImmutableArray<XmlNode> SubNodes { get; }

        string this[string attributeName] { get; }
    }
}
