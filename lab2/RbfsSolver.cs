namespace lab2;

public class RbfsSolver
{
    private readonly int _boardSize;
    private readonly Func<int[], int, int> _hFunc;
    private readonly string _hName;
    private readonly SearchStatistics _stats;

    private int g(RbfsNode node) => node.Depth;

    private int h(RbfsNode node) => node.H;

    private int f(RbfsNode node) => node.F;

    public RbfsSolver(int size, Func<int[], int, int> heuristic, string heuristicName)
    {
        _boardSize = size;
        _hFunc = heuristic;
        _hName = heuristicName;
        _stats = new SearchStatistics($"RBFS (h={_hName})");
    }

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

    public SearchStatistics Run()
    {
        var initialBoard = new int[_boardSize];
        Array.Fill(initialBoard, -1);
        var rootNode = new RbfsNode(initialBoard, 0, _hFunc);
        _stats.GeneratedNodes++;

        var result = RecursiveSearch(rootNode, int.MaxValue);

        if (result.Solution != null)
        {
            _stats.SolutionFound = true;
            _stats.SolutionBoard = result.Solution.Board;
        }

        _stats.MaxNodesInMomory = _boardSize * _boardSize;

        return _stats;
    }

    private RbfsResult RecursiveSearch(RbfsNode node, int fLimit)
    {
        _stats.Iterations++;

        if (node.Depth == _boardSize)
        {
            if (node.H == 0)
            {
                return new RbfsResult(node, 0);
            }
            else
            {
                _stats.DeadEnds++;
                return new RbfsResult(null, int.MaxValue);
            }
        }

        var successors = new List<RbfsNode>();
        int nextCol = node.Depth;
        for (int row = 0; row < _boardSize; row++)
        {
            int[] newBoard = (int[])node.Board.Clone();
            newBoard[nextCol] = row;
            var child = new RbfsNode(newBoard, node.Depth + 1, _hFunc);

            child.F = Math.Max(child.F, node.F);

            successors.Add(child);
            _stats.GeneratedNodes++;
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

            int alternativeF = (successors.Count > 1) ? successors[1].F : int.MaxValue;

            var result = RecursiveSearch(best, Math.Min(fLimit, alternativeF));

            if (result.Solution != null)
            {
                return result;
            }

            best.F = result.NewFLimit;
        }
    }
}