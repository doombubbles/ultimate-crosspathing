using System;
using BTD_Mod_Helper.Api.Commands;

namespace UltimateCrosspathing;

#if DEBUG
internal class GenerateByteLoadersCommand : ModCommand<GenerateCommand>
{
    public override string Command => "uc";
    public override string Help => "Generate Ultimate Crosspathing byte loaders";

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