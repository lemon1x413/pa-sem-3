namespace lab2;

public class SearchStatistics
{
    public string AlgorithmName { get; }
    public bool SolutionFound { get; set; } = false;
    public int[]? SolutionBoard { get; set; }

    public long Iterations { get; set; } = 0; 
    public long DeadEnds { get; set; } = 0;
    public long GeneratedNodes { get; set; } = 0; 
    public long MaxNodesInMomory { get; set; } = 0;

    public SearchStatistics(string name)
    {
        AlgorithmName = name;
    }

    public void Print()
    {
        Console.WriteLine($"Statistics for: {AlgorithmName}");
        if (SolutionFound && SolutionBoard != null)
        {
            Console.WriteLine("Solution has been found.");
            Console.WriteLine($"Board: [{string.Join(", ", SolutionBoard)}]");
        }
        else
        {
            Console.WriteLine("Solution has not been found.");
        }

        Console.WriteLine($"Iterations: {Iterations:N0}");
        Console.WriteLine($"Dead ends: {DeadEnds:N0}");
        Console.WriteLine($"Generated nodes: {GeneratedNodes:N0}");
        Console.WriteLine($"Max nodes in memory: {MaxNodesInMomory:N0}");
    }
}