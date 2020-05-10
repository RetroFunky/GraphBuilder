using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Testing.Models
{
    /// <summary>
    /// all nodes have an int based id, a string name and a value that can be provided as a parameter.
    /// </summary>
    /// <typeparam name="NodeType"></typeparam>
    /// <typeparam name="EdgeType"></typeparam>
    public class Graph<NodeType, EdgeType>
    {
        public bool directed;

        public List<Node<NodeType>> nodes;

        public List<Edge<EdgeType>> edges;


    }
}
