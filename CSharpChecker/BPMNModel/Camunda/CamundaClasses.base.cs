using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BPMNModel.XMLParser;

namespace BPMNModel.Camunda
{
    public interface ICamundaBaseElement
    {
    }

    public interface ICamundaLoaderBase
    {
        void Load(XmlParserCamundaNode node);
    }
}
