using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Fitnesses;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticApp.Models
{

    internal class Fitness : IFitness
    {

        public List<Edge> Edges { get; private set; }

        public Fitness(List<Edge> EdgesArg)
        {
            Edges = EdgesArg;
        }

        public double Evaluate(IChromosome chromosome)
        {
            var genes = chromosome.GetGenes();
            int startingPoint = Edges[Convert.ToInt32(genes[0].Value, CultureInfo.InvariantCulture)].VertexA; // węzeł początkowy
            var lastEdgeIndex = Convert.ToInt32(genes[0].Value, CultureInfo.InvariantCulture);
            Edge currentEdge = Edges[lastEdgeIndex]; 
            Chromosome cr = chromosome as Chromosome;
            cr.finalPath = currentEdge.VertexA.ToString() + "-" +currentEdge.VertexB.ToString();
            var distanceSum = (float)currentEdge.Cost;
            

            for (int i = 1; i < genes.Length; i++)
            {
                int edgeIndex = Convert.ToInt32(genes[i].Value, CultureInfo.InvariantCulture);
                Edge edge = Edges[edgeIndex];
                Edge reverseEdge = Edges.Find(e => e.VertexA == edge.VertexB && e.VertexB == edge.VertexA); // znalezienie odwrotej krawędzi
                if (edge.Visited)
                {
                    int nextIndex = i + 1;
                    if (nextIndex == genes.Length)
                    {
                        nextIndex = 0;
                    }
                    int nextEdgeIndex = Convert.ToInt32(genes[nextIndex].Value, CultureInfo.InvariantCulture);
                    if (edge.Index == Edges[nextEdgeIndex].Index)
                    {
                        if (!Fitness.AllEdgesVisited(Edges) && edge.VertexB != startingPoint)
                        {
                            i++;
                            continue;
                        }
                    }
                }
                edge.Visited = true; // krawędź została odwiedzona
                reverseEdge.Visited = true; // odwrotność krawędzi została odwiedzona

                if (i != 0 && edge.VertexA != Edges[lastEdgeIndex].VertexB)
                {
                    distanceSum =float.MaxValue; //jeśli ścieżka nie jest poprawna koszt jest znacząco zwiększany
                    lastEdgeIndex = edgeIndex;
                }
                else
                {
                    distanceSum += edge.Cost;
                    cr.finalPath += "-" + edge.VertexB.ToString();
                    lastEdgeIndex = edgeIndex;
                }
                if (AllEdgesVisited(Edges)) // sprawdzenie czy ścieżka jest zamknięta i czy wszystkie krawędzie zostały odwiedzone
                {
                    if (edge.VertexB == startingPoint)
                    {
                        break;
                    }
                    Edge possibleEdge = Edges.Find(e => e.VertexA == edge.VertexB && e.VertexB == startingPoint);
                    if (possibleEdge != null)
                    {
                        distanceSum += possibleEdge.Cost;
                        cr.finalPath += "-" + possibleEdge.VertexB.ToString();
                        break;
                    }

                }
            }
            if (!AllEdgesVisited(Edges))
            {
                distanceSum = float.MaxValue;
            }
            foreach (Edge e in Edges)
            {
                e.Visited = false;
            }

            var fitness = 1.0 / distanceSum;


            return fitness;

        }
        public static bool AllEdgesVisited(List<Edge> edges)
        {
            foreach (var e in edges)
            {
                if (e.Visited == false)
                {
                    return false;
                }
            }
            return true;
        }
    }
}