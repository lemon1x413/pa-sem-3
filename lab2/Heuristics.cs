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
        int conflicts = 0;
        
        for (int i = 0; i < depth; i++)
        {
            if (board[i] == -1) continue;
            
            for (int j = i + 1; j < depth; j++)
            {
                if (board[j] == -1) continue;
                
                if (board[i] == board[j])
                {
                    conflicts += 2; 
                }
                
                int rowDiff = Math.Abs(i - j);
                int colDiff = Math.Abs(board[i] - board[j]);
                
                if (rowDiff == colDiff)
                {
                    conflicts++;
                }
            }
        }
        
        int emptyColumns = 0;
        for (int i = 0; i < depth; i++)
        {
            if (board[i] == -1)
            {
                emptyColumns++;
            }
        }
        
        return conflicts + emptyColumns * 2;
    }
}