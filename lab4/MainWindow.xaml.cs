using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace lab4;

public partial class MainWindow : Window
{
    private SparseIndexDatabase _database;
    private DatabasePersistence _persistence;
    private PerformanceTester _tester;

    public MainWindow()
    {
        InitializeComponent();
        _database = new SparseIndexDatabase();
        _persistence = new DatabasePersistence("database.dat");
        _tester = new PerformanceTester(_database);

        try
        {
            if (_persistence.Exists)
            {
                _persistence.Load(_database);
                UpdateUI();
                SetStatus("Database loaded from file");
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error loading database: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void UpdateUI()
    {
        RecordsListBox.Items.Clear();
        var records = _database.GetAllRecords();
        foreach (var record in records)
        {
            RecordsListBox.Items.Add(record.ToString());
        }

        IndexListBox.Items.Clear();
        var indexEntries = _database.GetIndexEntries();
        foreach (var entry in indexEntries)
        {
            IndexListBox.Items.Add($"Key: {entry.Key}, Block: {entry.BlockIndex}");
        }

        TotalRecordsRun.Text = _database.RecordCount.ToString();
        IndexSizeRun.Text = indexEntries.Count.ToString();
        OverflowRun.Text = _database.GetOverflowRecords().Count.ToString();
    }

    private void SetStatus(string message)
    {
        StatusTextBlock.Text = message;
    }

    private void Search_Click(object sender, RoutedEventArgs e)
    {
        if (!int.TryParse(SearchKeyTextBox.Text, out int key))
        {
            MessageBox.Show("Please enter a valid integer key", "Input Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        var result = _database.Search(key);
        ComparisonsRun.Text = _database.ComparisonCount.ToString();

        if (result != null)
        {
            SearchResultTextBlock.Text = $"Found: {result}";
            SearchStatusRun.Text = "Found";
            SetStatus($"Search successful. Comparisons: {_database.ComparisonCount}");
        }
        else
        {
            SearchResultTextBlock.Text = $"Record with key {key} not found";
            SearchStatusRun.Text = "Not Found";
            SetStatus($"Record not found. Comparisons: {_database.ComparisonCount}");
        }
    }

    private void Add_Click(object sender, RoutedEventArgs e)
    {
        if (!int.TryParse(AddKeyTextBox.Text, out int key))
        {
            MessageBox.Show("Please enter a valid integer key", "Input Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        string data = AddDataTextBox.Text;
        if (string.IsNullOrWhiteSpace(data))
        {
            MessageBox.Show("Please enter data for the record", "Input Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        try
        {
            _database.Add(new Record(key, data));
            UpdateUI();
            AddKeyTextBox.Clear();
            AddDataTextBox.Clear();
            SetStatus($"Record with key {key} added successfully");
        }
        catch (InvalidOperationException ex)
        {
            MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void Edit_Click(object sender, RoutedEventArgs e)
    {
        if (!int.TryParse(EditKeyTextBox.Text, out int key))
        {
            MessageBox.Show("Please enter a valid integer key", "Input Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        string newData = EditDataTextBox.Text;
        if (string.IsNullOrWhiteSpace(newData))
        {
            MessageBox.Show("Please enter new data for the record", "Input Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (_database.Edit(key, newData))
        {
            UpdateUI();
            EditKeyTextBox.Clear();
            EditDataTextBox.Clear();
            SetStatus($"Record with key {key} updated successfully");
        }
        else
        {
            MessageBox.Show($"Record with key {key} not found", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void Delete_Click(object sender, RoutedEventArgs e)
    {
        if (!int.TryParse(DeleteKeyTextBox.Text, out int key))
        {
            MessageBox.Show("Please enter a valid integer key", "Input Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (_database.Delete(key))
        {
            UpdateUI();
            DeleteKeyTextBox.Clear();
            SetStatus($"Record with key {key} deleted successfully");
        }
        else
        {
            MessageBox.Show($"Record with key {key} not found", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void NewDatabase_Click(object sender, RoutedEventArgs e)
    {
        var result = MessageBox.Show("Create a new database? This will clear all current data.", "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Question);
        if (result == MessageBoxResult.Yes)
        {
            _database.Clear();
            UpdateUI();
            PerformanceReportTextBox.Clear();
            SetStatus("New database created");
        }
    }

    private void SaveDatabase_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            _persistence.Save(_database);
            SetStatus("Database saved successfully");
            MessageBox.Show("Database saved to file", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error saving database: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void LoadDatabase_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            _persistence.Load(_database);
            UpdateUI();
            SetStatus("Database loaded from file");
            MessageBox.Show("Database loaded successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error loading database: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void PerformanceTest_Click(object sender, RoutedEventArgs e)
    {
        var result = MessageBox.Show("Fill database with 10,000 random records and run performance test? This will take a moment.", "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Question);
        if (result != MessageBoxResult.Yes)
            return;

        try
        {
            SetStatus("Filling database with random data...");
            this.Dispatcher.Invoke(() => { }, System.Windows.Threading.DispatcherPriority.Background);

            _tester.FillWithRandomData(10000);
            UpdateUI();

            SetStatus("Running performance test...");
            this.Dispatcher.Invoke(() => { }, System.Windows.Threading.DispatcherPriority.Background);

            var report = _tester.GenerateReport();
            PerformanceReportTextBox.Text = report;

            SetStatus("Performance test completed");
            MessageBox.Show("Performance test completed. See report on the right panel.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error during performance test: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            SetStatus("Performance test failed");
        }
    }

    private void ClearAll_Click(object sender, RoutedEventArgs e)
    {
        var result = MessageBox.Show("Clear all data? This cannot be undone.", "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Question);
        if (result == MessageBoxResult.Yes)
        {
            _database.Clear();
            UpdateUI();
            PerformanceReportTextBox.Clear();
            SetStatus("All data cleared");
        }
    }
}
