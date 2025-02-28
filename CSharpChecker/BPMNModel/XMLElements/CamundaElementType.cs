using BPMNModel.Camunda;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMNModel.XMLElements
{
    public class CamundaElementType : ElementType
    {
        public CamundaElementType(CamundaJSonType camundaJSonType)
        {
            CamundaJSonType = camundaJSonType;
        }

        public List<Attribute> Attributes { get; } = new();

        public string? BodyElementName { get; set; }
        public CamundaJSonType CamundaJSonType { get; init; }
        public Dictionary<string, Element> InnerElementsByTypeElementName { get; } = new();
        public string Name => CamundaJSonType.Name;
        public CamundaElementType? Parent { get; set; }

        /// <summary>
        /// Determines whether this instance [can be cast into] the specified type.
        /// </summary>
        /// <param name="type">The target, more general, type.</param>
        /// <returns>
        ///   <c>true</c> if this instance [can be cast into] the specified type; otherwise, <c>false</c>.
        /// </returns>
        public bool CanBeCastInto(CamundaElementType type)
        {
            if (type.Name.Equals(Name))
            {
                return true;
            }
            else if (Parent == null)
            {
                return false;
            }
            else
            {
                return Parent.CanBeCastInto(type);
            }
        }
    }
}