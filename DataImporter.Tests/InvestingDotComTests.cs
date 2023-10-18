using DataImporter.Importers.InvestingDotCom;
using DataImporter.Models;

namespace DataImporter.Tests;

public class InvestingDotComTests
{
    [Test]
    public void DirectDownloadTest()
    {
        var requestInfo = new OnlineRequestInfo
        {
            Uri = new Uri("https://www.investing.com/equities/exxon-mobil-historical-data"),
        };
        
        var reader = new DataImporter()
        {
            IgnoreVolume = VolumeBehavior.IgnoreReading
        };
        var dataReader = reader.GetReader(ImportSourceType.InvestingDotComHttps);
        dataReader.Import(requestInfo);
    }
}