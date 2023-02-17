using BTD_Mod_Helper.Api.Data;
using BTD_Mod_Helper.Api.ModOptions;

namespace UltimateCrosspathing;

public class Settings : ModSettings
{
    protected override int Order => 1;

    public static readonly ModSettingInt MaxTiers = new(15)
    {
        min = 0,
        max = 15,
        slider = true,
        description = "Controls the maximum number of upgrades that any one tower can have at once. " +
                      "Setting this all the way to 15 will enable the complete Ultimate Crosspathing experience!"
    };

#if DEBUG
    
    private static readonly ModSettingCategory DebugSettings = "Debug Settings";

    public static readonly ModSettingBool PostMergeRegenerate = new(false)
    {
        displayName = "DEBUG: Post-Merge While Generating",
        category = DebugSettings
    };

    private static readonly ModSettingButton ExportTowerBytes = new(LoadInfo.ExportTowers)
    {
        displayName = "DEBUG: Export Tower Bytes",
        buttonText = "Export",
        category = DebugSettings
    };
#endif
    
}