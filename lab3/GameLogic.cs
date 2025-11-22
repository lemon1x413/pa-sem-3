namespace lab3;

public class GameLogic
{
    public enum Difficulty { Easy, Medium, Hard }
    public enum Player { Human, Computer }

    private const int GRID_SIZE = 5; 
    private bool[,]? horizontalLines;
    private bool[,]? verticalLines;
    private int[,]? boxes; 
    private Player currentPlayer;
    private int humanScore;
    private int computerScore;
    private Difficulty difficulty;
    private List<(int, int, bool)>? moveHistory;

    public int GridSize => GRID_SIZE;
    public int HumanScore => humanScore;
    public int ComputerScore => computerScore;
    public Player CurrentPlayer => currentPlayer;
    public Difficulty CurrentDifficulty => difficulty;

    public GameLogic(Difficulty difficulty = Difficulty.Medium)
    {
        this.difficulty = difficulty;
        InitializeGame();
    }

    private void InitializeGame()
    {
        horizontalLines = new bool[GRID_SIZE, GRID_SIZE - 1];
        verticalLines = new bool[GRID_SIZE - 1, GRID_SIZE];
        boxes = new int[GRID_SIZE - 1, GRID_SIZE - 1];
        currentPlayer = Player.Human;
        humanScore = 0;
        computerScore = 0;
        moveHistory = new List<(int, int, bool)>();
    }

    public void ResetGame()
    {
        InitializeGame();
    }

    public bool IsHorizontalLineFree(int row, int col)
    {
        if (row < 0 || row >= GRID_SIZE || col < 0 || col >= GRID_SIZE - 1)
            return false;
        return !horizontalLines[row, col];
    }

    public bool IsVerticalLineFree(int row, int col)
    {
        if (row < 0 || row >= GRID_SIZE - 1 || col < 0 || col >= GRID_SIZE)
            return false;
        return !verticalLines[row, col];
    }

    public bool PlaceHorizontalLine(int row, int col)
    {
        if (!IsHorizontalLineFree(row, col))
            return false;

        horizontalLines[row, col] = true;
        moveHistory.Add((row, col, true));
        return true;
    }

    public bool PlaceVerticalLine(int row, int col)
    {
        if (!IsVerticalLineFree(row, col))
            return false;

        verticalLines[row, col] = true;
        moveHistory.Add((row, col, false));
        return true;
    }
    
    public List<(int, int)> CheckCompletedBoxes()
    {
        var completedBoxes = new List<(int, int)>();

        for (int row = 0; row < GRID_SIZE - 1; row++)
        {
            for (int col = 0; col < GRID_SIZE - 1; col++)
            {
                if (boxes[row, col] == 0)
                {
                    bool top = horizontalLines[row, col];
                    bool bottom = horizontalLines[row + 1, col];
                    bool left = verticalLines[row, col];
                    bool right = verticalLines[row, col + 1];

                    if (top && bottom && left && right)
                    {
                        boxes[row, col] = currentPlayer == Player.Human ? 1 : 2;
                        completedBoxes.Add((row, col));

                        if (currentPlayer == Player.Human)
                            humanScore++;
                        else
                            computerScore++;
                    }
                }
            }
        }

        return completedBoxes;
    }

    public void SwitchPlayer()
    {
        currentPlayer = currentPlayer == Player.Human ? Player.Computer : Player.Human;
    }

    public int GetBoxOwner(int row, int col)
    {
        if (row < 0 || row >= GRID_SIZE - 1 || col < 0 || col >= GRID_SIZE - 1)
            return 0;
        return boxes[row, col];
    }

    public bool IsGameOver()
    {
        return humanScore + computerScore == (GRID_SIZE - 1) * (GRID_SIZE - 1);
    }

    public List<(int, int, bool)> GetAvailableMoves()
    {
        var moves = new List<(int, int, bool)>();

        for (int row = 0; row < GRID_SIZE; row++)
        {
            for (int col = 0; col < GRID_SIZE - 1; col++)
            {
                if (IsHorizontalLineFree(row, col))
                    moves.Add((row, col, true));
            }
        }

        for (int row = 0; row < GRID_SIZE - 1; row++)
        {
            for (int col = 0; col < GRID_SIZE; col++)
            {
                if (IsVerticalLineFree(row, col))
                    moves.Add((row, col, false));
            }
        }

        return moves;
    }

  
    public GameLogic Clone()
    {
        var clone = new GameLogic(difficulty);
        clone.horizontalLines = (bool[,])horizontalLines.Clone();
        clone.verticalLines = (bool[,])verticalLines.Clone();
        clone.boxes = (int[,])boxes.Clone();
        clone.currentPlayer = currentPlayer;
        clone.humanScore = humanScore;
        clone.computerScore = computerScore;
        clone.moveHistory = new List<(int, int, bool)>(moveHistory);
        return clone;
    }

 
    public bool ApplyMove(int row, int col, bool isHorizontal)
    {
        if (isHorizontal)
        {
            if (!PlaceHorizontalLine(row, col))
                return false;
        }
        else
        {
            if (!PlaceVerticalLine(row, col))
                return false;
        }

        var completedBoxes = CheckCompletedBoxes();
        return completedBoxes.Count > 0;
    }

    public void SetDifficulty(Difficulty newDifficulty)
    {
        difficulty = newDifficulty;
    }
}
