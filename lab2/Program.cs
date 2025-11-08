using System.Diagnostics;

namespace lab2;

class Program
{
    private static readonly TimeSpan TimeLimit = TimeSpan.FromMinutes(30);
    private static readonly long MemoryLimitBytes = 1 * 1024 * 1024 * 1024; 

    private static CancellationTokenSource? _timerCts;
    private static bool _resourceLimitExceeded = false;
    private static string _resourceError = "";


    static void Main(string[] args)
    {
        System.Console.OutputEncoding = System.Text.Encoding.UTF8;
        const int BOARD_SIZE = 8;

        var bfsSolver = new BfsSolver(BOARD_SIZE);
        RunTest("1. BFS ", bfsSolver.Run);

        var rbfsSolverF2 = new RbfsSolver(BOARD_SIZE, Heuristics.F2, "F2");
        RunTest("2. RBFS (F2 heuristics) ", rbfsSolverF2.Run);

        var rbfsSolverHNew = new RbfsSolver(BOARD_SIZE, Heuristics.H_new, "H_new");
        RunTest("3. RBFS (H_new heuristics)", rbfsSolverHNew.Run);
    }
    
    static void RunTest(string testName, Func<SearchStatistics> solveAction)
    {
        Console.WriteLine($"\nTest start: {testName}");
        
        _resourceLimitExceeded = false;
        _resourceError = "";
        
        var stopwatch = new Stopwatch();
        SearchStatistics? stats = null;
        Exception? exception = null;

        _timerCts = new CancellationTokenSource(TimeLimit);

        using var timer = new Timer(
            callback: CheckResourceLimits,
            state: null,
            dueTime: TimeSpan.FromSeconds(1),
            period: TimeSpan.FromSeconds(1));

        try
        {
            stopwatch.Start();
            stats = solveAction();
            stopwatch.Stop();
        }
        catch (Exception ex)
        {
            if (_resourceLimitExceeded)
            {
                exception = new Exception(_resourceError, ex);
            }
            else
            {
                exception = ex;
            }
        }
        finally
        {
            stopwatch.Stop();
            timer.Change(Timeout.Infinite, Timeout.Infinite);
            _timerCts?.Cancel();
            _timerCts?.Dispose();
            _timerCts = null;
        }

        Console.WriteLine($"Test fineshed in: {stopwatch.ElapsedMilliseconds} мс");
        if (stats != null)
        {
            stats.Print();
        }
        else
        {
            Console.WriteLine($"Error: {exception?.Message}");
        }
    }
    
    private static void CheckResourceLimits(object? state)
    {
        if (_resourceLimitExceeded) return;
        if (_timerCts == null || _timerCts.IsCancellationRequested)
        {
            _resourceLimitExceeded = true;
            _resourceError = "30 minutes limit has been exceeded";
            return;
        }

        var currentMemory = GC.GetTotalMemory(false);
        if (currentMemory > MemoryLimitBytes)
        {
            _resourceLimitExceeded = true;
            _resourceError =
                $"Memory limit has been exceeded: {currentMemory / (1024.0 * 1024.0):F2} Мб.";
            return;
        }
    }
}