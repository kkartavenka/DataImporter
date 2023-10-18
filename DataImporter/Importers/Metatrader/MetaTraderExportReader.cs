using DataImporter.Classes;
using DataImporter.Models;

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

    public MetaTraderExportReader(VolumeBehavior ignoreVolume) : base(ignoreVolume)
    {
    }
}