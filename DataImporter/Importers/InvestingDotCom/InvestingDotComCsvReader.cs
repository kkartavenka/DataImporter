using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using DataImporter.Classes;
using DataImporter.Models;

namespace DataImporter.Importers.InvestingDotCom;

public class InvestingDotComCsvReader : InvestingDotDomBase
{
    private const string DefaultLocale = "en_US";
    
    private readonly IReaderConfiguration _defaultConfiguration = new CsvConfiguration(CultureInfo.InvariantCulture)
    {
        Delimiter = ","
    };
    
    public InvestingDotComCsvReader(VolumeBehavior volumeBehavior) : base(volumeBehavior)
    {
    }

    public override void Import(object sourceInfo)
    {
        var source = GetSource(sourceInfo);

        if (source.FilePath is null)
            throw new ArgumentNullException(nameof(source.FilePath));

        TryGetSymbol(source.FilePath);
        using var reader = new StreamReader(source.FilePath);
        using var csv = new CsvReader(reader, source.ReaderConfiguration ?? _defaultConfiguration);

        csv.Read();
        csv.ReadHeader();

        while (csv.Read())
        {
            ProcessRow(csv);
        }

        _data = _data.OrderBy(m => m.Date).ToList();

        _isInitialized = true;
    }

    public override async Task ImportAsync(object sourceInfo)
    {
        var source = GetSource(sourceInfo);

        if (source.StreamSource is null && source.FilePath is null)
            throw new ArgumentNullException($"{nameof(source.FilePath)} and {nameof(source.StreamSource)} are null");
        
        TryGetSymbol(source.FilePath);

        using var reader = source.StreamSource is null
            ? new StreamReader(source.FilePath)
            : new StreamReader(source.StreamSource);
        
        using var csv = new CsvReader(reader, source.ReaderConfiguration ?? _defaultConfiguration);

        await csv.ReadAsync();
        csv.ReadHeader();

        while (await csv.ReadAsync())
        {
            ProcessRow(csv);
        }

        _data = _data.OrderBy(m => m.Date).ToList();

        _isInitialized = true;
    }

    private void ProcessRow(IReaderRow csv)
    {
        var volume = VolumeBehavior switch
        {
            VolumeBehavior.IgnoreReading => null,
            VolumeBehavior.IgnoreException => csv.GetField<string>((int)CsvColumnMapping.Volume).ToDouble(csv.Configuration.CultureInfo, ignoreException: true),
            VolumeBehavior.Strict => csv.GetField<string>((int)CsvColumnMapping.Volume).ToDouble(csv.Configuration.CultureInfo),
            _ => throw new NotImplementedException(nameof(VolumeBehavior))
        };
        
        var newElement = new Ohlc(
            csv.GetField<double>((int)CsvColumnMapping.Open),
            csv.GetField<double>((int)CsvColumnMapping.High),
            csv.GetField<double>((int)CsvColumnMapping.Low),
            csv.GetField<double>((int)CsvColumnMapping.Close),
            volume,
            GetDate(csv.GetField<string>((int)CsvColumnMapping.Date)));

        UpdateDecimalCount(newElement.Close);

        _data.Add(newElement);
    }

    private void TryGetSymbol(string? fileName)
    {
        if (fileName is null)
        {
            return;
        }
        
        if (File.Exists(fileName))
        {
            Symbol = Path.GetFileNameWithoutExtension(fileName);
        }
    }

    private CsvSource GetSource(object sourceInfo) => sourceInfo switch
    {
        string filePath => new CsvSource(filePath)
        {
            ReaderConfiguration = new CsvConfiguration(new CultureInfo(DefaultLocale))
        },
        Stream stream => new CsvSource(stream)
        {
            ReaderConfiguration = new CsvConfiguration(new CultureInfo(DefaultLocale))
        },
        CsvSource csvSource => csvSource,
        _ => throw new ArgumentException(
            $"Expected type {typeof(string)} or {typeof(CsvSource)} for {nameof(sourceInfo)}")
    };
}