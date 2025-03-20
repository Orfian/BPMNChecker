using BPMNModel.Model;
using BPMNModel.XMLParser;
using Utility;

namespace BPMNModel.Camunda
{
    public class CamundaFactory
    {
        public static ICamundaLoaderBase Load(XmlParserCamundaNode node, Factory bpmnFactory)
        {
            var thisFactoryType = typeof(CamundaFactory);
            var assembly = thisFactoryType.Assembly;

            var loadedTypeName = thisFactoryType.Namespace + "." + CamundaExtensions.ConvertCSName(node.Type.Name);
            var loadedType = assembly.GetType(loadedTypeName);
            if (loadedType == null)
            {
                throw new BPMNCheckerExceptions($"Processing error, already checked node {node} refers to nonexisting type {loadedTypeName}.");
            }
            var loadedItemAsObject = Activator.CreateInstance(loadedType);

            if (loadedItemAsObject != null && loadedItemAsObject is ICamundaLoaderBase loadedItem)
            {
                loadedItem.Load(node, bpmnFactory);

                return loadedItem;
            }
            else
            {
                throw new BPMNCheckerExceptions($"Processing error, Class {loadedTypeName} should have a constructor without parameters.");
            }
        }

        public static void LoadElements<T>(List<T> target, List<XmlParserNode> nodes, Factory bpmnFactory) where T : class
        {
            foreach (var node in nodes)
            {
                if (node is XmlParserCamundaNode camundaNode)
                {
                    target.Add((T)(CamundaFactory.Load(camundaNode, bpmnFactory)));
                }
                else
                {
                    throw new BPMNCheckerExceptions($"Processing error, all nodes should be of type XmlParserCamundaNode, not true for: {node}");
                }
            }
        }
    }
}