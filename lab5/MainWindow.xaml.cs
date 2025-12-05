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
using System.Threading.Tasks;

namespace lab5;

public partial class MainWindow : Window
{
    private AntColonyOptimization? _aco;

    public MainWindow()
    {
        InitializeComponent();
    }

    private async void SolveButton_Click(object sender, RoutedEventArgs e)
    {
        SolveButton.IsEnabled = false;
        ClearButton.IsEnabled = false;
        StatusText.Text = "Розв'язання...";
        ProgressListBox.Items.Clear();
        BestDistanceText.Text = "Обчислення...";

        try
        {
            _aco = new AntColonyOptimization();
            
            var result = await Task.Run(() => _aco.Solve());

            BestDistanceText.Text = $"{result.BestDistance:F2}";
            StatusText.Text = "Завершено";

            if (result.IterationBestDistances != null)
            {
                for (int i = 0; i < result.IterationBestDistances.Count; i++)
                {
                    ProgressListBox.Items.Add($"Ітерація {i + 1}: {result.IterationBestDistances[i]:F2}");
                }
            }

            ProgressListBox.ScrollIntoView(ProgressListBox.Items[ProgressListBox.Items.Count - 1]);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Помилка: {ex.Message}", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            StatusText.Text = "Помилка";
        }
        finally
        {
            SolveButton.IsEnabled = true;
            ClearButton.IsEnabled = true;
        }
    }

    private void ClearButton_Click(object sender, RoutedEventArgs e)
    {
        ProgressListBox.Items.Clear();
        BestDistanceText.Text = "Очікування...";
        StatusText.Text = "Готово";
    }
}
