using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using DataImporter.Models;

namespace DataImporter.Classes;

public class InvestingDotComCsvReader : BaseClass
{
    private IReaderConfiguration _defaultConfiguration = new CsvConfiguration(CultureInfo.InvariantCulture)
    {
        Delimiter = ","
    };
    private readonly string[] _knownDateTimeFormats = { "MMM d, yyyy", "d-MMM-yy", "M/d/yyyy" };

    public override void Import(object sourceInfo)
    {
        CsvSource source = sourceInfo switch
        {
            string filePath => new CsvSource(filePath),
            CsvSource csvSource => csvSource,
            _ => throw new ArgumentException(
                $"Expected type {typeof(string)} or {typeof(CsvSource)} for {nameof(sourceInfo)}")
        };

        GetSymbol(source.FilePath);
        using var reader = new StreamReader(source.FilePath);
        using var csv = new CsvReader(reader, source.ReaderConfiguration ?? _defaultConfiguration);

        csv.Read();
        csv.ReadHeader();

        while (csv.Read())
        {
            var newElement = new Ohlc(
                csv.GetField<double>((int)InvestingDotComCsvColumn.Open),
                csv.GetField<double>((int)InvestingDotComCsvColumn.High),
                csv.GetField<double>((int)InvestingDotComCsvColumn.Low),
                csv.GetField<double>((int)InvestingDotComCsvColumn.Close),
                IgnoreVolume ? null : csv.GetField<string>((int)InvestingDotComCsvColumn.Volume).ToDouble(),
                GetDate(csv.GetField<string>((int)InvestingDotComCsvColumn.Date)));

            UpdateDecimalCount(newElement.Close);

            _data.Add(newElement);
        }

        _data = _data.OrderBy(m => m.Date).ToList();

        _isInitialized = true;
    }

    private DateTimeOffset GetDate(string? dateString)
    {
        if (dateString is null) throw new ArgumentNullException(nameof(dateString));

        dateString = dateString.Replace("\"", "");

        foreach (var dtFormat in _knownDateTimeFormats)
            if (DateTimeOffset.TryParseExact(dateString, dtFormat, null, DateTimeStyles.None, out var convertedDate))
                return convertedDate;

        throw new FormatException($"Unexpected format {nameof(dateString)} = {dateString}");
    }

    private void GetSymbol(string fileName)
    {
        if (!File.Exists(fileName))
            throw new FileNotFoundException(fileName);

        Symbol = Path.GetFileNameWithoutExtension(fileName);
    }

    private void UpdateDecimalCount(double value)
    {
        var valueAsDecimal = (decimal)value;
        var count = BitConverter.GetBytes(decimal.GetBits(valueAsDecimal)[3])[2];
        if (count > _roundPoint) _roundPoint = count;
    }

    public InvestingDotComCsvReader(bool ignoreVolume) : base(ignoreVolume)
    {
    }
}