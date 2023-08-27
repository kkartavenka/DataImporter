using System.Globalization;
using CsvHelper;
using DataImporter.Models;

namespace DataImporter.Classes;

public class InvestingDotComReader : IDataReader
{
    private readonly List<Ohlc> _data = new();
    private readonly string[] _knownDateTimeFormats = { "MMM d, yyyy", "d-MMM-yy", "M/d/yyyy" };
    private bool _isInitialized;
    private int _roundPoint = -1;

    public void Read(string filePath)
    {
        using var reader = new StreamReader(filePath);
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

        csv.Read();
        csv.ReadHeader();

        while (csv.Read())
        {
            var newElement = new Ohlc(
                csv.GetField<double>((int)InvestingDotComColumn.Open),
                csv.GetField<double>((int)InvestingDotComColumn.High),
                csv.GetField<double>((int)InvestingDotComColumn.Low),
                csv.GetField<double>((int)InvestingDotComColumn.Close),
                csv.GetField<string>((int)InvestingDotComColumn.Volume).ToDouble(),
                GetDate(csv.GetField<string>((int)InvestingDotComColumn.Date)));

            UpdateDecimalCount(newElement.Close);
        }

        _isInitialized = true;
    }

    public List<Ohlc> Data
    {
        get
        {
            CheckInitialized();
            return _data;
        }
    }

    public int RoundPoint
    {
        get
        {
            CheckInitialized();
            return _roundPoint;
        }
    }

    private void CheckInitialized()
    {
        if (!_isInitialized)
            throw new Exception($"{nameof(Data)} is empty, initialized it by providing a file via {nameof(Read)}");
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

    private void UpdateDecimalCount(double value)
    {
        var valueAsDecimal = (decimal)value;
        var count = BitConverter.GetBytes(decimal.GetBits(valueAsDecimal)[3])[2];
        if (count > _roundPoint) _roundPoint = count;
    }
}