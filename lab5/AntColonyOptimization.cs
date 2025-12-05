using System;
using System.Collections.Generic;
using System.Linq;

namespace lab5;

public class AntColonyOptimization
{
    private const int NumVertices = 250;
    private const int NumAnts = 45;
    private const int WildAnts = 10;
    private const double Alpha = 4.0;
    private const double Beta = 2.0;
    private const double Rho = 0.3;
    private const int MaxIterations = 100;
    private readonly double[,]? _distances;
    private readonly double[,]? _pheromones;
    private readonly Random? _random;
    
    public class Result
    {
        public int[]? BestPath { get; set; }
        public double BestDistance { get; set; }
        public List<double>? IterationBestDistances { get; set; }
    }

    public AntColonyOptimization()
    {
        _random = new Random();
        _distances = new double[NumVertices, NumVertices];
        _pheromones = new double[NumVertices, NumVertices];
        InitializeDistances();
        InitializePheromones();
    }

    private void InitializeDistances()
    {
        for (int i = 0; i < NumVertices; i++)
        {
            for (int j = i + 1; j < NumVertices; j++)
            {
                double distance = _random!.Next(1, 41);
                _distances![i, j] = distance;
                _distances[j, i] = distance;
            }
            _distances![i, i] = 0;
        }
    }

    private void InitializePheromones()
    {
        double initialPheromone = 1.0 / (NumVertices * GetGreedyDistance());
        
        for (int i = 0; i < NumVertices; i++)
        {
            for (int j = 0; j < NumVertices; j++)
            {
                _pheromones![i, j] = initialPheromone;
            }
        }
    }

    private double GetGreedyDistance()
    {
        var path = new int[NumVertices];
        var visited = new bool[NumVertices];
        
        path[0] = 0;
        visited[0] = true;
        
        for (int i = 1; i < NumVertices; i++)
        {
            int lastCity = path[i - 1];
            int nearestCity = -1;
            double minDistance = double.MaxValue;
            
            for (int j = 0; j < NumVertices; j++)
            {
                if (!visited[j] && _distances![lastCity, j] < minDistance)
                {
                    minDistance = _distances[lastCity, j];
                    nearestCity = j;
                }
            }
            
            if (nearestCity == -1)
                nearestCity = 0;
            path[i] = nearestCity;
            visited[nearestCity] = true;
        }
        
        return CalculatePathDistance(path);
    }

    public Result Solve()
    {
        int[]? bestPath = null;
        double bestDistance = double.MaxValue;
        var iterationBestDistances = new List<double>();
        
        for (int iteration = 0; iteration < MaxIterations; iteration++)
        {
            var antPaths = new List<int[]>();
            var antDistances = new List<double>();
            
            for (int ant = 0; ant < NumAnts; ant++)
            {
                int[] path;
                
                if (ant < WildAnts)
                {
                    path = GenerateRandomPath();
                }
                else
                {
                    path = GenerateAntPath();
                }
                
                double distance = CalculatePathDistance(path);
                antPaths.Add(path);
                antDistances.Add(distance);
                
                if (distance < bestDistance)
                {
                    bestDistance = distance;
                    bestPath = (int[])path.Clone();
                }
            }
            
            UpdatePheromones(antPaths, antDistances);
            iterationBestDistances.Add(bestDistance);
        }
        
        return new Result
        {
            BestPath = bestPath,
            BestDistance = bestDistance,
            IterationBestDistances = iterationBestDistances
        };
    }

    private int[] GenerateRandomPath()
    {
        int[] path = new int[NumVertices];
        for (int i = 0; i < NumVertices; i++)
        {
            path[i] = i;
        }
        
        for (int i = NumVertices - 1; i > 0; i--)
        {
            int randomIndex = _random!.Next(i + 1);
            (path[i], path[randomIndex]) = (path[randomIndex], path[i]);
        }
        
        return path;
    }

    private int[] GenerateAntPath()
    {
        int[] path = new int[NumVertices];
        bool[] visited = new bool[NumVertices];
        
        int startCity = _random!.Next(NumVertices);
        path[0] = startCity;
        visited[startCity] = true;
        
        for (int step = 1; step < NumVertices; step++)
        {
            int currentCity = path[step - 1];
            int nextCity = SelectNextCity(currentCity, visited);
            path[step] = nextCity;
            visited[nextCity] = true;
        }
        
        return path;
    }

    private int SelectNextCity(int currentCity, bool[] visited)
    {
        double[] probabilities = new double[NumVertices];
        double sum = 0.0;
        
        for (int j = 0; j < NumVertices; j++)
        {
            if (!visited[j])
            {
                double pheromone = Math.Pow(_pheromones![currentCity, j], Alpha);
                double visibility = Math.Pow(1.0 / _distances![currentCity, j], Beta);
                probabilities[j] = pheromone * visibility;
                sum += probabilities[j];
            }
        }
        
        if (sum == 0)
        {
            for (int j = 0; j < NumVertices; j++)
            {
                if (!visited[j])
                    return j;
            }
        }
        
        double random = _random!.NextDouble() * sum;
        double cumulative = 0.0;
        
        for (int j = 0; j < NumVertices; j++)
        {
            if (!visited[j])
            {
                cumulative += probabilities[j];
                if (random <= cumulative)
                    return j;
            }
        }
        
        for (int j = 0; j < NumVertices; j++)
        {
            if (!visited[j])
                return j;
        }
        
        return 0;
    }

    private double CalculatePathDistance(int[] path)
    {
        double distance = 0.0;
        for (int i = 0; i < path.Length - 1; i++)
        {
            distance += _distances![path[i], path[i + 1]];
        }
        distance += _distances![path[path.Length - 1], path[0]];
        return distance;
    }

    private void UpdatePheromones(List<int[]> antPaths, List<double> antDistances)
    {
        for (int i = 0; i < NumVertices; i++)
        {
            for (int j = 0; j < NumVertices; j++)
            {
                _pheromones![i, j] *= (1.0 - Rho);
            }
        }
        
        for (int ant = 0; ant < antPaths.Count; ant++)
        {
            double pheromoneDeposit = 1.0 / antDistances[ant];
            int[] path = antPaths[ant];
            
            for (int i = 0; i < path.Length - 1; i++)
            {
                _pheromones![path[i], path[i + 1]] += pheromoneDeposit;
                _pheromones[path[i + 1], path[i]] += pheromoneDeposit;
            }
            
            _pheromones![path[path.Length - 1], path[0]] += pheromoneDeposit;
            _pheromones[path[0], path[path.Length - 1]] += pheromoneDeposit;
        }
    }
}
