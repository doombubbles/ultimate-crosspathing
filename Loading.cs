using System.Collections;
using System.Linq;
using Assets.Scripts.Models.Towers;
using Assets.Scripts.Unity;
using BTD_Mod_Helper;
using BTD_Mod_Helper.Api;
using BTD_Mod_Helper.Extensions;
using MelonLoader;

namespace UltimateCrosspathing
{
    public class Loading : ModLoadTask
    {
        public override string DisplayName => "Ultimately Crosspathing...";

        public override IEnumerator Coroutine()
        {
            if (!UltimateCrosspathingMod.RegenerateTowers)
            {
                var loadInfos = GetContent<LoadInfo>().Where(info => info.Enabled).ToList();
                while (loadInfos.Any(info => !info.loaded))
                {
                    while (loadInfos.FirstOrDefault(info => !info.loaded && info.Loader.Loaded) is LoadInfo loadInfo)
                    {
                        yield return null;
                        var dummy = loadInfo.Loader.LoadResult();
                        yield return null;

                        var towers = dummy.behaviors
                            .Where(model => model != null)
                            .Select(model => model.TryCast<TowerModel>())
                            .ToList();
                        var towersByType = towers.GroupBy(model => model.baseId).ToList();
                        yield return null;

                        foreach (var towerModels in towersByType)
                        {
                            foreach (var towerModel in towerModels)
                            {
                                Towers.PostMerge(towerModel);
                                foreach (var crosspathingPatchMod in MelonHandler.Mods.OfType<CrosspathingPatchMod>())
                                {
                                    crosspathingPatchMod.Postmerge(towerModel, towerModel.baseId,
                                        towerModel.tiers[0],
                                        towerModel.tiers[1],
                                        towerModel.tiers[2]);
                                }
                            }

                            yield return null;
                        }

                        Game.instance.model.AddTowersToGame(towers);
                        yield return null;

                        foreach (var towerModels in towersByType)
                        {
                            foreach (var towerModel in towerModels)
                            {
                                Towers.AddUpgradeToPrevs(towerModel);
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

        public static void ExportTowers()
        {
            foreach (var info in GetContent<LoadInfo>().Where(info => info.Enabled))
            {
                info.Export();
            }

            ModHelper.Msg<UltimateCrosspathingMod>("Finished exporting!");
        }
    }
}