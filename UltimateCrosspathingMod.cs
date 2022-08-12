using System;
using System.Diagnostics;
using Assets.Scripts.Unity.UI_New.Popups;
using BTD_Mod_Helper;
using BTD_Mod_Helper.Api;
using MelonLoader;
using UltimateCrosspathing;

[assembly:
    MelonInfo(typeof(UltimateCrosspathingMod), ModHelperData.Name, ModHelperData.Version, ModHelperData.RepoOwner)]
[assembly: MelonGame("Ninja Kiwi", "BloonsTD6")]

namespace UltimateCrosspathing
{
    public class UltimateCrosspathingMod : BloonsTD6Mod
    {
        public static bool SuccessfullyLoaded { get; set; }

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

            var melonLoader55 = false;
            try
            {
                melonLoader55 = ModHelperData.CheckMelonLoader055();
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
            }
            else if (!melonLoader55)
            {
                TaskScheduler.ScheduleTask(
                    () => PopupScreen.instance.ShowPopup(PopupScreen.Placement.menuCenter, "Not On MelonLoader 0.5.5",
                        "Ultimate Crosspathing failed to load. Not On MelonLoader 0.5.5. Click ok to be taken to the page with info about it.",
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
            }
            else if (!SuccessfullyLoaded)
            {
                TaskScheduler.ScheduleTask(
                    () => PopupScreen.instance.ShowOkPopup(
                        "Ultimate Crosspathing failed to load, see the log for more details. An update to the mod or the Mod Helper may be required, check the Mod Browser or the GitHub page for details."),
                    () => PopupScreen.instance != null && !PopupScreen.instance.IsPopupActive()
                );
            }
        }
    }
}