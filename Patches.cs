using Il2CppAssets.Scripts.Simulation.Towers.Behaviors;
using Il2CppAssets.Scripts.Unity.UI_New.InGame.TowerSelectionMenu;
using HarmonyLib;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Simulation.Towers.Behaviors.Attack;

namespace UltimateCrosspathing;

/// <summary>
/// Show correct upgrade pips for towers
/// </summary>
[HarmonyPatch(typeof(UpgradeObject), nameof(UpgradeObject.CheckBlockedPath))]
internal class UpgradeObject_CheckBlockedPath
{
    [HarmonyPostfix]
    internal static void Postfix(UpgradeObject __instance, ref int __result, ref bool __runOriginal)
    {
        if (__instance.tts == null || !LoadInfo.ShouldWork(__instance.tts.Def.baseId) || !__runOriginal) return;

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

/// <summary>
/// Show correct upgrade pips for powers pro
/// </summary>
[HarmonyPatch(typeof(PowerProUpgradeObject), nameof(PowerProUpgradeObject.CheckBlockedPath))]
internal static class PowerProUpgradeObject_CheckBlockedPath
{
    [HarmonyPostfix]
    internal static void Postfix(PowerProUpgradeObject __instance, ref int __result, ref bool __runOriginal)
    {
        if (__instance.tts == null || !LoadInfo.ShouldWork(__instance.tts.Def.baseId) || !__runOriginal) return;

        var tier = __instance.tier;
        var tiers = __instance.tts.Def.tiers;
        var sum = tiers.Sum();
        var remainingTiers = Settings.MaxTiersPowersPro - sum;
        __result = tier + remainingTiers;
        if (__result > 3)
        {
            __result = 3;
        }
    }
}

/// <summary>
/// Make sure paths aren't closed for towers
/// </summary>
[HarmonyPatch(typeof(TowerSelectionMenu), nameof(TowerSelectionMenu.IsUpgradePathClosed))]
internal class TowerSelectionMenu_IsUpgradePathClosed
{
    [HarmonyPostfix]
    internal static void Postfix(TowerSelectionMenu __instance, int path, ref bool __result, ref bool __runOriginal)
    {
        if (__instance.selectedTower == null || !__runOriginal) return;

        var towerModel = __instance.selectedTower.Def;
        var blockBeastHandler = towerModel.baseId == TowerType.BeastHandler &&
                                towerModel.tiers.Count(t => t > 0) >= 2 &&
                                towerModel.tiers[path] == 0;
        if (LoadInfo.ShouldWork(towerModel.baseId) && !blockBeastHandler)
        {
            __result &= towerModel.tiers.Sum() >= (__instance.powerProDetails.activeSelf
                                                       ? Settings.MaxTiersPowersPro
                                                       : Settings.MaxTiers);
        }
    }
}

/// <summary>
/// Make sure paths aren't closed for powers pro
/// </summary>
[HarmonyPatch(typeof(PowerProUpgradeObject), nameof(PowerProUpgradeObject.CheckLocked))]
internal static class PowerProUpgradeObject_CheckLocked
{
    [HarmonyPostfix]
    internal static void Postfix(PowerProUpgradeObject __instance, ref bool __runOriginal)
    {
        if (__instance.tts == null || !__runOriginal) return;

        var towerModel = __instance.tts.Def;
        if (LoadInfo.ShouldWork(towerModel.baseId))
        {
            var locked = __instance.locked.activeSelf;
            locked &= towerModel.tiers.Sum() >= Settings.MaxTiersPowersPro;
            __instance.locked.SetActive(locked);
        }
    }
}

/// <summary>
/// Fix bug with max tiers not being checked on certain UI updates to the tower selection menu
/// </summary>
[HarmonyPatch(typeof(UpgradeObject), nameof(UpgradeObject.LoadUpgrades))]
internal static class UpgradeObject_LoadUpgrades
{
    [HarmonyPostfix]
    internal static void Postfix(UpgradeObject __instance)
    {
        __instance.UpdateVisuals(__instance.path, false);
    }
}

/// <summary>
/// Auto collect banks quicker
/// </summary>
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

/// <summary>
/// Fix bug where previous target suppliers that were since destroyed would sometimes still stick around
/// </summary>
[HarmonyPatch(typeof(Attack), nameof(Attack.UpdateActiveTargetSupplier))]
internal static class Attack_UpdateActiveTargetSupplier
{
    [HarmonyPrefix]
    internal static void Prefix(Attack __instance)
    {
        if (__instance.activeTargetSupplier is {IsDestroyed: true})
        {
            __instance.activeTargetSupplier = null;
        }
    }
}