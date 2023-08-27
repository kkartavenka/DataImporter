using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using DataImporter.Models;

namespace DataImporter.Classes;

public class InvestingDotComReader : BaseClass
{
    private readonly string[] _knownDateTimeFormats = { "MMM d, yyyy", "d-MMM-yy", "M/d/yyyy" };

    public override void Import(string filePath)
    {
        GetSymbol(filePath);
        using var reader = new StreamReader(filePath);
        using var csv = new CsvReader(reader, ReaderConfiguration);

        csv.Read();
        csv.ReadHeader();

        while (csv.Read())
        {
            var newElement = new Ohlc(
                csv.GetField<double>((int)InvestingDotComColumn.Open),
                csv.GetField<double>((int)InvestingDotComColumn.High),
                csv.GetField<double>((int)InvestingDotComColumn.Low),
                csv.GetField<double>((int)InvestingDotComColumn.Close),
                IgnoreVolume ? null : csv.GetField<string>((int)InvestingDotComColumn.Volume).ToDouble(),
                GetDate(csv.GetField<string>((int)InvestingDotComColumn.Date)));

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

    public InvestingDotComReader(bool ignoreVolume) : base(ignoreVolume)
    {
    }
}