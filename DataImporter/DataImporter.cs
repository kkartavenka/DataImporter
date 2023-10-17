using DataImporter.Classes;
using DataImporter.Importers.InvestingDotCom;
using DataImporter.Importers.Metatrader;
using DataImporter.Models;

namespace DataImporter;

public class DataImporter
{
    public IDataReader GetReader(ImportSourceType sourceType) => sourceType switch
    {
        ImportSourceType.InvestingDotComHttps => new InvestingDotComImporter(IgnoreVolume),
        ImportSourceType.InvestingDotComCsv => new InvestingDotComCsvReader(IgnoreVolume),
        ImportSourceType.MetaTrader => new MetaTraderExportReader(IgnoreVolume),
        _ => throw new NotImplementedException()
    };
    
    public bool IgnoreVolume { get; init; }
}