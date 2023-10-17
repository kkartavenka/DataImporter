using System.Globalization;
using DataImporter.Classes;

namespace DataImporter.Importers.InvestingDotCom;

public class InvestingDotDomBase: BaseClass
{
    private readonly string[] _knownDateTimeFormats = { "MMM d, yyyy", "d-MMM-yy", "M/d/yyyy" };

    internal DateTimeOffset GetDate(string? dateString)
    {
        if (dateString is null)
        {
            throw new ArgumentNullException(nameof(dateString));
        }

        dateString = dateString.Replace("\"", "");

        foreach (var dtFormat in _knownDateTimeFormats)
            if (DateTimeOffset.TryParseExact(dateString, dtFormat, null, DateTimeStyles.None, out var convertedDate))
                return convertedDate;

        throw new FormatException($"Unexpected format {nameof(dateString)} = {dateString}");
    }
    
    internal void UpdateDecimalCount(double value)
    {
        var valueAsDecimal = (decimal)value;
        var count = BitConverter.GetBytes(decimal.GetBits(valueAsDecimal)[3])[2];
        if (count > _roundPoint)
        {
            _roundPoint = count;
        }
    }

    protected InvestingDotDomBase(bool ignoreVolume) : base(ignoreVolume)
    {
    }

    public override void Import(object sourceInfo)
    {
        throw new NotImplementedException();
    }

    public override Task ImportAsync(object sourceInfo)
    {
        throw new NotImplementedException();
    }
}