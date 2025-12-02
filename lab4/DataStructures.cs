namespace lab4;

public class Record
{
    public int Key { get; set; }
    public string Data { get; set; }

    public Record(int key, string data)
    {
        Key = key;
        Data = data;
    }

    public override string ToString() => $"Key: {Key}, Data: {Data}";
}

/// <summary>
/// Sparse Index with Overflow Area Data Structure
/// 
/// STRUCTURE:
/// - Main Index: Sorted array of index entries (sparse, not all keys present)
/// - Overflow Area: Linked list for records that don't fit in main index
/// - Each index entry points to a block of records
/// 
/// SEARCH ALGORITHM (Homogeneous Binary Search):
/// 1. Binary search in index to find appropriate block
/// 2. Linear search within the block
/// 3. If not found, check overflow area
/// 
/// TIME COMPLEXITY:
/// - Search: O(log n) for index + O(k) for block search = O(log n + k)
///   where n = number of index entries, k = block size
/// - Insert: O(n) worst case (may need to reorganize)
/// - Delete: O(log n + k)
/// - Edit: O(log n + k)
/// </summary>
public class SparseIndexDatabase
{
    private const int BLOCK_SIZE = 10; // Records per block
    private const int INDEX_INTERVAL = 5; // Index every 5th record

    private List<IndexEntry> _index; // Sparse index
    private List<Record> _overflowArea; // Overflow area for records
    private List<Record> _allRecords; // All records sorted by key

    public int ComparisonCount { get; private set; }

    public SparseIndexDatabase()
    {
        _index = new List<IndexEntry>();
        _overflowArea = new List<Record>();
        _allRecords = new List<Record>();
        ComparisonCount = 0;
    }

    private class IndexEntry
    {
        public int Key { get; set; }
        public int BlockIndex { get; set; }

        public IndexEntry(int key, int blockIndex)
        {
            Key = key;
            BlockIndex = blockIndex;
        }
    }

    public Record? Search(int key)
    {
        ComparisonCount = 0;

        int left = 0, right = _index.Count - 1;
        int blockIndex = 0;

        while (left <= right)
        {
            int mid = (left + right) / 2;
            ComparisonCount++;

            if (_index[mid].Key == key)
            {
                blockIndex = _index[mid].BlockIndex;
                break;
            }
            else if (_index[mid].Key < key)
            {
                blockIndex = _index[mid].BlockIndex;
                left = mid + 1;
            }
            else
            {
                right = mid - 1;
            }
        }

        int blockStart = blockIndex * BLOCK_SIZE;
        int blockEnd = Math.Min(blockStart + BLOCK_SIZE, _allRecords.Count);

        for (int i = blockStart; i < blockEnd; i++)
        {
            ComparisonCount++;
            if (_allRecords[i].Key == key)
                return _allRecords[i];
        }

        foreach (var record in _overflowArea)
        {
            ComparisonCount++;
            if (record.Key == key)
                return record;
        }

        return null;
    }

    public void Add(Record record)
    {
        if (Search(record.Key) != null)
            throw new InvalidOperationException("Key already exists");

        _allRecords.Add(record);
        _allRecords.Sort((a, b) => a.Key.CompareTo(b.Key));

        RebuildIndex();
    }
    
    public bool Delete(int key)
    {
        var record = Search(key);
        if (record == null)
            return false;

        if (_allRecords.Contains(record))
            _allRecords.Remove(record);
        else if (_overflowArea.Contains(record))
            _overflowArea.Remove(record);

        RebuildIndex();
        return true;
    }

    public bool Edit(int key, string newData)
    {
        var record = Search(key);
        if (record == null)
            return false;

        record.Data = newData;
        return true;
    }

    private void RebuildIndex()
    {
        _index.Clear();

        if (_allRecords.Count == 0)
            return;

        for (int i = 0; i < _allRecords.Count; i += INDEX_INTERVAL)
        {
            int blockIndex = i / BLOCK_SIZE;
            _index.Add(new IndexEntry(_allRecords[i].Key, blockIndex));
        }

        _overflowArea.Clear();
        int maxCapacity = (_index.Count + 1) * BLOCK_SIZE;

        if (_allRecords.Count > maxCapacity)
        {
            _overflowArea = _allRecords.Skip(maxCapacity).ToList();
            _allRecords = _allRecords.Take(maxCapacity).ToList();
        }
    }

    public List<Record> GetAllRecords()
    {
        var result = new List<Record>(_allRecords);
        result.AddRange(_overflowArea);
        result.Sort((a, b) => a.Key.CompareTo(b.Key));
        return result;
    }

    public List<(int Key, int BlockIndex)> GetIndexEntries()
    {
        return _index.Select(e => (e.Key, e.BlockIndex)).ToList();
    }

    public List<Record> GetBlockRecords(int blockIndex)
    {
        int start = blockIndex * BLOCK_SIZE;
        int end = Math.Min(start + BLOCK_SIZE, _allRecords.Count);

        if (start >= _allRecords.Count)
            return new List<Record>();

        return _allRecords.Skip(start).Take(end - start).ToList();
    }

    public List<Record> GetOverflowRecords()
    {
        return new List<Record>(_overflowArea);
    }

    public void Clear()
    {
        _index.Clear();
        _overflowArea.Clear();
        _allRecords.Clear();
        ComparisonCount = 0;
    }

    public int RecordCount => _allRecords.Count + _overflowArea.Count;
}