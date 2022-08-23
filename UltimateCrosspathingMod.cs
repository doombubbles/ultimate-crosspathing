global using System.Linq;
global using BTD_Mod_Helper;
global using BTD_Mod_Helper.Api;
using System;
using System.Diagnostics;
using Assets.Scripts.Unity.UI_New.Popups;
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
            var modHelper3 = false;
            try
            {
                modHelper3 = ModHelperData.CheckModHelper3();
            }
            catch (Exception)
            {
            }

            if (!modHelper3)
            {
                TaskScheduler.ScheduleTask(
                    () => PopupScreen.instance.ShowPopup(PopupScreen.Placement.menuCenter, "Not On Mod Helper 3.0",
                        "Ultimate Crosspathing failed to load. You are not using Mod Helper 3.0. Click ok to be taken to the page with info about it.",
                        new Action(() =>
                        {
                            Process.Start(
                                new ProcessStartInfo(
                                    "https://github.com/gurrenm3/BTD-Mod-Helper/wiki/Mod-Helper-3.0-Alpha")
                                {
                                    UseShellExecute = true
                                });
                        }), "Ok", null, "Cancel", Popup.TransitionAnim.Scale),
                    () => PopupScreen.instance != null && !PopupScreen.instance.IsPopupActive()
                );
                return;
            }


            var failedTowers = ModContent.GetContent<LoadInfo>().Count(info => info.loaded != true);

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