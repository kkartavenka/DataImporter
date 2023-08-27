using DataImporter.Classes;
using DataImporter.Models;

namespace DataImporter;

public class DataImporter
{
    public IDataReader GetReader(ImportSourceType sourceType) => sourceType switch
    {
        ImportSourceType.InvestingDotCom => new InvestingDotComReader(),
        ImportSourceType.MetaTrader => new MetaTraderExportReader(),
        _ => throw new NotImplementedException()
    };
}