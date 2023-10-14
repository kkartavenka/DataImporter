using CsvHelper.Configuration;
using DataImporter.Models;

namespace DataImporter.Classes;

public class InvestingDotComImporter: IDataReader
{
    public void Import(object sourceInfo)
    {
        throw new NotImplementedException();
    }

    public List<Ohlc> Data { get; }
    public int RoundPoint { get; }
    public IReaderConfiguration ReaderConfiguration { get; set; }
    public string? Symbol { get; }
    public bool IgnoreVolume { get; }
}