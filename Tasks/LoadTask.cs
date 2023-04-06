#if RELEASE
using System;
using System.Collections;
using System.Collections.Generic;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Unity;
using BTD_Mod_Helper.Extensions;

namespace UltimateCrosspathing.Tasks
{
    public class Loading : ModLoadTask
    {
        public override string DisplayName => "Ultimately Crosspathing...";

        public override bool ShowProgressBar => true;

        public override IEnumerator Coroutine()
        {
            var loadInfos = GetContent<LoadInfo>().Where(info => info.Enabled).ToList();

            var validInfosTowers = new Dictionary<LoadInfo, List<TowerModel>>();
            foreach (var loadInfo in loadInfos)
            {
                try
                {
                    var towers = loadInfo.Loader.LoadResult().behaviors
                        .Where(model => model != null)
                        .Select(model => model.TryCast<TowerModel>())
                        .ToList();
                    validInfosTowers[loadInfo] = towers;
                }
                catch (Exception e)
                {
                    ModHelper.Warning<UltimateCrosspathingMod>(e);
                    ModHelper.Warning<UltimateCrosspathingMod>($"Failed loading {loadInfo.Name}s :(");
                    loadInfo.loaded = false;
                }

                yield return null;
            }

            var i = 0;
            foreach (var (loadInfo, towers) in validInfosTowers)
            {
                Progress = i++ / (float) validInfosTowers.Count;
                // Apply all post merge fixes to towers
                try
                {
                    foreach (var towerModel in towers)
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

                // Actually add the new tower models to the game model
                Game.instance.model.AddTowersToGame(towers);
                yield return null;

                // Add the necessary upgrades on the towers below each one
                try
                {
                    foreach (var towerModel in towers)
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
                
                loadInfo.loaded = true;
                ModHelper.Msg<UltimateCrosspathingMod>($"Finished loading {loadInfo.Name}s!");
                Description = loadInfo.Name.Spaced() + "s";
            }

            Description = "Done!";
        }
    }
}
#endif
