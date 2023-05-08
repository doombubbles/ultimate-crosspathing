global using System.Linq;
global using BTD_Mod_Helper;
global using BTD_Mod_Helper.Api;
using System;
using BTD_Mod_Helper.Api.Helpers;
using Il2CppAssets.Scripts.Unity.UI_New.Popups;
using MelonLoader;
using UltimateCrosspathing;

[assembly:
    MelonInfo(typeof(UltimateCrosspathingMod), ModHelperData.Name, ModHelperData.Version, ModHelperData.RepoOwner)]
[assembly: MelonGame("Ninja Kiwi", "BloonsTD6")]
[assembly: MelonPriority(-500)]

namespace UltimateCrosspathing
{
    public class UltimateCrosspathingMod : BloonsTD6Mod
    {
        private const string HelpUrl = "https://github.com/doombubbles/ultimate-crosspathing/blob/main/HELP.md";

        public override void OnMainMenu()
        {
            var failedTowers = ModContent.GetContent<LoadInfo>().Count(info => info.loaded != true && info.Enabled);

            if (failedTowers > 0)
            {
                TaskScheduler.ScheduleTask(() => PopupScreen.instance.ShowPopup(PopupScreen.Placement.menuCenter,
                        "Crosspathing Issue",
                        $"{failedTowers} tower(s) failed to Ultimately Crosspath. " +
                        "An update to the mod, Mod Helper, or MelonLoader may be required. Press 'Help' to be taken to the common issues page.",
                        new Action(() => ProcessHelper.OpenURL(HelpUrl)), "Help", null, "Cancel",
                        Popup.TransitionAnim.Scale),
                    () => PopupScreen.instance != null && !PopupScreen.instance.IsPopupActive()
                );
            }
        }
    }
}