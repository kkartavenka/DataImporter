using CsvHelper.Configuration;
using DataImporter.Models;

namespace DataImporter;

public interface IDataReader
{
    void Read(string filePath);
    List<Ohlc> Data { get; }
    int RoundPoint { get; }
    IReaderConfiguration ReaderConfiguration { get; set; }
}