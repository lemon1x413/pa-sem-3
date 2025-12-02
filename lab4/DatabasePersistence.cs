using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace lab4;


public class DatabasePersistence
{
    private readonly string _filePath;

    public DatabasePersistence(string filePath = "database.dat")
    {
        _filePath = filePath;
    }


    public void Save(SparseIndexDatabase database)
    {
        try
        {
            using var writer = new BinaryWriter(File.Create(_filePath), Encoding.UTF8);
            var records = database.GetAllRecords();
            writer.Write(records.Count);

            foreach (var record in records)
            {
                writer.Write(record.Key);
                writer.Write(record.Data ?? "");
            }
        }
        catch (Exception ex)
        {
            throw new IOException($"Failed to save database: {ex.Message}", ex);
        }
    }
    
    public void Load(SparseIndexDatabase database)
    {
        if (!File.Exists(_filePath))
            return;

        try
        {
            database.Clear();

            using (var reader = new BinaryReader(File.OpenRead(_filePath), Encoding.UTF8))
            {
                int count = reader.ReadInt32();

                for (int i = 0; i < count; i++)
                {
                    int key = reader.ReadInt32();
                    string data = reader.ReadString();
                    database.Add(new Record(key, data));
                }
            }
        }
        catch (Exception ex)
        {
            throw new IOException($"Failed to load database: {ex.Message}", ex);
        }
    }

    public void Delete()
    {
        if (File.Exists(_filePath))
            File.Delete(_filePath);
    }

    public bool Exists => File.Exists(_filePath);
}
