#if DEBUG
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Models.Towers;
using Assets.Scripts.Unity;
using BTD_Mod_Helper;
using BTD_Mod_Helper.Api;
using BTD_Mod_Helper.Api.Helpers;
using BTD_Mod_Helper.Extensions;
using HarmonyLib;

namespace UltimateCrosspathing;

public class GenerateTask : ModLoadTask
{
    public static List<TowerModel> TowerModels { get; } = new();

    public override IEnumerator Coroutine()
    {
        UltimateCrosspathingMod.StarterMessage();

        var loadInfo = GetContent<LoadInfo>()
            .Where(info => info.Enabled)
            .Select(info => info.Name)
            .ToArray();

        while (true)
        {
            var merges = loadInfo
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

            if (UltimateCrosspathingMod.PostMergeRegenerate)
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