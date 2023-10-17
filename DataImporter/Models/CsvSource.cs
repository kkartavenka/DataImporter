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

    public required string FilePath { get; init; }
    public IReaderConfiguration? ReaderConfiguration { get; init; }
}