namespace DataImporter.Models;

public class InvestingDotComHistoricalData
{
    public string? InstrumentId { get; set; }
    public required string Instrument { get; init; }
    public Uri? Uri { get; init; }
}