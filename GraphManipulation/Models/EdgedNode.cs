using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Testing.Models
{
    class EdgedNode<NodeValue, EdgeValue> : Node<NodeValue>
    {
        /// <summary>
        /// To be used for undirected graphs;
        /// </summary>
        public List<Edge<EdgeValue>> Responses;
        /// <summary>
        /// To be used for directed graphs
        /// </summary>
        public List<Edge<EdgeValue>> OutgoingRespones;
        public List<Edge<EdgeValue>> IncomingResponses;
    }
}
