using System.Globalization;
using CsvHelper.Configuration;
using DataImporter.Models;

namespace DataImporter.Classes;

public abstract class BaseClass : IDataReader
{
    internal List<Ohlc> _data = new();
    internal int _roundPoint = -1;
    internal bool _isInitialized;

    public BaseClass(bool ignoreVolume)
    {
        IgnoreVolume = ignoreVolume;
    }
    public abstract void Import(object sourceInfo);
    public abstract Task ImportAsync(object sourceInfo);

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

    public string? Symbol { get; set; }
    public bool IgnoreVolume { get; }

    private void CheckInitialized()
    {
        if (!_isInitialized)
        {
            throw new Exception($"{nameof(Data)} is empty, initialized it by providing a file via {nameof(Import)}");
        }
    }
}