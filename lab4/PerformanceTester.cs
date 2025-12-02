using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace lab4;

public class PerformanceTester
{
    private readonly SparseIndexDatabase _database;
    private readonly Random _random;

    public PerformanceTester(SparseIndexDatabase database)
    {
        _database = database;
        _random = new Random();
    }

    public void FillWithRandomData(int count)
    {
        _database.Clear();
        var usedKeys = new HashSet<int>();

        for (int i = 0; i < count; i++)
        {
            int key;
            do
            {
                key = _random.Next(1, count * 10);
            } while (usedKeys.Contains(key));

            usedKeys.Add(key);
            string data = $"Data_{key}_{Guid.NewGuid().ToString().Substring(0, 8)}";
            _database.Add(new Record(key, data));
        }
    }

    public (double AverageComparisons, double AverageTime, int TotalSearches) PerformanceTest(int searchCount = 25)
    {
        var records = _database.GetAllRecords();
        if (records.Count == 0)
            return (0, 0, 0);

        long totalComparisons = 0;
        long totalTime = 0;
        int successfulSearches = 0;

        var stopwatch = new Stopwatch();

        for (int i = 0; i < searchCount; i++)
        {
            var randomRecord = records[_random.Next(records.Count)];

            stopwatch.Restart();
            var result = _database.Search(randomRecord.Key);
            stopwatch.Stop();

            if (result != null)
            {
                totalComparisons += _database.ComparisonCount;
                totalTime += stopwatch.ElapsedMilliseconds;
                successfulSearches++;
            }
        }

        double avgComparisons = successfulSearches > 0 ? (double)totalComparisons / successfulSearches : 0;
        double avgTime = successfulSearches > 0 ? (double)totalTime / successfulSearches : 0;

        return (avgComparisons, avgTime, successfulSearches);
    }

    public string GenerateReport()
    {
        var records = _database.GetAllRecords();
        var (avgComparisons, avgTime, searchCount) = PerformanceTest(25);

        var report = new System.Text.StringBuilder();
        report.AppendLine("=== DATABASE PERFORMANCE REPORT ===");
        report.AppendLine($"Total Records: {records.Count}");
        report.AppendLine($"Searches Performed: {searchCount}");
        report.AppendLine($"Average Comparisons per Search: {avgComparisons:F2}");
        report.AppendLine($"Average Time per Search: {avgTime:F3} ms");
        report.AppendLine();
        report.AppendLine("TIME COMPLEXITY ANALYSIS:");
        report.AppendLine("- Index Binary Search: O(log m) where m = number of index entries");
        report.AppendLine("- Block Linear Search: O(k) where k = block size");
        report.AppendLine("- Overflow Area Search: O(n) where n = overflow records");
        report.AppendLine("- Overall Search: O(log m + k) average case");
        report.AppendLine();
        report.AppendLine("EMPIRICAL RESULTS:");
        report.AppendLine($"- Theoretical minimum comparisons: {Math.Log2(records.Count):F2}");
        report.AppendLine($"- Actual average comparisons: {avgComparisons:F2}");
        report.AppendLine($"- Efficiency ratio: {(Math.Log2(records.Count) / avgComparisons):F2}");

        return report.ToString();
    }
}
