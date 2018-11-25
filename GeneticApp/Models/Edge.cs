using System.Collections.Generic;
using System.Linq;

namespace GeneticApp.Models
{
    public class Edge
    {
        public int VertexA { get; set; }
        public int VertexB { get; set; }
        public int Cost { get; set; }
        public int Index { get; set; }
        public bool Visited { get; set; }

        public Edge(int vertexA, int vertexB, int cost, int index)
        {
            Cost = cost;
            VertexA = vertexA;
            VertexB = vertexB;
            Visited = false;
            Index = index;
        }

        public List<Edge> GetNeighbours(List<Edge> edges)
        {
            return edges.Where(e => IsNextTo(e)).ToList();
        }

        public bool IsNextTo(Edge edge)
        {
            return VertexA == edge.VertexB;
        }
    }
}