using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace lab3;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private GameLogic _game;
    private DotsAndBoxesAI _ai;
    private const int DOT_RADIUS = 6;
    private const int CELL_SIZE = 80;
    private const int MARGIN = 40;
    private bool isComputerThinking = false;
    private Stack<(int, int, bool)> moveStack = new();

    public MainWindow()
    {
        InitializeComponent();
        _game = new GameLogic(GameLogic.Difficulty.Medium);
        _ai = new DotsAndBoxesAI(GameLogic.Difficulty.Medium);
        DrawBoard();
    }

    private void DrawBoard()
    {
        GameCanvas.Children.Clear();

        int gridSize = _game.GridSize;
        int canvasWidth = (int)GameCanvas.ActualWidth;
        int canvasHeight = (int)GameCanvas.ActualHeight;

        // Calculate actual cell size based on canvas
        int actualCellSize = Math.Min(
            (canvasWidth - 2 * MARGIN) / (gridSize - 1),
            (canvasHeight - 2 * MARGIN) / (gridSize - 1)
        );

        // Ensure minimum cell size
        if (actualCellSize <= 0)
            actualCellSize = 80;

        // Draw boxes
        for (int row = 0; row < gridSize - 1; row++)
        {
            for (int col = 0; col < gridSize - 1; col++)
            {
                int x = MARGIN + col * actualCellSize;
                int y = MARGIN + row * actualCellSize;

                int owner = _game.GetBoxOwner(row, col);
                Color boxColor = owner switch
                {
                    1 => Color.FromArgb(100, 33, 150, 243), // Blue for human
                    2 => Color.FromArgb(100, 244, 67, 54), // Red for computer
                    _ => Colors.Transparent
                };

                Rectangle box = new Rectangle
                {
                    Width = actualCellSize,
                    Height = actualCellSize,
                    Fill = new SolidColorBrush(boxColor),
                    Stroke = new SolidColorBrush(Colors.LightGray),
                    StrokeThickness = 1
                };

                Canvas.SetLeft(box, x);
                Canvas.SetTop(box, y);
                GameCanvas.Children.Add(box);
            }
        }

        // Draw horizontal lines
        for (int row = 0; row < gridSize; row++)
        {
            for (int col = 0; col < gridSize - 1; col++)
            {
                int x = MARGIN + col * actualCellSize;
                int y = MARGIN + row * actualCellSize;
                int capturedRow = row;
                int capturedCol = col;

                Line line = new Line
                {
                    X1 = x,
                    Y1 = y,
                    X2 = x + actualCellSize,
                    Y2 = y,
                    Stroke =
                        new SolidColorBrush(_game.IsHorizontalLineFree(row, col) ? Colors.LightGray : Colors.Black),
                    StrokeThickness = _game.IsHorizontalLineFree(row, col) ? 3 : 5,
                    Tag = $"h_{row}_{col}"
                };

                if (_game.IsHorizontalLineFree(row, col))
                {
                    line.MouseEnter += (s, e) => line.Stroke = new SolidColorBrush(Colors.Blue);
                    line.MouseLeave += (s, e) => line.Stroke = new SolidColorBrush(Colors.LightGray);
                    line.MouseLeftButtonDown += (s, e) => OnLineClicked(capturedRow, capturedCol, true);
                    line.Cursor = Cursors.Hand;
                }

                GameCanvas.Children.Add(line);
            }
        }

        // Draw vertical lines
        for (int row = 0; row < gridSize - 1; row++)
        {
            for (int col = 0; col < gridSize; col++)
            {
                int x = MARGIN + col * actualCellSize;
                int y = MARGIN + row * actualCellSize;
                int capturedRow = row;
                int capturedCol = col;

                Line line = new Line
                {
                    X1 = x,
                    Y1 = y,
                    X2 = x,
                    Y2 = y + actualCellSize,
                    Stroke = new SolidColorBrush(_game.IsVerticalLineFree(row, col) ? Colors.LightGray : Colors.Black),
                    StrokeThickness = _game.IsVerticalLineFree(row, col) ? 3 : 5,
                    Tag = $"v_{row}_{col}"
                };

                if (_game.IsVerticalLineFree(row, col))
                {
                    line.MouseEnter += (s, e) => line.Stroke = new SolidColorBrush(Colors.Blue);
                    line.MouseLeave += (s, e) => line.Stroke = new SolidColorBrush(Colors.LightGray);
                    line.MouseLeftButtonDown += (s, e) => OnLineClicked(capturedRow, capturedCol, false);
                    line.Cursor = Cursors.Hand;
                }

                GameCanvas.Children.Add(line);
            }
        }

        // Draw dots
        for (int row = 0; row < gridSize; row++)
        {
            for (int col = 0; col < gridSize; col++)
            {
                int x = MARGIN + col * actualCellSize;
                int y = MARGIN + row * actualCellSize;

                Ellipse dot = new Ellipse
                {
                    Width = DOT_RADIUS * 2,
                    Height = DOT_RADIUS * 2,
                    Fill = new SolidColorBrush(Colors.Black)
                };

                Canvas.SetLeft(dot, x - DOT_RADIUS);
                Canvas.SetTop(dot, y - DOT_RADIUS);
                GameCanvas.Children.Add(dot);
            }
        }

        UpdateUI();
    }

    private void OnLineClicked(int row, int col, bool isHorizontal)
    {
        if (isComputerThinking || _game.CurrentPlayer != GameLogic.Player.Human || _game.IsGameOver())
            return;

        if (!MakeMove(row, col, isHorizontal))
            return;

        moveStack.Push((row, col, isHorizontal));
        DrawBoard();
        if (_game.IsGameOver())
        {
            ShowGameOverMessage();
            return;
        }

        // If human didn't complete a box, it's computer's turn
        if (_game.CurrentPlayer == GameLogic.Player.Computer)
        {
            isComputerThinking = true;
            StatusText.Text = "Computer is thinking...";
            Dispatcher.InvokeAsync(async () =>
            {
                await Task.Delay(500); // Small delay for better UX
                ComputerMove();
                isComputerThinking = false;
            });
        }
    }

    private bool MakeMove(int row, int col, bool isHorizontal)
    {
        if (isHorizontal)
        {
            if (!_game.IsHorizontalLineFree(row, col))
                return false;
            _game.PlaceHorizontalLine(row, col);
        }
        else
        {
            if (!_game.IsVerticalLineFree(row, col))
                return false;
            _game.PlaceVerticalLine(row, col);
        }

        var completedBoxes = _game.CheckCompletedBoxes();

        if (completedBoxes.Count == 0)
        {
            _game.SwitchPlayer();
        }

        return true;
    }

    private void ComputerMove()
    {
        if (_game.IsGameOver())
        {
            ShowGameOverMessage();
            return;
        }
        
        var move = _ai.FindBestMove(_game);
        MakeMove(move.Item1, move.Item2, move.Item3);
        moveStack.Push((move.Item1, move.Item2, move.Item3));
        DrawBoard();
        
        if (_game.CurrentPlayer == GameLogic.Player.Computer)
        {
            Dispatcher.InvokeAsync(async () =>
            {
                await Task.Delay(300);
                ComputerMove();
            });
        }
    }

    private void ShowGameOverMessage()
    {
        string message;
        if (_game.HumanScore > _game.ComputerScore)
            message = $"You won! {_game.HumanScore} - {_game.ComputerScore}";
        else if (_game.ComputerScore > _game.HumanScore)
            message = $"Computer won! {_game.ComputerScore} - {_game.HumanScore}";
        else
            message = $"It's a tie! {_game.HumanScore} - {_game.ComputerScore}";

        StatusText.Text = message;
        MessageBox.Show(message, "Game Over", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    private void UpdateUI()
    {
        HumanScoreText.Text = _game.HumanScore.ToString();
        ComputerScoreText.Text = _game.ComputerScore.ToString();
        CurrentPlayerText.Text = _game.CurrentPlayer == GameLogic.Player.Human ? "Your Turn" : "Computer's Turn";

        if (!_game.IsGameOver())
        {
            StatusText.Text = _game.CurrentPlayer == GameLogic.Player.Human
                ? "Your turn - click a line"
                : "Computer is thinking...";
        }
    }

    private void NewGameClick(object sender, RoutedEventArgs e)
    {
        _game.ResetGame();
        moveStack.Clear();
        isComputerThinking = false;
        DrawBoard();
        StatusText.Text = "New game started!";
    }

    private void DifficultyChanged(object sender, RoutedEventArgs e)
    {
        if (sender is RadioButton rb && rb.Tag is string difficulty)
        {
            var newDifficulty = difficulty switch
            {
                "Easy" => GameLogic.Difficulty.Easy,
                "Hard" => GameLogic.Difficulty.Hard,
                _ => GameLogic.Difficulty.Medium
            };

            _game.SetDifficulty(newDifficulty);
            _ai.SetDifficulty(newDifficulty);
            StatusText.Text = $"Difficulty changed to {difficulty}";
        }
    }

    protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
    {
        base.OnRenderSizeChanged(sizeInfo);
        if (GameCanvas.ActualWidth > 0 && GameCanvas.ActualHeight > 0)
        {
            DrawBoard();
        }
    }
}