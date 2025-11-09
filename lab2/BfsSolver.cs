namespace lab2;

public static class BfsSolver
{
    private class BfsNode
    {
        public int[] Board { get; }
        public int Depth { get; }

        public BfsNode(int[] board, int depth)
        {
            Board = board;
            Depth = depth;
        }
    }

    public static SearchStatistics Run(int size)
    {
        var stats = new SearchStatistics("BFS");
        var frontier = new Queue<BfsNode>();

        var initialBoard = new int[size];
        Array.Fill(initialBoard, -1);
        frontier.Enqueue(new BfsNode(initialBoard, 0));
        stats.GeneratedNodes++;

        while (frontier.Count > 0)
        {
            stats.Iterations++;
            stats.MaxNodesInMomory = Math.Max(stats.MaxNodesInMomory, frontier.Count);

            var currentNode = frontier.Dequeue();

            if (currentNode.Depth == size)
            {
                if (Heuristics.F2(currentNode.Board, size) == 0)
                {
                    stats.SolutionFound = true;
                    stats.SolutionBoard = currentNode.Board;
                    return stats;
                }
                else
                {
                    stats.DeadEnds++;
                    continue;
                }
            }

            int nextCol = currentNode.Depth;
            for (int row = 0; row < size; row++)
            {
                int[] newBoard = (int[])currentNode.Board.Clone();
                newBoard[nextCol] = row;
                var childNode = new BfsNode(newBoard, currentNode.Depth + 1);
                frontier.Enqueue(childNode);
                stats.GeneratedNodes++;
            }
        }

        return stats;
    }
}