using System.Linq;
using Assets.Scripts.Unity.UI_New.Popups;
using BTD_Mod_Helper;
using BTD_Mod_Helper.Api;
using BTD_Mod_Helper.Extensions;
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

        private const string FailedLoadingMessage = "Ultimate Crosspathing failed to load. " +
                                                    "An update to the mod may be required, check the Mod Browser or the GitHub page for details.";


        public override void OnMainMenu()
        {
            if (!SuccessfullyLoaded)
            {
                TaskScheduler.ScheduleTask(
                    () => PopupScreen.instance.ShowOkPopup(FailedLoadingMessage),
                    () => PopupScreen.instance != null && !PopupScreen.instance.IsPopupActive()
                );
            }
        }


        public static void StarterMessage()
        {
            var enabled = ModContent.GetContent<LoadInfo>().Where(info => info.Enabled).Select(info => info.Name)
                .ToArray();
            var disabled = ModContent.GetContent<LoadInfo>().Where(info => !info.Enabled).Select(info => info.Name)
                .ToArray();

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
        }
    }
}