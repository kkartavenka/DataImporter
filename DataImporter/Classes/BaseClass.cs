using System.Globalization;
using CsvHelper.Configuration;
using DataImporter.Models;

namespace DataImporter.Classes;

public abstract class BaseClass : IDataReader
{
    internal List<Ohlc> _data = new();
    internal int _roundPoint = -1;
    internal bool _isInitialized;

    public abstract void Read(string filePath);

    public List<Ohlc> Data
    {
        get
        {
            CheckInitialized();
            return _data;
        }
    }

    public int RoundPoint
    {
        get
        {
            CheckInitialized();
            return _roundPoint;
        }
    }

    public IReaderConfiguration ReaderConfiguration { get; set; } = new CsvConfiguration(CultureInfo.InvariantCulture)
    {
        Delimiter = ","
    };

    public string? Symbol { get; protected set; }

    private void CheckInitialized()
    {
        if (!_isInitialized)
            throw new Exception($"{nameof(Data)} is empty, initialized it by providing a file via {nameof(Read)}");
    }
}