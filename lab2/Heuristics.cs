namespace lab2;

public static class Heuristics
{
    public static int F2(int[] board, int depth)
    {
        int conflicts = 0;
        if (depth <= 1) return 0;

        for (int i = 0; i < depth; i++)
        {
            for (int j = i + 1; j < depth; j++)
            {
                if (board[i] == board[j])
                {
                    conflicts++;
                }
                else if (Math.Abs(board[i] - board[j]) == (j - i))
                {
                    conflicts++;
                }
            }
        }

        return conflicts;
    }

    public static int H_new(int[] board, int depth)
    {
        int[] oneSolution = { 4, 2, 0, 6, 1, 7, 5, 3 };

        int misplaced = 0;
        for (int i = 0; i < depth; i++)
        {
            if (board[i] != oneSolution[i])
            {
                misplaced++;
            }
        }

        return misplaced;
    }
}