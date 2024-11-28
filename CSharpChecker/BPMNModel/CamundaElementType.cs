using BPMNModel.Camunda;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMNModel
{
    public class CamundaElementType : ElementType
    {
        public List<Attribute> Attributes { get; } = new();

        public Dictionary<string, Element> InnerElementsByTypeElementName { get; } = new();

        public CamundaJSonType CamundaJSonType {  get; init; }

        public CamundaElementType(CamundaJSonType camundaJSonType) { 
            CamundaJSonType = camundaJSonType;
        }

        public CamundaElementType? Parent { get; set; }

        public string? BodyElementName { get; set; }


    }
}
