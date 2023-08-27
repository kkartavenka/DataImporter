using DataImporter.Models;

namespace DataImporter.Tests;

public class Tests
{
    [SetUp]
    public void Setup()
    {
    }

    // AMD Historical Data.csv
    // NVDA Historical Data.csv
    [Test]
    public void Test1()
    {
        var reader = new DataImporter()
        {
            IgnoreVolume = true
        };
        var amd = reader.GetReader(ImportSourceType.InvestingDotCom);
        amd.Import("AMD Historical Data.csv");
        Assert.True(amd.Data.Any());
        Assert.True(amd.Data.First().Volume is null);

        reader.IgnoreVolume = false;
        var nvda = reader.GetReader(ImportSourceType.InvestingDotCom);
        nvda.Import("NVDA Historical Data.csv");
        Assert.True(nvda.Data.Any());
        Assert.False(nvda.Data.First().Volume is null);
    }
}