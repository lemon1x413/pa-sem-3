namespace lab2;

public static class RbfsSolver
{
    private static SearchStatistics? _currentRbfsStats;

    private class RbfsNode
    {
        public int[] Board { get; }
        public int Depth { get; } 
        public int H { get; } 
        public int F { get; set; } 

        public RbfsNode(int[] board, int depth, Func<int[], int, int> hFunc)
        {
            Board = board;
            Depth = depth;
            H = hFunc(board, depth);
            F = Depth + H;
        }
    }

    private record RbfsResult(RbfsNode? Solution, int NewFLimit);

    public static SearchStatistics Run(int size, Func<int[], int, int> heuristic, string heuristicName)
    {
        _currentRbfsStats = new SearchStatistics($"RBFS (h={heuristicName})");

        var initialBoard = new int[size];
        Array.Fill(initialBoard, -1);
        var rootNode = new RbfsNode(initialBoard, 0, heuristic);
        _currentRbfsStats.GeneratedNodes++;

        var result = RecursiveSearch(rootNode, int.MaxValue, heuristic, size);

        if (result.Solution != null)
        {
            _currentRbfsStats.SolutionFound = true;
            _currentRbfsStats.SolutionBoard = result.Solution.Board;
        }

        _currentRbfsStats.MaxNodesInMomory = size * size; 

        return _currentRbfsStats;
    }

    private static RbfsResult RecursiveSearch(RbfsNode node, int fLimit, Func<int[], int, int> hFunc, int size)
    {
        _currentRbfsStats!.Iterations++;

        if (node.Depth == size) 
        {
            if (node.H == 0) 
            {
                return new RbfsResult(node, 0);
            }
            else
            {
                _currentRbfsStats.DeadEnds++;
                return new RbfsResult(null, int.MaxValue); 
            }
        }

        var successors = new List<RbfsNode>();
        int nextCol = node.Depth;
        for (int row = 0; row < size; row++) 
        {
            int[] newBoard = (int[])node.Board.Clone();
            newBoard[nextCol] = row;
            var child = new RbfsNode(newBoard, node.Depth + 1, hFunc);

            child.F = Math.Max(child.F, node.F);

            successors.Add(child);
            _currentRbfsStats.GeneratedNodes++;
        }

        if (successors.Count == 0)
        {
            return new RbfsResult(null, int.MaxValue);
        }

        while (true)
        {
            successors.Sort((a, b) => a.F.CompareTo(b.F));

            var best = successors[0];

            if (best.F > fLimit)
            {
                return new RbfsResult(null, best.F);
            }

            int alternativeF = successors.Count > 1 ? successors[1].F : int.MaxValue;

            var result = RecursiveSearch(best, Math.Min(fLimit, alternativeF), hFunc, size);

            if (result.Solution != null)
            {
                return result;
            }

            best.F = result.NewFLimit;
        }
    }
}