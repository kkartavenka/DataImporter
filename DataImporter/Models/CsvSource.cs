using System.Diagnostics.CodeAnalysis;
using CsvHelper.Configuration;

namespace DataImporter.Models;

public class CsvSource
{
    public CsvSource()
    {
    }

    [SetsRequiredMembers]
    public CsvSource(string filePath)
    {
        FilePath = filePath;
    }
    
    public CsvSource(Stream stream)
    {
        StreamSource = stream;
    }

    public string? FilePath { get; }
    public Stream? StreamSource { get; init; }
    public IReaderConfiguration? ReaderConfiguration { get; init; }
}