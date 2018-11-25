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
            List<Edge> neighbours = edges.Where(e => e.IsNextTo(this)).ToList();
            return neighbours;

        }
        public bool IsNextTo(Edge edge)
        {
            bool result = false;
            if (this.VertexA == edge.VertexB)
            {
                result = true;
            }
            return result;
        }
    }
}