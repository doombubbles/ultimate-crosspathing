using System;
using BTD_Mod_Helper.Api.Commands;

namespace UltimateCrosspathing;

#if DEBUG
internal class ExportTowerBytesCommands : ModCommand<ExportCommand>
{
    public override string Command => "uc";
    public override string Help => "Export Ultimate Crosspathing tower bytes";
    
    public override bool Execute(ref string resultText)
    {
        try
        {
            LoadInfo.ExportTowers();
        }
        catch (Exception e)
        {
            ModHelper.Error<UltimateCrosspathingMod>(e);
        }
        
        return true;
    }

}
#endif