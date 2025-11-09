using System.Text;

namespace lab2;

public class SearchStatistics(string name)
{
    public string AlgorithmName { get; } = name;
    public bool SolutionFound { get; set; } = false;
    public int[]? SolutionBoard { get; set; }

    public long Iterations { get; set; } = 0; 
    public long DeadEnds { get; set; } = 0;
    public long GeneratedNodes { get; set; } = 0; 
    public long MaxNodesInMomory { get; set; } = 0;

    public void Print()
    {
        Console.WriteLine($"Statistics for: {AlgorithmName}");
        if (SolutionFound && SolutionBoard != null)
        {
            Console.WriteLine("Solution has been found:\n");
            Console.WriteLine(FormatBoard(SolutionBoard));
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

    private static string FormatBoard(int[] board)
    {
        int size = board.Length;
        var sb = new StringBuilder();

        sb.Append("  "); 
        for (int col = 0; col < size; col++)
        {
            sb.Append($" {col} "); 
        }
        sb.Append('\n'); 

        for (int row = 0; row < size; row++)
        {
            sb.Append(row.ToString().PadRight(2));

            for (int col = 0; col < size; col++)
            {
                sb.Append(board[col] == row ? " Q " : " . ");
            }
            sb.Append('\n'); 
        }

        return sb.ToString();
    }
}