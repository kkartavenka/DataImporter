using DataImporter.Importers.InvestingDotCom;
using DataImporter.Models;

namespace DataImporter.Tests;

public class InvestingDotComTests
{
    [Test]
    public void DirectDownloadTest()
    {
        //https://api.investing.com/api/financialdata/historical/7888?start-date=2023-09-13&end-date=2023-10-13&time-frame=Daily&add-missing-rows=false
        var requestInfo = new OnlineRequestInfo
        {
            Uri = new Uri("https://www.investing.com/equities/exxon-mobil-historical-data"),
        };
        
        var reader = new DataImporter()
        {
            IgnoreVolume = true
        };
        var dataReader = reader.GetReader(ImportSourceType.InvestingDotComHttps);
        dataReader.Import(requestInfo);
    }
}