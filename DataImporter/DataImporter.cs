using DataImporter.Classes;
using DataImporter.Models;

namespace DataImporter;

public class DataImporter
{
    public IDataReader GetReader(ImportSourceType sourceType) => sourceType switch
    {
        ImportSourceType.InvestingDotCom => new InvestingDotComReader(IgnoreVolume),
        ImportSourceType.MetaTrader => new MetaTraderExportReader(IgnoreVolume),
        _ => throw new NotImplementedException()
    };
    
    public bool IgnoreVolume { get; set; }
}