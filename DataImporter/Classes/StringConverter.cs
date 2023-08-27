using System.Text.RegularExpressions;

namespace DataImporter.Classes;

internal static class StringConverter
{
    private static readonly Regex NumericPartRegex = new(@"\d+((\.?\,?\d+)?)");
    private static readonly Regex NonNumericPartRegex = new(@"(\D+)\z");

    private const string KMultiplier = "k";
    private const string MMultiplier = "m";

    internal static double ToDouble(this string? value)
    {
        if (value is null)
        {
            throw new ArgumentNullException(value);
        }
        
        var loweredValue = value.ToLower();

        var numericPart = NumericPartRegex.Match(loweredValue);

        if (!numericPart.Success)
        {
            throw new FormatException($"Cannot obtain numeric part from string {nameof(value)} = {value}");
        }

        var nonNumericPart = NonNumericPartRegex.Match(loweredValue);

        var multiplier = nonNumericPart.Value switch
        {
            KMultiplier when nonNumericPart.Success => 1e3,
            MMultiplier when nonNumericPart.Success => 1e6,
            _ => 1
        };

        if (double.TryParse(numericPart.Value, out var dblPart))
        {
            return dblPart * multiplier;
        }

        throw new FormatException($"Unable to parse volume format into double {nameof(value)} = {value}");
    }
}