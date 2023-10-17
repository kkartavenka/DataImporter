using DataImporter.Classes;

namespace DataImporter.Importers.Metatrader;

public class MetaTraderExportReader: BaseClass
{
    public override void Import(object sourceInfo)
    {
        throw new NotImplementedException();
    }

    public override Task ImportAsync(object sourceInfo)
    {
        throw new NotImplementedException();
    }

    public MetaTraderExportReader(bool ignoreVolume) : base(ignoreVolume)
    {
    }
}