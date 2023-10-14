using CsvHelper.Configuration;
using DataImporter.Models;

namespace DataImporter;

public interface IDataReader
{
    void Import(object sourceInfo);
    List<Ohlc> Data { get; }
    int RoundPoint { get; }
    IReaderConfiguration ReaderConfiguration { get; set; }
    string? Symbol { get; }
    bool IgnoreVolume { get; }
}