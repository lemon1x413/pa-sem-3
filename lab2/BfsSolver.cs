namespace lab2;

public class BfsSolver(int size)
{
    private record Node(int[] Board, int Depth);

    public SearchStatistics Run()
    {
        var stats = new SearchStatistics("BFS");
        var queue = new Queue<Node>();

        var initialBoard = new int[size];
        Array.Fill(initialBoard, -1);
        queue.Enqueue(new Node(initialBoard, 0));
        stats.GeneratedNodes++;

        while (queue.Count > 0)
        {
            stats.Iterations++;
            stats.MaxNodesInMomory = Math.Max(stats.MaxNodesInMomory, queue.Count);

            var currentNode = queue.Dequeue();

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

                var childNode = new Node(newBoard, currentNode.Depth + 1);
                queue.Enqueue(childNode);
                stats.GeneratedNodes++;
            }
        }

        return stats;
    }
}