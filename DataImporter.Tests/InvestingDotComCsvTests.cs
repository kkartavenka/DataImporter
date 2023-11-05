using DataImporter.Models;

namespace DataImporter.Tests;

[TestFixture("TestData/AMD Historical Data.csv")]
[TestFixture("TestData/NVDA Historical Data.csv")]
public class Tests
{
    private readonly string _sourceFile;
    public Tests(string sourceFile)
    {
        _sourceFile = sourceFile;
    }
    
    [Test]
    public void SourceAsString()
    {
        var reader = new DataImporter()
        {
            IgnoreVolume = VolumeBehavior.IgnoreReading
        };
        var dataReader = reader.GetReader(ImportSourceType.InvestingDotComCsv);
        dataReader.Import(_sourceFile);
        Assert.Multiple(() =>
        {
            Assert.That(dataReader.Data.Any(), Is.True);
            Assert.That(dataReader.Data.First().Volume is null, Is.True);
        });
    }
    
    [Test]
    public void SourceAsCsvSource()
    {
        var reader = new DataImporter()
        {
            IgnoreVolume = VolumeBehavior.IgnoreReading
        };
        var dataReader = reader.GetReader(ImportSourceType.InvestingDotComCsv);
        dataReader.Import(new CsvSource(_sourceFile));
        Assert.Multiple(() =>
        {
            Assert.That(dataReader.Data.Any(), Is.True);
            Assert.That(dataReader.Data.First().Volume is null, Is.True);
        });
    }
    
    [Test]
    public void SourceWrongType()
    {
        var reader = new DataImporter()
        {
            IgnoreVolume = VolumeBehavior.IgnoreReading
        };
        var dataReader = reader.GetReader(ImportSourceType.InvestingDotComCsv);
        
        Assert.Throws<ArgumentException>(() => dataReader.Import(16));
    }
}