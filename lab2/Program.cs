using System.Diagnostics;

namespace lab2;

class Program
{
    private static void Main(string[] args)
    {
        const int boardSize = 8;

        var bfsSolver = new BfsSolver(boardSize);
        RunTest("1. BFS ", bfsSolver.Run);

        var rbfsSolverF2 = new RbfsSolver(boardSize, Heuristics.F2, "F2");
        RunTest("2. RBFS (F2 heuristics) ", rbfsSolverF2.Run);

        var rbfsSolverHNew = new RbfsSolver(boardSize, Heuristics.H_new, "H_new");
        RunTest("3. RBFS (H_new heuristics)", rbfsSolverHNew.Run);
    }

    private static void RunTest(string testName, Func<SearchStatistics> solveAction)
    {
        Console.WriteLine($"\nTest start: {testName}");

        var stopwatch = new Stopwatch();

        stopwatch.Start();
        var stats = solveAction();
        stopwatch.Stop();

        Console.WriteLine($"Test finished in: {stopwatch.ElapsedMilliseconds} ms");

        stats.Print();
    }
}