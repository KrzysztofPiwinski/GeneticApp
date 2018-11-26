using GeneticApp.Models;
using GeneticSharp.Domain;
using GeneticSharp.Domain.Crossovers;
using GeneticSharp.Domain.Mutations;
using GeneticSharp.Domain.Populations;
using GeneticSharp.Domain.Selections;
using GeneticSharp.Domain.Terminations;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;

namespace GeneticApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void ChooseFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                textBox.Text = openFileDialog.FileName;
                resultBox.Text = "";
            }
        }

        private void submitButton_Click(object sender, RoutedEventArgs ev)
        {
            resultBox.Text = "";
            if (string.IsNullOrEmpty(textBox.Text))
            {
                resultBox.Text = "Wybierz plik z danymi...";
                return;
            }
            string[] lines = File.ReadAllLines(textBox.Text);
            int edgesNumber = lines.Count();
            List<Edge> edges = new List<Edge>();
            int edgeIndex = 0;
            string stringSeparator = "\t";
            foreach (string l in lines)
            {
                string[] values = l.Split(stringSeparator.ToCharArray(), StringSplitOptions.None);
                int[] numericValues = values.Select(s => int.Parse(s)).ToArray();

                // Krawędź może być przechodzona w obu kierunkach
                edges.Add(new Edge(numericValues[0], numericValues[1], numericValues[2], edgeIndex));

                // Ddodawana jest także w odwróconej wersji
                edges.Add(new Edge(numericValues[1], numericValues[0], numericValues[2], edgeIndex));

                //Krawędź i jej odwrócona wersja mają ten sam indeks(dla łatwiejszego odnajdowania)
                edgeIndex++;
            }

            EliteSelection selection = new EliteSelection();
            ThreeParentCrossover crossover = new ThreeParentCrossover();
            TworsMutation mutation = new TworsMutation();
            Fitness fitness = new Fitness(edges);
            Chromosome chromosome = new Chromosome(4 * edgesNumber, edges);
            Population population = new Population(200, 400, chromosome);

            GeneticAlgorithm ga = new GeneticAlgorithm(population, fitness, selection, crossover, mutation)
            {
                Termination = new GenerationNumberTermination(400)
            };

            Stopwatch timer = new Stopwatch();
            timer.Start();
            ga.Start();
            timer.Stop();

            Chromosome bestChromosome = ga.BestChromosome as Chromosome;
            int currentEdgeIndex = Convert.ToInt32(bestChromosome.GetGene(0).Value, CultureInfo.InvariantCulture);
            Edge currentEdge = edges[currentEdgeIndex];
            int startVertex = currentEdge.VertexA;
            int totalCost = currentEdge.Cost;
            string verticesSequence = currentEdge.VertexA.ToString() + "-" + currentEdge.VertexB.ToString();
            string orirginalVerticesSequence= currentEdge.VertexA.ToString() + "-" + currentEdge.VertexB.ToString();

            Edge originalEdge;
            for (int i = 1; i < bestChromosome.Length; i++)
            {
                originalEdge = edges[Convert.ToInt32(bestChromosome.GetGene(i).Value, CultureInfo.InvariantCulture)];
                orirginalVerticesSequence += "-" + originalEdge.VertexB.ToString();
            }

                resultBox.Text += $"Funkcja dopasowania najlepszego rozwiązania wynosi: {bestChromosome.Fitness}\n";
            for (int i = 1; i < bestChromosome.Length; i++)
            {
                currentEdgeIndex = Convert.ToInt32(bestChromosome.GetGene(i).Value, CultureInfo.InvariantCulture);
                currentEdge = edges[currentEdgeIndex];
               

               if (currentEdge.Visited)
                {
                    int nextIndex = i + 1;
                    if (nextIndex == bestChromosome.Length)
                    {
                        nextIndex = 0;
                    }
                    int nextEdgeIndex = Convert.ToInt32(bestChromosome.GetGene(nextIndex).Value, CultureInfo.InvariantCulture);
                    if (currentEdge.Index == edges[nextEdgeIndex].Index)
                    {
                        if(!Fitness.AllEdgesVisited(edges)&& currentEdge.VertexB != startVertex)
                        {
                            i++;
                            continue;
                        }
                       
                    }          
                }
                currentEdge.Visited = true;
                edges.Find(e => e.VertexA == currentEdge.VertexB && e.VertexB == currentEdge.VertexA).Visited = true;
                totalCost += currentEdge.Cost;
                verticesSequence += "-" + currentEdge.VertexB.ToString();

                if (Fitness.AllEdgesVisited(edges))
                {
                    if (currentEdge.VertexB == startVertex)
                    {
                        break;
                    }

                    Edge possibleEdge = edges.Find(e => e.VertexA == currentEdge.VertexB && e.VertexB == startVertex);
                    if (possibleEdge != null)
                    {
                        totalCost += possibleEdge.Cost;
                        verticesSequence += "-" + possibleEdge.VertexB.ToString();
                        break;
                    }
                }
            }
            foreach (Edge e in edges)
            {
                e.Visited = false;
            }
            fitness.Evaluate(bestChromosome);
            resultBox.Text += $"Ścieżka: {verticesSequence}\n";
            resultBox.Text += $"Ocieżka: {orirginalVerticesSequence}\n";
            resultBox.Text += $"Koszt najlepszego rozwiązania: {totalCost}\n";
            resultBox.Text += $"Czas wykonania: {timer.Elapsed.ToString(@"hh\:mm\:ss\.ff")}\n";
        }
    }
}