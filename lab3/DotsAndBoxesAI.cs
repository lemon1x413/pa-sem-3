namespace lab3;

public class DotsAndBoxesAI
{
    private GameLogic.Difficulty difficulty;
    private int maxDepth;
    private Random random = new Random();

    public DotsAndBoxesAI(GameLogic.Difficulty difficulty)
    {
        SetDifficulty(difficulty);
    }

    public (int, int, bool) FindBestMove(GameLogic game)
    {
        var availableMoves = game.GetAvailableMoves();

        if (availableMoves.Count == 0)
            throw new InvalidOperationException("No available moves");

        if (difficulty == GameLogic.Difficulty.Easy)
        {
            if (random.Next(100) < 40) 
            {
                return availableMoves[random.Next(availableMoves.Count)];
            }
        }

        var bestMove = availableMoves[0];
        int bestScore = int.MinValue;

        foreach (var move in availableMoves)
        {
            var gameCopy = game.Clone();
            bool completedBox = gameCopy.ApplyMove(move.Item1, move.Item2, move.Item3);

            if (!completedBox)
            {
                gameCopy.SwitchPlayer();
            }

            int score = AlphaBeta(gameCopy, maxDepth - 1, int.MinValue, int.MaxValue, false);

            if (score > bestScore)
            {
                bestScore = score;
                bestMove = move;
            }
        }

        return bestMove;
    }

    private int AlphaBeta(GameLogic game, int depth, int alpha, int beta, bool isMaximizing)
    {
        if (game.IsGameOver() || depth == 0)
        {
            return Evaluate(game);
        }

        var availableMoves = game.GetAvailableMoves();

        if (availableMoves.Count == 0)
        {
            return Evaluate(game);
        }

        if (isMaximizing)
        {
            int maxEval = int.MinValue;

            foreach (var move in availableMoves)
            {
                var gameCopy = game.Clone();
                bool completedBox = gameCopy.ApplyMove(move.Item1, move.Item2, move.Item3);

                if (!completedBox)
                {
                    gameCopy.SwitchPlayer();
                }

                int eval = AlphaBeta(gameCopy, depth - 1, alpha, beta, !isMaximizing);
                maxEval = Math.Max(maxEval, eval);
                alpha = Math.Max(alpha, eval);

                if (beta <= alpha)
                    break; 
            }

            return maxEval;
        }
        else
        {
            int minEval = int.MaxValue;

            foreach (var move in availableMoves)
            {
                var gameCopy = game.Clone();
                bool completedBox = gameCopy.ApplyMove(move.Item1, move.Item2, move.Item3);

                if (!completedBox)
                {
                    gameCopy.SwitchPlayer();
                }

                int eval = AlphaBeta(gameCopy, depth - 1, alpha, beta, !isMaximizing);
                minEval = Math.Min(minEval, eval);
                beta = Math.Min(beta, eval);

                if (beta <= alpha)
                    break; 
            }

            return minEval;
        }
    }

    private int Evaluate(GameLogic game)
    {
        int scoreDiff = game.ComputerScore - game.HumanScore;

        if (game.IsGameOver())
        {
            if (game.ComputerScore > game.HumanScore)
                return 1000 + scoreDiff * 100;
            else if (game.ComputerScore < game.HumanScore)
                return -1000 + scoreDiff * 100;
            else
                return 0;
        }

        int computerPotential = CountPotentialBoxes(game, GameLogic.Player.Computer);
        int humanPotential = CountPotentialBoxes(game, GameLogic.Player.Human);

        return scoreDiff * 10 + (computerPotential - humanPotential) * 5;
    }

    private int CountPotentialBoxes(GameLogic game, GameLogic.Player player)
    {
        int count = 0;
        int playerValue = player == GameLogic.Player.Computer ? 2 : 1;

        for (int row = 0; row < game.GridSize - 1; row++)
        {
            for (int col = 0; col < game.GridSize - 1; col++)
            {
                if (game.GetBoxOwner(row, col) != 0)
                    continue; 

                int sidesCompleted = 0;

                if (!game.IsHorizontalLineFree(row, col))
                    sidesCompleted++;

                if (!game.IsHorizontalLineFree(row + 1, col))
                    sidesCompleted++;

                if (!game.IsVerticalLineFree(row, col))
                    sidesCompleted++;

                if (!game.IsVerticalLineFree(row, col + 1))
                    sidesCompleted++;

                if (sidesCompleted == 3)
                    count++;
            }
        }

        return count;
    }

    public void SetDifficulty(GameLogic.Difficulty newDifficulty)
    {
        difficulty = newDifficulty;
        maxDepth = difficulty switch
        {
            GameLogic.Difficulty.Easy => 2,
            GameLogic.Difficulty.Medium => 4,
            GameLogic.Difficulty.Hard => 6,
            _ => 4
        };
    }
}
