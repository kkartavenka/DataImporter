using DataImporter.Models;

namespace DataImporter.Tests;

public class Tests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void Test1()
    {
        var reader = new DataImporter().GetReader(ImportSourceType.InvestingDotCom);
        reader.Read("NVDA Historical Data.csv");
        Assert.True(reader.Data.Any());
    }
}