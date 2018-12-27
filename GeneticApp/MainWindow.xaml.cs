﻿using GeneticApp.Models;
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

            TournamentSelection selection = new TournamentSelection();
            ThreeParentCrossover crossover = new ThreeParentCrossover();
            DisplacementMutation mutation = new DisplacementMutation();
            Fitness fitness = new Fitness(edges);
            Chromosome chromosome = new Chromosome(5 * edgesNumber, edges);
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
            resultBox.Text += $"Funkcja dopasowania najlepszego rozwiązania wynosi: {bestChromosome.Fitness}\n";
            resultBox.Text += $"Ścieżka: {bestChromosome.finalPath}\n";
            resultBox.Text += $"Koszt najlepszego rozwiązania: {1/bestChromosome.Fitness}\n";
            resultBox.Text += $"Czas wykonania: {timer.Elapsed.ToString(@"hh\:mm\:ss\.ff")}\n";
        }
    }
}