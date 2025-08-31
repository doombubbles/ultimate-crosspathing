using System;
using BTD_Mod_Helper.Api.Components;
using BTD_Mod_Helper.Api.Data;
using BTD_Mod_Helper.Api.Enums;
using BTD_Mod_Helper.Api.ModOptions;
using UnityEngine;

namespace UltimateCrosspathing;

public class Settings : ModSettings
{
    protected override int Order => 1;

    public const int MaxTiersMin = 7;
    public const int MaxTiersMax = 15;

    public static readonly ModSettingInt MaxTiers = new(MaxTiersMax)
    {
        min = MaxTiersMin,
        max = MaxTiersMax,
        slider = true,
        description = "Controls the maximum number of upgrades that any one tower can have at once. " +
                      "Having this all the way to 15 will enable the complete Ultimate Crosspathing experience!"
    };

    public static readonly ModSettingInt MaxTiersPowersPro = new(9)
    {
        min = 3,
        max = 9,
        slider = true,
        description = "Controls the maximum number of upgrades that any one por power can have at once. " +
                      "Having this all the way to 9 will enable the complete Ultimate Crosspathing experience!"
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

    public static readonly ModSettingCategory IndividualTowers = new("Individual Towers")
    {
        modifyCategory = category =>
        {
            var enableAll = category.AddButton(
                new Info("Enable All", 1000, -100, 562, 200, anchor: new Vector2(0.5f, 1)),
                VanillaSprites.GreenBtnLong, new Action(() =>
                {
                    foreach (var loadInfo in GetContent<LoadInfo>())
                    {
                        loadInfo.Enabled = true;
                    }
                }));
            enableAll.LayoutElement.ignoreLayout = true;
            enableAll.AddText(new Info("Text", InfoPreset.FillParent), "Enable All", 80f);

            var disableAll = category.AddButton(
                new Info("Disable All", -1000, -100, 562, 200, anchor: new Vector2(0.5f, 1)),
                VanillaSprites.RedBtnLong, new Action(() =>
                {
                    foreach (var loadInfo in GetContent<LoadInfo>())
                    {
                        loadInfo.Enabled = false;
                    }
                }));
            disableAll.LayoutElement.ignoreLayout = true;
            disableAll.AddText(new Info("Text", InfoPreset.FillParent), "Disable All", 80f);
        }
    };
}