using System.Linq;
using Il2CppAssets.Scripts.Simulation.Towers.Behaviors;
using Il2CppAssets.Scripts.Unity.UI_New.InGame.TowerSelectionMenu;
using BTD_Mod_Helper.Api;
using HarmonyLib;

namespace UltimateCrosspathing
{
    [HarmonyPatch(typeof(UpgradeObject), nameof(UpgradeObject.CheckBlockedPath))]
    internal class UpgradeObject_CheckBlockedPath
    {
        [HarmonyPostfix]
        internal static void Postfix(UpgradeObject __instance, ref int __result)
        {
            if (LoadInfo.ShouldWork(__instance.tts.Def.baseId))
            {
                var tier = __instance.tier;
                var tiers = __instance.tts.Def.tiers;
                var sum = tiers.Sum();
                var remainingTiers = Settings.MaxTiers - sum;
                __result = tier + remainingTiers;
                if (__result > 5)
                {
                    __result = 5;
                }
            }
        }
    }

    [HarmonyPatch(typeof(TowerSelectionMenu), nameof(TowerSelectionMenu.IsUpgradePathClosed))]
    internal class TowerSelectionMenu_IsUpgradePathClosed
    {
        [HarmonyPostfix]
        internal static void Postfix(TowerSelectionMenu __instance, ref bool __result)
        {
            if (LoadInfo.ShouldWork(__instance.selectedTower.Def.baseId))
            {
                __result &= __instance.selectedTower.Def.tiers.Sum() >= Settings.MaxTiers;
            }
        }
    }

    [HarmonyPatch(typeof(Bank), nameof(Bank.Cash), MethodType.Setter)]
    internal class Bank_Cash
    {
        [HarmonyPostfix]
        internal static void Postfix(Bank __instance)
        {
            if (__instance.bankModel.autoCollect && __instance.Cash >= __instance.bankModel.capacity)
            {
                __instance.Collect();
            }
        }
    }
}