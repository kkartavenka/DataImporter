using System.Globalization;
using System.Net;
using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using DataImporter.Classes;
using DataImporter.Models;

namespace DataImporter.Importers.InvestingDotCom;

public class InvestingDotComImporter: InvestingDotDomBase
{
    private const string DefaultLocale = "en_US";
    
    private readonly HttpClientHandler _handler = new()
    {
        AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
    };
    private static HttpClient? _httpClient;
    private readonly object _lock = new();

    public InvestingDotComImporter(bool ignoreVolume): base(ignoreVolume)
    {
        SetupHttpClient();
    }
    public override void Import(object sourceInfo)
    {
        if (sourceInfo is not OnlineRequestInfo onlineRequestInfo)
        {
            throw new ArgumentException($"Expected type {typeof(OnlineRequestInfo)} for {nameof(sourceInfo)}");
        }

        Process(onlineRequestInfo).Wait();
    }

    public override async Task ImportAsync(object sourceInfo)
    {
        if (sourceInfo is not OnlineRequestInfo onlineRequestInfo)
        {
            throw new ArgumentException($"Expected type {typeof(OnlineRequestInfo)} for {nameof(sourceInfo)}");
        }

        await Process(onlineRequestInfo);
    }

    private void SetupHttpClient()
    {
        lock (_lock)
        {
            if (_httpClient is not null)
            {
                return;
            }
            
            _httpClient = new HttpClient(_handler);
            _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_7) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/117.0.0.0 Safari/537.36 Edg/117.0.2045.55");
            _httpClient.DefaultRequestHeaders.Add("Domain-Id","www");
            _httpClient.DefaultRequestHeaders.Add("Accept-Encoding","gzip, deflate");
            _httpClient.DefaultRequestHeaders.Add("Connection","keep-alive");
        }
    }
    
    private async Task Process(OnlineRequestInfo onlineRequestInfo)
    {
        if (_httpClient is null)
        {
            throw new ArgumentNullException(nameof(_httpClient));
        }

        var config = Configuration.Default
            .WithDefaultLoader();

        using var response = await _httpClient.GetAsync(onlineRequestInfo.Uri);
        var browsingContext = BrowsingContext.New(config);
        var document = await browsingContext.OpenAsync(x => x.Content(response.Content.ReadAsStream()));

        var locale = GetLocale(document);
        Symbol = GetName(document);
        GetDataTable(document, locale);
        _isInitialized = true;
    }

    private string? GetName(IDocument? document)
    {
        if (document is null)
        {
            throw new ArgumentNullException(nameof(document));
        }
        
        return document.QuerySelector("h1.text-xl")?.InnerHtml;
    }

    private void GetDataTable(IDocument? document, CultureInfo cultureInfo)
    {
        if (document is null)
        {
            throw new ArgumentNullException(nameof(document));
        }

        var table = document.QuerySelector("table.w-full")?.QuerySelectorAll("tr").ToList();
        if (table is null)
        {
            throw new ArgumentNullException($"{nameof(table)} is null. Probably issues with parsing");
        }
        
        foreach (var row in table)
        {
            var columns = row.QuerySelectorAll("td").ToList();
            if (columns.Count != 7)
            {
                continue;
            }
            
            var newElement = new Ohlc(
                open: columns[(int)WebColumnMapping.Open].InnerHtml.ToDouble(cultureInfo),
                high: columns[(int)WebColumnMapping.High].InnerHtml.ToDouble(cultureInfo),
                low: columns[(int)WebColumnMapping.Low].InnerHtml.ToDouble(cultureInfo),
                close: columns[(int)WebColumnMapping.Close].InnerHtml.ToDouble(cultureInfo),
                date: GetDate(columns[(int)WebColumnMapping.Date].QuerySelector("time")?.InnerHtml),
                volume: IgnoreVolume? null:columns[(int)WebColumnMapping.Volume].InnerHtml.ToDouble(cultureInfo));
            
            UpdateDecimalCount(newElement.Close);

            _data.Add(newElement);
        }

        if (!_data.Any())
        {
            throw new Exception("The data was not retrieved");
        }
    }

    private CultureInfo GetLocale(IDocument? document)
    {
        if (document is null)
        {
            throw new ArgumentNullException(nameof(document));
        }

        var locale = document
            .QuerySelectorAll("meta")
            .FirstOrDefault(m => m.Attributes.Any(x => x.Value == "og:locale"));

        if (locale is not IHtmlMetaElement meta)
        {
            return CultureInfo.GetCultureInfo(DefaultLocale);
        }

        return CultureInfo.GetCultureInfo(meta.Content ?? DefaultLocale);
    }
}