global using System.Linq;
global using BTD_Mod_Helper;
global using BTD_Mod_Helper.Api;
using Il2CppAssets.Scripts.Unity.UI_New.Popups;
using MelonLoader;
using UltimateCrosspathing;

[assembly:
    MelonInfo(typeof(UltimateCrosspathingMod), ModHelperData.Name, ModHelperData.Version, ModHelperData.RepoOwner)]
[assembly: MelonGame("Ninja Kiwi", "BloonsTD6")]

namespace UltimateCrosspathing
{
    public class UltimateCrosspathingMod : BloonsTD6Mod
    {
        public override void OnMainMenu()
        {
            var failedTowers = ModContent.GetContent<LoadInfo>().Count(info => info.loaded != true && info.Enabled);

            if (failedTowers > 0)
            {
                TaskScheduler.ScheduleTask(() => PopupScreen.instance.ShowOkPopup(
                        $"{failedTowers} tower(s) failed to Ultimately Crosspath. " +
                        "An update to the mod or the Mod Helper may be required, check the Mod Browser or the GitHub page for details."),
                    () => PopupScreen.instance != null && !PopupScreen.instance.IsPopupActive()
                );
            }
        }
    }
}