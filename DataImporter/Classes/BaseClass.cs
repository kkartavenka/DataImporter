using DataImporter.Models;

namespace DataImporter.Classes;

public abstract class BaseClass : IDataReader
{
    internal List<Ohlc> _data = new();
    internal int _roundPoint = -1;
    internal bool IsInitialized;

    protected BaseClass(VolumeBehavior volumeBehavior)
    {
        VolumeBehavior = volumeBehavior;
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
    
    public string? Symbol { get; protected set; }
    public VolumeBehavior VolumeBehavior { get; }

    private void CheckInitialized()
    {
        if (!IsInitialized)
        {
            throw new Exception($"{nameof(Data)} is empty, initialized it by providing a file via {nameof(Import)}");
        }
    }
}