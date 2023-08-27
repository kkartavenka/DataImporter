namespace DataImporter.Models;

public class Ohlc
{
    public Ohlc(double open, double high, double low, double close, double? volume, DateTimeOffset date) {
        Open = open;
        High = high;
        Low = low;
        Close = close;
        Volume = volume;
        Date = date;
        TypicalPrice = (high + low + close) / 3;
    }

    public double High { get; private init; }
    public double Low { get; private init; }
    public double Close { get; private init; }
    public double Open { get; private init; }
    public double? Volume { get; private init; }
    public double TypicalPrice { get; private init; }
    public DateTimeOffset Date { get; private init; }

}
