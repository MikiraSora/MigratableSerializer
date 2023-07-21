using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace MigratableSerializer.Base.Graph
{
    internal class DirectedGraph<T>
    {
        private Dictionary<T, Node<T>> nodes = new();
        private readonly Func<T, T, bool> eqFunc;

        public DirectedGraph(Func<T, T, bool> eqFunc)
        {
            this.eqFunc = eqFunc;
        }

        public void AddEdge(T from, T to)
        {
            if (!nodes.TryGetValue(from, out var fromNode))
                fromNode = nodes[from] = new(from);
            if (!nodes.TryGetValue(to, out var toNode))
                toNode = nodes[to] = new(to);

            fromNode.AddEdge(toNode);
        }

        public List<Node<T>> GetOutgoingEdges(T nodeId)
        {
            if (nodes.TryGetValue(nodeId, out Node<T> node))
                return node.OutgoingEdges;

            return null;
        }

        public List<T> FindPath(T from, T to)
        {
            var path = new List<T>();
            var visited = new HashSet<T>();

            bool DFS(T currentNodeId, T endNodeId)
            {
                if (eqFunc(currentNodeId, endNodeId))
                {
                    path.Add(currentNodeId);
                    return true;
                }

                visited.Add(currentNodeId);
                var outgoingEdges = GetOutgoingEdges(currentNodeId);
                if (outgoingEdges != null)
                {
                    foreach (var node in outgoingEdges)
                    {
                        if (!visited.Contains(node.Id) && DFS(node.Id, endNodeId))
                        {
                            path.Add(currentNodeId);
                            return true;
                        }
                    }
                }

                return false;
            }

            if (DFS(from, to))
            {
                path.Reverse();
                return path;
            }

            return null;
        }
    }
}
