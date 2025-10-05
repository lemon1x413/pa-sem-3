using System.Data;
using System.Diagnostics;

namespace lab1;

public readonly struct Record
{
    public Record(string line)
    {
        string[] parts = line.Split('-');
        Key = int.Parse(parts[0]);
        Value = parts[1];
        Date = parts[2];
    }

    private int Key { get; }
    private string Value { get; }
    private string Date { get; }

    public override string ToString() => $"{Key}-{Value}-{Date}";

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
        //GenerateFile(path, 45000000);
        AdaptiveSorter.AdaptiveMergeSort(path);
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

    public static void AdaptiveMergeSort(string pathA)
    {
        List<int> indexListA = IndexCounter(pathA);
        while (indexListA.Count > 2)
        {
            using (var fileAStreamReader = new StreamReader(pathA))
            using (var fileBStreamWriter = new StreamWriter(pathB))
            using (var fileCStreamWriter = new StreamWriter(pathC))
            {
                string? line;
                var lineCounter = 0;
                var index = 1;
                while ((line = fileAStreamReader.ReadLine()) != null)
                {
                    if (lineCounter == indexListA[index - 1])
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

            var indexListB = IndexCounter(pathB);
            var indexListC = IndexCounter(pathC);

            using (var fileAStreamWriter = new StreamWriter(pathA))
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

            indexListA = IndexCounter(pathA);
        }
        Console.WriteLine("Sorting done!");
    }
    
    private static List<int> IndexCounter(string path)
    {
        List<int> indexList = new([0]);
        using (var streamReader = new StreamReader(path))
        {
            var indexIterator = 0;
            string? line;
            Record? previousRecord = null;
            while ((line = streamReader.ReadLine()) != null)
            {
                var record = new Record(line);
                if (previousRecord != null)
                {
                    if (record < previousRecord)
                    {
                        indexList.Add(indexIterator);
                    }
                }

                previousRecord = record;
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