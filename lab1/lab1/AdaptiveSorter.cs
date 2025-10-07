

using System.Text;

namespace lab1;

public static class AdaptiveSorter
{
    const string pathB = "../../../files/fileB.txt";

    const string pathC = "../../../files/fileC.txt";

    public static void ModifiedAdaptiveMergeSort(string pathA)
    {
        const string pathT = "../../../files/fileT.txt";

        using (var fileAStreamReader = new StreamReader(pathA, Encoding.UTF8))
        using (var fileTStreamWriter = new StreamWriter(pathT))
        {
            var records = new List<string>();
            const int bytesToRead = 40 * 1024 * 1024;
            while (!fileAStreamReader.EndOfStream)
            {
                int bytesRead = 0;
                while (fileAStreamReader.ReadLine() is { } line && bytesRead < bytesToRead)
                {
                    bytesRead += Encoding.UTF8.GetByteCount(line);
                    records.Add(line);
                }

                QuickSort(records, 0, records.Count - 1);
                foreach (var record in records)
                {
                    fileTStreamWriter.WriteLine(record);
                }

                records.Clear();
            }
            fileAStreamReader.Close();
            fileTStreamWriter.Close();
        }

        
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

    private static void QuickSort(List<string> A, int left, int right)
    {
        if (left < right)
        {
            int pivot = Partition(A, left, right);
            QuickSort(A, left, pivot - 1);
            QuickSort(A, pivot + 1, right);
        }
    }

    private static int Partition(List<string> A, int left, int right)
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
            while (indexB < indexListB.Count && indexC < indexListC.Count)
            {
                int n1 = indexListB[indexB] - indexListB[indexB - 1];
                int n2 = indexListC[indexC] - indexListC[indexC - 1];

                int i = 0;
                int j = 0;

                while (i < n1 && j < n2)
                {
                    if (CompareLines(lineB, lineC) <= 0)
                    {
                        fileAStreamWriter.WriteLine(lineB);
                        if (!fileBStreamReader.EndOfStream)
                            lineB = fileBStreamReader.ReadLine();
                        i++;
                    }
                    else
                    {
                        fileAStreamWriter.WriteLine(lineC);
                        if (!fileCStreamReader.EndOfStream)
                            lineC = fileCStreamReader.ReadLine();
                        j++;
                    }
                }

                while (i < n1)
                {
                    fileAStreamWriter.WriteLine(lineB);
                    if (!fileBStreamReader.EndOfStream)
                        lineB = fileBStreamReader.ReadLine();
                    i++;
                }

                while (j < n2)
                {
                    fileAStreamWriter.WriteLine(lineC);
                    if (!fileCStreamReader.EndOfStream)
                        lineC = fileCStreamReader.ReadLine();
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
                    fileAStreamWriter.WriteLine(lineB);
                    if (!fileBStreamReader.EndOfStream)
                        lineB = fileBStreamReader.ReadLine();
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
                    fileAStreamWriter.WriteLine(lineC);
                    if (!fileCStreamReader.EndOfStream)
                        lineC = fileCStreamReader.ReadLine();
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
            var prevLine = streamReader.ReadLine();
            string? line;
            while ((line = streamReader.ReadLine()) != null)
            {
                if (CompareLines(prevLine,line) > 0)
                {
                    Console.WriteLine("Not Sorted");
                    return;
                }
            }

            Console.WriteLine("Is Sorted");
        }
    }
}