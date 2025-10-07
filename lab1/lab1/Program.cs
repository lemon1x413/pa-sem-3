using System.Buffers;
using System.Data;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace lab1;

class Program
{
    static void Main(string[] args)
    {
        var path = "../../../files/fileA.txt";
        
        var timer = new Stopwatch();
        timer.Start();
        GenerateFile(path, 45000000);
        timer.Stop();
        Console.WriteLine($"Generation time: {timer.Elapsed}");
        timer.Restart();
        AdaptiveSorter.ModifiedAdaptiveMergeSort(path);
        timer.Start();
        //AdaptiveSorter.AdaptiveMergeSort(path);
        timer.Stop();
        Console.WriteLine($"Sorting time: {timer.Elapsed}");
        timer.Restart();
        AdaptiveSorter.IsSorted(path);
    }

    static void GenerateFile(string path, int n)
    {
        using var streamWriter = new StreamWriter(path);
        Random rand = new Random();
        string charSet = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        for (int i = 0; i < n; i++)
        {
            string randomChar = "";
            for (int j = 0; j < rand.Next(1, 20); j++)
            {
                randomChar += charSet[rand.Next(0, charSet.Length)];
            }

            DateTime start = new DateTime(2000, 1, 1);
            DateTime end = DateTime.Today;

            int range = (end - start).Days;

            DateTime randomDate = start.AddDays(rand.Next(range));

            string dateString = randomDate.ToString("dd.MM.yyyy");
            var row = $"{rand.Next(1, 100000)}-{randomChar}-{dateString}";
            streamWriter.WriteLine(row);
        }

        Console.WriteLine("Generation done!");
    }
}
