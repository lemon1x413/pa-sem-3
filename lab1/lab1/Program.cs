using System.Data;

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

    public static bool operator <(Record rec1, Record rec2)
    {
        return rec1.Key < rec2.Key;
    }
    public static bool operator >(Record rec1, Record rec2)
    {
        return rec1.Key > rec2.Key;
    }   
    public static bool operator <=(Record rec1, Record rec2)
    {
        return rec1.Key <= rec2.Key;
    }
    public static bool operator >=(Record rec1, Record rec2)
    {
        return rec1.Key >= rec2.Key;
    }
    
    
}

class Program
{
    static void Main(string[] args)
    {
        var path = "../../../files/file.txt";
        //GenerateFile(path, 10000000);
        using (StreamReader sr = new(path))
        {
            var line = sr.ReadLine();
            Record record1 = new(line);
            Console.WriteLine(record1);
            sr.Close();
        }
        /*int[] arr = GenerateRandomArray(1000, 1, 1000);
        MergeSort(arr, 0, arr.Length - 1);
        PrintArray(arr);*/
    }

    static void MergeSort(int[] arr, int left, int right)
    {
        if (left < right)
        {
            int middle = (left + right) / 2;
            MergeSort(arr, left, middle);
            MergeSort(arr, middle + 1, right);
            Merge(arr, left, middle, right);
        }
    }

    static void Merge(int[] arr, int left, int middle, int right)
    {
        int n1 = middle - left + 1;
        int n2 = right - middle;
        int[] leftArray = new int[n1];
        int[] rightArray = new int[n2];
        int i, j;
        for (i = 0; i < n1; i++)
        {
            leftArray[i] = arr[left + i];
        }

        for (j = 0; j < n2; j++)
        {
            rightArray[j] = arr[middle + 1 + j];
        }

        i = 0;
        j = 0;
        int k = left;
        while (i < n1 && j < n2)
        {
            if (leftArray[i] <= rightArray[j])
            {
                arr[k] = leftArray[i];
                i++;
            }
            else
            {
                arr[k] = rightArray[j];
                j++;
            }

            k++;
        }

        while (i < n1)
        {
            arr[k] = leftArray[i];
            i++;
            k++;
        }

        while (j < n2)
        {
            arr[k] = rightArray[j];
            j++;
            k++;
        }
    }


    static int[] GenerateRandomArray(int size, int min, int max)
    {
        Random random = new Random();
        int[] arr = new int[size];
        for (int i = 0; i < size; i++)
        {
            arr[i] = random.Next(min, max + 1);
        }

        return arr;
    }

    static void PrintArray(int[] arr)
    {
        foreach (var item in arr)
        {
            Console.WriteLine(item);
        }
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
            var row = $"{rand.Next(1, 10000)}-{randomChar}-{dateString}";
            streamWriter.WriteLine(row);
        }

        Console.WriteLine("Done!");
    }
}

static class AdaptiveSorter
{
    static List<int> index = new();

    static void AdaptiveMergeSort(string path)
    {
        using (StreamReader streamReader = new StreamReader(path))
        {
        }
    }

    static void Merge(int[] arr, int left, int middle, int right)
    {
    }
}