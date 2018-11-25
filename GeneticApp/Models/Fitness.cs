using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Fitnesses;
using System.Collections.Generic;
using System.Linq;

namespace GeneticApp.Models
{
    internal class Fitness : IFitness
    {
        private List<Edge> _edges { get; set; }

        public Fitness(List<Edge> edges)
        {
            _edges = edges;
        }

        public double Evaluate(IChromosome chromosome)
        {
            Gene[] genes = chromosome.GetGenes();
            double distanceSum = 0.0;
            int lastEdgeIndex = int.Parse(genes[0].Value.ToString());

            // Węzeł początkowy
            int startingPoint = _edges[int.Parse(genes[0].Value.ToString())].VertexA;
            for (int i = 0; i < genes.Length; i++)
            {
                int edgeIndex = int.Parse(genes[i].Value.ToString());
                Edge edge = _edges[edgeIndex];

                // Znalezienie odwrotej krawędzi
                Edge reverseEdge = _edges.SingleOrDefault(e => e.VertexA == edge.VertexB && e.VertexB == edge.VertexA);

                // Oznaczenie krawędzi i jej odwrotności jako oznaczona
                edge.Visited = true;
                reverseEdge.Visited = true;

                if (i != 0 && edge.VertexA != _edges[int.Parse(genes[i - 1].Value.ToString())].VertexB)
                {
                    // Jeśli ścieżka nie jest poprawna koszt jest znacząco zwiększany
                    distanceSum += edge.Cost * 1000;
                    lastEdgeIndex = edgeIndex;
                }
                else
                {
                    distanceSum += edge.Cost;
                    lastEdgeIndex = edgeIndex;
                }

                // Sprawdzenie czy ścieżka jest zamknięta i czy wszystkie krawędzie zostały odwiedzone
                if (AllEdgesVisited(_edges))
                {
                    if (edge.VertexB == startingPoint)
                    {
                        break;
                    }

                    Edge possibleEdge = _edges.SingleOrDefault(e => e.VertexA == edge.VertexB && e.VertexB == startingPoint);
                    if (possibleEdge != null)
                    {
                        distanceSum += possibleEdge.Cost;
                        break;
                    }
                }
            }

            if (!AllEdgesVisited(_edges))
            {
                distanceSum *= 1000;
            }

            _edges.ForEach(e => e.Visited = false);

            return 1.0 / distanceSum;
        }

        public static bool AllEdgesVisited(List<Edge> edges)
        {
            return edges.All(e => e.Visited);
        }
    }
}