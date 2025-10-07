using System.Buffers;
using System.Data;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace lab1;

public readonly struct Record : IComparable<Record>
{
    public Record(string line)
    {
        Key = int.Parse(line.Split('-')[0]);
        this.line = line;
    }

    private int Key { get; }
    private string line { get; }


    public override string ToString() => line;

    public int CompareTo(Record other) => Key.CompareTo(other.Key);

    public override bool Equals(object? obj) => obj is Record record && Key == record.Key;

    public override int GetHashCode() => Key.GetHashCode();

    public static bool operator <(Record rec1, Record rec2) => rec1.Key < rec2.Key;

    public static bool operator >(Record rec1, Record rec2) => rec1.Key > rec2.Key;

    public static bool operator <=(Record rec1, Record rec2) => rec1.Key <= rec2.Key;

    public static bool operator >=(Record rec1, Record rec2) => rec1.Key >= rec2.Key;

    public static bool operator ==(Record rec1, Record rec2) => rec1.Key == rec2.Key;

    public static bool operator !=(Record rec1, Record rec2) => rec1.Key != rec2.Key;
}

class Program
{
    static void Main(string[] args)
    {
        var path = "../../../files/fileA.txt";
        GenerateFile(path, 45000000);
        var timer = new Stopwatch();
        timer.Start();
        AdaptiveSorter.ModifiedAdaptiveMergeSort(path);

        //AdaptiveSorter.AdaptiveMergeSort(path);
        timer.Stop();
        Console.WriteLine(timer.Elapsed);

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

static class AdaptiveSorter
{
    const string pathB = "../../../files/fileB.txt";

    const string pathC = "../../../files/fileC.txt";

    public static void ModifiedAdaptiveMergeSort(string pathA)
    {
        const string pathT = "../../../files/fileT.txt";

        using var fileAStreamReader = new StreamReader(pathA, Encoding.UTF8);
        using var fileTStreamWriter = new StreamWriter(pathT);
        var records = new List<string>();
        const int bytesToRead = 20 * 1024 * 1024;
        while (!fileAStreamReader.EndOfStream)
        {
            int bytesRead = 0;
            while (fileAStreamReader.ReadLine() is { } line && bytesRead < bytesToRead)
            {
                bytesRead += Encoding.UTF8.GetByteCount(line);

                if (bytesRead <= bytesToRead)
                    records.Add(line);
                else
                    break;
            }

            Console.WriteLine(records.Count);

            QuickSort(records, 0, records.Count - 1);
            foreach (var record in records)
            {
                fileTStreamWriter.WriteLine(record);
            }

            records.Clear();
        }

        fileAStreamReader.Close();
        fileTStreamWriter.Close();
        using (var fileAStreamWriter = new StreamWriter(pathA))
        using (var fileTStreamReader = new StreamReader(pathT))
        {
            while (!fileTStreamReader.EndOfStream)
            {
                fileAStreamWriter.WriteLine(fileTStreamReader.ReadLine());
            }

            fileAStreamWriter.Close();
            fileTStreamReader.Close();
        }

        AdaptiveMergeSort(pathA);
    }

    static int CompareLines(string a, string b)
    {
        int idxA = a.IndexOf('-');
        int idxB = b.IndexOf('-');

        int keyA = int.Parse(a.AsSpan(0, idxA));
        int keyB = int.Parse(b.AsSpan(0, idxB));

        return keyA.CompareTo(keyB);
    }

    static void QuickSort(List<string> A, int left, int right)
    {
        if (left < right)
        {
            int pivot = Partition(A, left, right);
            QuickSort(A, left, pivot - 1);
            QuickSort(A, pivot + 1, right);
        }
    }

    static int Partition(List<string> A, int left, int right)
    {
        string x = A[right];
        int i = left - 1;
        for (int j = left; j < right; j++)
        {
            if (CompareLines(A[j], x) <= 0)
            {
                i++;
                (A[i], A[j]) = (A[j], A[i]);
            }
        }

        (A[i + 1], A[right]) = (A[right], A[i + 1]);
        return i + 1;
    }

    public static void AdaptiveMergeSort(string pathA)
    {
        List<int> indexListA = IndexCounter(pathA);
        while (indexListA.Count > 2)
        {
            Split(indexListA, pathA);

            var indexListB = IndexCounter(pathB);
            var indexListC = IndexCounter(pathC);

            AdaptiveMerge(indexListB, indexListC, pathA);

            Console.WriteLine(indexListA.Count);

            indexListA = IndexCounter(pathA);
        }

        Console.WriteLine("Sorting done!");
    }

    private static void Split(List<int> indexList, string path)
    {
        using (var fileAStreamReader = new StreamReader(path))
        using (var fileBStreamWriter = new StreamWriter(pathB))
        using (var fileCStreamWriter = new StreamWriter(pathC))
        {
            string? line;
            var lineCounter = 0;
            var index = 1;
            while ((line = fileAStreamReader.ReadLine()) != null)
            {
                if (lineCounter == indexList[index - 1])
                {
                    index++;
                }

                if (index % 2 == 0)
                {
                    fileBStreamWriter.WriteLine(line);
                }
                else
                {
                    fileCStreamWriter.WriteLine(line);
                }

                lineCounter++;
            }
        }
    }

    private static void AdaptiveMerge(List<int> indexListB, List<int> indexListC, string path)
    {
        using (var fileAStreamWriter = new StreamWriter(path))
        using (var fileBStreamReader = new StreamReader(pathB))
        using (var fileCStreamReader = new StreamReader(pathC))
        {
            int indexB = 1;
            int indexC = 1;
            string? lineB = fileBStreamReader.ReadLine();
            string? lineC = fileCStreamReader.ReadLine();
            Record recordB = new(lineB);
            Record recordC = new(lineC);
            while (indexB < indexListB.Count && indexC < indexListC.Count)
            {
                int n1 = indexListB[indexB] - indexListB[indexB - 1];
                int n2 = indexListC[indexC] - indexListC[indexC - 1];

                int i = 0;
                int j = 0;

                while (i < n1 && j < n2)
                {
                    if (recordB <= recordC)
                    {
                        fileAStreamWriter.WriteLine(recordB.ToString());
                        if ((lineB = fileBStreamReader.ReadLine()) != null)
                            recordB = new(lineB);
                        i++;
                    }
                    else
                    {
                        fileAStreamWriter.WriteLine(recordC.ToString());
                        if ((lineC = fileCStreamReader.ReadLine()) != null)
                            recordC = new(lineC);
                        j++;
                    }
                }

                while (i < n1)
                {
                    fileAStreamWriter.WriteLine(recordB.ToString());
                    if ((lineB = fileBStreamReader.ReadLine()) != null)
                        recordB = new(lineB);
                    i++;
                }

                while (j < n2)
                {
                    fileAStreamWriter.WriteLine(recordC.ToString());
                    if ((lineC = fileCStreamReader.ReadLine()) != null)
                        recordC = new(lineC);
                    j++;
                }

                indexB++;
                indexC++;
            }

            while (indexB < indexListB.Count)
            {
                int i = 0;
                int n1 = indexListB[indexB] - indexListB[indexB - 1];
                while (i < n1)
                {
                    fileAStreamWriter.WriteLine(recordB.ToString());
                    if ((lineB = fileBStreamReader.ReadLine()) != null)
                        recordB = new(lineB);
                    i++;
                }

                indexB++;
            }

            while (indexC < indexListC.Count)
            {
                int j = 0;
                int n2 = indexListC[indexC] - indexListC[indexC - 1];
                while (j < n2)
                {
                    fileAStreamWriter.WriteLine(recordC.ToString());
                    if ((lineC = fileCStreamReader.ReadLine()) != null)
                        recordC = new(lineC);
                    j++;
                }

                indexC++;
            }
        }
    }
    
    private static List<int> IndexCounter(string path)
    {
        List<int> indexList = new([0]);
        using (var streamReader = new StreamReader(path))
        {
            var indexIterator = 0;
            string? line;
            string? previousLine = null;
            while ((line = streamReader.ReadLine()) != null)
            {
                if (previousLine != null)
                {
                    if (CompareLines(line, previousLine) < 0)
                    {
                        indexList.Add(indexIterator);
                    }
                }

                previousLine = line;
                indexIterator++;
            }

            indexList.Add(indexIterator);
        }

        return indexList;
    }

    public static void IsSorted(string path)
    {
        using (StreamReader streamReader = new(path))
        {
            var prevRecord = new Record(streamReader.ReadLine());
            string? line;
            while ((line = streamReader.ReadLine()) != null)
            {
                var record = new Record(line);
                if (prevRecord > record)
                {
                    Console.WriteLine("Not Sorted");
                    return;
                }
            }

            Console.WriteLine("Is Sorted");
        }
    }
}