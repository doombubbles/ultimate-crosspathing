#if RELEASE
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Models.Towers;
using Assets.Scripts.Unity;
using BTD_Mod_Helper;
using BTD_Mod_Helper.Api;
using BTD_Mod_Helper.Extensions;

namespace UltimateCrosspathing.Tasks
{
    public class Loading : ModLoadTask
    {
        public override string DisplayName => "Ultimately Crosspathing...";

        public override IEnumerator Coroutine()
        {
            var loadInfos = GetContent<LoadInfo>().Where(info => info.Enabled).ToList();
            while (loadInfos.Any(info => info.loaded == null))
            {
                while (loadInfos.FirstOrDefault(info => info.loaded == null && info.Loader.Loaded) is LoadInfo loadInfo)
                {
                    yield return null;

                    var dummy = loadInfo.Loader.LoadResult();
                    yield return null;

                    IEnumerable<TowerModel> towers;
                    IEnumerable<IEnumerable<TowerModel>> towersByType;
                    try
                    {
                        towers = dummy.behaviors
                            .Where(model => model != null)
                            .Select(model => model.TryCast<TowerModel>())
                            .ToList();
                        towersByType = towers.GroupBy(model => model.baseId).ToList();
                    }
                    catch (Exception e)
                    {
                        ModHelper.Warning<UltimateCrosspathingMod>(e);
                        ModHelper.Warning<UltimateCrosspathingMod>($"Failed loading {loadInfo.Name}s :(");
                        loadInfo.loaded = false;
                        continue;
                    }

                    yield return null;

                    foreach (var towerModels in towersByType)
                    {
                        try
                        {
                            foreach (var towerModel in towerModels)
                            {
                                Towers.PostMerge(towerModel);
                            }
                        }
                        catch (Exception e)
                        {
                            ModHelper.Warning<UltimateCrosspathingMod>(e);
                            ModHelper.Warning<UltimateCrosspathingMod>($"Failed loading {loadInfo.Name}s :(");
                            loadInfo.loaded = false;
                            continue;
                        }

                        yield return null;
                    }

                    Game.instance.model.AddTowersToGame(towers);
                    yield return null;

                    foreach (var towerModels in towersByType)
                    {
                        try
                        {
                            foreach (var towerModel in towerModels)
                            {
                                Towers.AddUpgradeToPrevs(towerModel);
                            }
                        }
                        catch (Exception e)
                        {
                            ModHelper.Warning<UltimateCrosspathingMod>(e);
                            ModHelper.Warning<UltimateCrosspathingMod>($"Failed loading {loadInfo.Name}s :(");
                            loadInfo.loaded = false;
                            continue;
                        }

                        yield return null;
                    }

                    loadInfo.loaded = true;
                    ModHelper.Msg<UltimateCrosspathingMod>($"Finished loading {loadInfo.Name}s!");
                }

                yield return null;
            }
        }
    }
}
#endif