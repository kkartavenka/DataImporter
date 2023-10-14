using DataImporter.Models;

namespace DataImporter.Classes;

public class MetaTraderExportReader: BaseClass
{
    public override void Import(object sourceInfo)
    {
        throw new NotImplementedException();
    }

    public MetaTraderExportReader(bool ignoreVolume) : base(ignoreVolume)
    {
    }
}