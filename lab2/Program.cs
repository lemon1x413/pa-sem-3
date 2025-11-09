using System.Diagnostics;

namespace lab2;

class Program
{
    private static void Main(string[] args)
    {
        const int boardSize = 8;
        var stopwatch = new Stopwatch();

        Console.WriteLine("\nTest start for BFS");
        stopwatch.Restart();
        var bfsStats = BfsSolver.Run(boardSize);
        stopwatch.Stop();
        Console.WriteLine($"Test finished in: {stopwatch.ElapsedMilliseconds} ms");
        bfsStats.Print();
        
        Console.WriteLine("\n-----------------------------------------------------");
        
        Console.WriteLine("\nTest start for RBFS(F2)");
        stopwatch.Restart();
        var rbfsStatsF2 = RbfsSolver.Run(boardSize, Heuristics.F2, "F2");
        stopwatch.Stop();
        Console.WriteLine($"Test finished in: {stopwatch.ElapsedMilliseconds} ms");
        rbfsStatsF2.Print();
        
        Console.WriteLine("\n-----------------------------------------------------");
        
        Console.WriteLine("\nTest start for RBFS(H_new)");
        stopwatch.Restart();
        var rbfsStatsHNew = RbfsSolver.Run(boardSize, Heuristics.H_new, "H_new");
        stopwatch.Stop();
        Console.WriteLine($"Test finished in: {stopwatch.ElapsedMilliseconds} ms");
        rbfsStatsHNew.Print();
    }
}