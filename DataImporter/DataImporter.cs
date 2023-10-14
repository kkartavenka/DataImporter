using DataImporter.Classes;
using DataImporter.Models;

namespace DataImporter;

public class DataImporter
{
    public IDataReader GetReader(ImportSourceType sourceType) => sourceType switch
    {
        ImportSourceType.InvestingDotComCsv => new InvestingDotComCsvReader(IgnoreVolume),
        ImportSourceType.MetaTrader => new MetaTraderExportReader(IgnoreVolume),
        _ => throw new NotImplementedException()
    };
    
    public bool IgnoreVolume { get; set; }
    public string InvestingDotComBaseUrl { get; set; } = "https://api.investing.com/api/financialdata/historical/";
}