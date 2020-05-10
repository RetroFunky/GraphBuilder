using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Testing.Models
{
    public class Edge<EdgeType>
    {
        public (int, int) edge;
        public EdgeType type;
    }
}
