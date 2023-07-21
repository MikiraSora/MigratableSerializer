using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace MigratableSerializer.Base.Graph
{
    internal class Node<T>
    {
        public T Id { get; private set; }
        public List<Node<T>> OutgoingEdges { get; private set; }

        public Node(T id)
        {
            Id = id;
            OutgoingEdges = new List<Node<T>>();
        }

        public void AddEdge(Node<T> destination)
        {
            OutgoingEdges.Add(destination);
        }
    }
}
