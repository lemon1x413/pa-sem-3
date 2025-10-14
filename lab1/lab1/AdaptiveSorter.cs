using System.Text;

namespace lab1;

public static class AdaptiveSorter
{
    const string pathB = "fileB.txt";

    const string pathC = "fileC.txt";

    const int bufferSize = 1024 * 1024;

    public static void ModifiedAdaptiveMergeSort(string pathA)
    {
        const string pathT = "fileT.txt";

        const int bytesToRead = 40 * bufferSize;

        using (var fileAStreamReader = new StreamReader(pathA, Encoding.UTF8, true, bufferSize))
        using (var fileTStreamWriter = new StreamWriter(pathT, false, Encoding.UTF8, bufferSize))
        {
            var records = new List<string>();

            while (!fileAStreamReader.EndOfStream)
            {
                int bytesRead = 0;
                while (!fileAStreamReader.EndOfStream && bytesRead < bytesToRead)
                {
                    var line = fileAStreamReader.ReadLine();
                    bytesRead += Encoding.UTF8.GetByteCount(line);
                    records.Add(line);
                }

                records.Sort(CompareLines);

                foreach (var record in records)
                {
                    fileTStreamWriter.WriteLine(record);
                }

                records.Clear();
            }

            fileAStreamReader.Close();
            fileTStreamWriter.Close();
        }


        using (var fileAStreamWriter = new StreamWriter(pathA, false, Encoding.UTF8, bufferSize))
        using (var fileTStreamReader = new StreamReader(pathT, Encoding.UTF8, true, bufferSize))
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
        using (var fileAStreamReader = new StreamReader(path, Encoding.UTF8, true, bufferSize))
        using (var fileBStreamWriter = new StreamWriter(pathB, false, Encoding.UTF8, bufferSize))
        using (var fileCStreamWriter = new StreamWriter(pathC, false, Encoding.UTF8, bufferSize))
        {
            string? line;
            var lineCounter = 0;
            var index = 1;
            while (!fileAStreamReader.EndOfStream)
            {
                line = fileAStreamReader.ReadLine();
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
        using (var fileAStreamWriter = new StreamWriter(path, false, Encoding.UTF8, bufferSize))
        using (var fileBStreamReader = new StreamReader(pathB, Encoding.UTF8, true, bufferSize))
        using (var fileCStreamReader = new StreamReader(pathC, Encoding.UTF8, true, bufferSize))
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
        using (var streamReader = new StreamReader(path, Encoding.UTF8, true, bufferSize))
        {
            var indexIterator = 0;
            string? line;
            string? previousLine = null;
            while (!streamReader.EndOfStream)
            {
                line = streamReader.ReadLine();
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
        using (StreamReader streamReader = new(path, Encoding.UTF8, true, bufferSize))
        {
            var prevLine = streamReader.ReadLine();
            string? line;
            while (!streamReader.EndOfStream)
            {
                line = streamReader.ReadLine();
                if (CompareLines(prevLine, line) > 0)
                {
                    Console.WriteLine("File is not Sorted");
                    return;
                }

                prevLine = line;
            }

            Console.WriteLine("File is Sorted");
        }
    }
}