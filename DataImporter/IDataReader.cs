using DataImporter.Models;

namespace DataImporter;

public interface IDataReader
{
    void Import(object sourceInfo);
    Task ImportAsync(object sourceInfo);
    List<Ohlc> Data { get; }
    int RoundPoint { get; }
    string? Symbol { get; }
    VolumeBehavior VolumeBehavior { get; }
}