using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMNModel.Model
{
    /// <summary>
    /// Simple waker example  - walking based on linear traversal of the model tree where each node is visited once and nodes are sorted by their 
    /// </summary>
    public class LinearWalker : IWalker
    {
        

        public void Walk(ITraversableNode start, IBaseListener listener)
        {
            var visitedNodes = new List<ITraversableNode>();

            var toResolve = new List<ITraversableNode>
            {
                start
            };

            while (toResolve.Count > 0)
            {
                var node = toResolve[0];
                toResolve.RemoveAt(0);
                if (!visitedNodes.Contains(node))
                {
                    visitedNodes.Add(node);
                    foreach (var child in node.GetChildElements())
                    {
                        if (!visitedNodes.Contains(child))
                        {
                            toResolve.Add(child);
                        }
                    }
                }
            }

            foreach (var node in visitedNodes)
            {
                node.Enter(listener);
                node.Exit(listener);
            }

        }
    }
}
