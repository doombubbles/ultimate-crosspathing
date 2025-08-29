#if DEBUG
using System;
using System.Collections;
using System.Collections.Generic;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Unity;
using BTD_Mod_Helper.Api.Helpers;
using BTD_Mod_Helper.Extensions;
using HarmonyLib;

namespace UltimateCrosspathing.Tasks;

public class GenerateTask : ModLoadTask
{
    public static List<TowerModel> TowerModels { get; } = [];

    public override string DisplayName => "Generating Crosspaths Ultimately...";

    public override IEnumerator Coroutine()
    {
        var enabled = GetContent<LoadInfo>().Where(info => info.Enabled).Select(info => info.Name).ToArray();
        var disabled = GetContent<LoadInfo>().Where(info => !info.Enabled).Select(info => info.Name).ToArray();

        if (enabled.Any())
        {
            ModHelper.Msg<UltimateCrosspathingMod>("Enabled Towers: ");
            ModHelper.Msg<UltimateCrosspathingMod>(string.Join(", ", enabled));

            ModHelper.Msg<UltimateCrosspathingMod>("");

            ModHelper.Msg<UltimateCrosspathingMod>("Disabled Towers: ");
            ModHelper.Msg<UltimateCrosspathingMod>(string.Join(", ", disabled));

            ModHelper.Msg<UltimateCrosspathingMod>("");

            ModHelper.Msg<UltimateCrosspathingMod>("Beginning Crosspath Creation");
        }

        var loadInfos = GetContent<LoadInfo>();

        foreach (var info in loadInfos)
        {
            info.loaded = true;
        }

        var enabledTowers = loadInfos
            .Where(info => info.Enabled)
            .ToArray();

        while (true)
        {
            var merges = enabledTowers
                .SelectMany(Towers.GetMergeInfo)
                .ToArray();

            if (!merges.Any())
            {
                // Continue until there's no more MergeInfo to be had
                break;
            }

            var towerModels = new TowerModel[merges.Length];

            for (var i = 0; i < merges.Length; i++)
            {
                yield return null;

                var mergeInfo = merges[i];
                try
                {
                    towerModels[i] = Towers.NewMerge(mergeInfo);

                    ModHelper.Msg<UltimateCrosspathingMod>(mergeInfo.SucessMessage);
                }
                catch (Exception e)
                {
                    ModHelper.Error<UltimateCrosspathingMod>(e);
                    break;
                }
            }

            if (Settings.PostMergeRegenerate)
            {
                ModHelper.Msg<UltimateCrosspathingMod>("Starting PostMerging");

                for (var i = 0; i < towerModels.Length; i++)
                {
                    if (i % 20 == 0)
                    {
                        yield return null;
                    }

                    var towerModel = towerModels[i];
                    try
                    {
                        Towers.PostMerge(towerModel);
                    }
                    catch (Exception e)
                    {
                        ModHelper.Error<UltimateCrosspathingMod>("Failed at postmerging " + e);
                    }
                }

                try
                {
                    towerModels.Do(Towers.PostMerge);
                }
                catch (Exception e)
                {
                    ModHelper.Error<UltimateCrosspathingMod>("Failed at postmerging " + e);
                }
            }

            for (var i = 0; i < towerModels.Length; i++)
            {
                if (i % 20 == 0)
                {
                    yield return null;
                }

                var towerModel = towerModels[i];
                GameModelExporter.Export(towerModel, $"MergedTowers/{towerModel.baseId}/{towerModel.name}.json");
            }

            Game.instance.model.AddTowersToGame(towerModels);

            foreach (var towerModel in towerModels)
            {
                Towers.AddUpgradeToPrevs(towerModel);
            }

            TowerModels.AddRange(towerModels);
        }
    }
}
#endif
