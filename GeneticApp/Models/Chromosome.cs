using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Randomizations;
using System;
using System.Collections.Generic;

namespace GeneticApp.Models
{
    internal class Chromosome : ChromosomeBase
    {
        private readonly int edgesNumber;
        private readonly List<Edge> edges;
        public string finalPath;

        public Chromosome(int edgesQuantity, List<Edge> _edges) : base(edgesQuantity)
        {
            edgesNumber = edgesQuantity;
            edges = _edges;
            List<Edge> neighbours;
            int selectedEdgeIndex;
            int[] edgesIndexes = new int[edgesNumber];
            Random randomizationProvider = new Random();
            edgesIndexes[0] = randomizationProvider.Next(edgesNumber / 5);

            for (int i = 1; i < edgesNumber; i++)
            {
                neighbours = edges[edgesIndexes[i - 1]].GetNeighbours(edges);
                selectedEdgeIndex = randomizationProvider.Next(neighbours.Count);
                edgesIndexes[i] = edges.IndexOf(neighbours[selectedEdgeIndex]);
            }

            for (int i = 0; i < edgesNumber; i++)
            {
                ReplaceGene(i, new Gene(edgesIndexes[i]));
            }
        }

        public double Distance { get; internal set; }

        public override Gene GenerateGene(int geneIndex)
        {
            return new Gene(RandomizationProvider.Current.GetInt(0, edgesNumber));
        }

        public override IChromosome CreateNew()
        {
            return new Chromosome(edgesNumber, edges);
        }

        public override IChromosome Clone()
        {
            Chromosome clone = base.Clone() as Chromosome;
            clone.Distance = Distance;

            return clone;
        }
    }
}