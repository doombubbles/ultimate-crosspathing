using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Assets.Scripts.Models.Towers.Upgrades;
using Assets.Scripts.Unity;
using Assets.Scripts.Unity.UI_New.InGame.TowerSelectionMenu;
using Assets.Scripts.Utils;
using BTD_Mod_Helper;
using BTD_Mod_Helper.Api;
using BTD_Mod_Helper.Api.ModOptions;
using BTD_Mod_Helper.Extensions;
using HarmonyLib;
using MelonLoader;
using static Assets.Scripts.Models.Towers.TowerType;

[assembly: MelonInfo(typeof(UltimateCrosspathing.Main), "Ultimate Crosspathing", "1.0.0", "doombubbles")]
[assembly: MelonGame("Ninja Kiwi", "BloonsTD6")]

namespace UltimateCrosspathing
{
    public class Main : BloonsTD6Mod
    {
        public static string GetVersion => ModContent.GetInstance<Main>().Info.Version;

        public override string MelonInfoCsURL =>
            "https://raw.githubusercontent.com/doombubbles/ultimate-crosspathing/main/Main.cs";

        public override string LatestURL =>
            "https://github.com/doombubbles/ultimate-crosspathing/raw/main/UltimateCrosspathing.dll";

        private static readonly ModSettingBool DartMonkeyEnabled = true;
        private static readonly ModSettingBool BoomerangMonkeyEnabled = true;
        private static readonly ModSettingBool BombShooterEnabled = true;
        private static readonly ModSettingBool TackShooterEnabled = true;
        private static readonly ModSettingBool IceMonkeyEnabled = true;
        private static readonly ModSettingBool GlueGunnerEnabled = true;
        private static readonly ModSettingBool SniperMonkeyEnabled = true;
        private static readonly ModSettingBool MonkeySubEnabled = true;
        private static readonly ModSettingBool MonkeyBuccaneerEnabled = true;
        private static readonly ModSettingBool MonkeyAceEnabled = true;
        private static readonly ModSettingBool HeliPilotEnabled = true;
        private static readonly ModSettingBool MortarMonkeyEnabled = true;
        private static readonly ModSettingBool DartlingGunnerEnabled = true;
        private static readonly ModSettingBool WizardMonkeyEnabled = true;
        private static readonly ModSettingBool SuperMonkeyEnabled = true;

        private static readonly ModSettingBool NinjaMonkeyEnabled = true;

        //private static readonly ModSettingBool AlchemistEnabled = false;
        private static readonly ModSettingBool DruidEnabled = true;
        private static readonly ModSettingBool BananaFarmEnabled = true;
        private static readonly ModSettingBool SpikeFactoryEnabled = true;
        private static readonly ModSettingBool MonkeyVillageEnabled = true;
        private static readonly ModSettingBool EngineerMonkeyEnabled = true;

        public static readonly ModSettingBool TryLoadingFromJSON = new ModSettingBool(true)
        {
            displayName = "Try Loading From JSONs"
        };

        private static readonly ModSettingBool DeleteStoredJSONs = new ModSettingBool(false)
        {
            IsButton = true,
            displayName = "Delete All Stored JSONs"
        };

        private static readonly ModSettingBool ExecuteOrder66 = new ModSettingBool(false)
        {
            displayName = "Execute Order 66"
        };

        public static int MaxTiers => ExecuteOrder66 ? 15 : 7;

        public override void OnTitleScreen()
        {
            DeleteStoredJSONs.OnInitialized.Add(option =>
            {
                var buttonOption = (ButtonOption)option;
                buttonOption.ButtonText.text = "Delete";
                buttonOption.Button.AddOnClick(() =>
                {
                    var dir = FileIOUtil.sandboxRoot + "MergedTowers/";
                    if (Directory.Exists(dir))
                    {
                        Directory.Delete(dir, true);
                        Directory.CreateDirectory(dir);
                    }
                });
            });

            foreach (var crosspathingPatchMod in MelonHandler.Mods.OfType<CrosspathingPatchMod>())
            {
                crosspathingPatchMod.ModifyPathPriorities(Towers.PathPriorities);
            }

            if (DartMonkeyEnabled)
            {
                Towers.CreateCrosspathsForTower(Game.instance.model.GetTower(DartMonkey));
            }

            if (BoomerangMonkeyEnabled)
            {
                Towers.CreateCrosspathsForTower(Game.instance.model.GetTower(BoomerangMonkey));
            }

            if (BombShooterEnabled)
            {
                Towers.CreateCrosspathsForTower(Game.instance.model.GetTower(BombShooter));
            }

            if (TackShooterEnabled)
            {
                Towers.CreateCrosspathsForTower(Game.instance.model.GetTower(TackShooter));
            }

            if (IceMonkeyEnabled)
            {
                Towers.CreateCrosspathsForTower(Game.instance.model.GetTower(IceMonkey));
            }

            if (GlueGunnerEnabled)
            {
                Towers.CreateCrosspathsForTower(Game.instance.model.GetTower(GlueGunner));
            }

            if (SniperMonkeyEnabled)
            {
                Towers.CreateCrosspathsForTower(Game.instance.model.GetTower(SniperMonkey));
            }

            if (MonkeySubEnabled)
            {
                Towers.CreateCrosspathsForTower(Game.instance.model.GetTower(MonkeySub));
            }

            if (MonkeyBuccaneerEnabled)
            {
                Towers.CreateCrosspathsForTower(Game.instance.model.GetTower(MonkeyBuccaneer));
            }

            if (MonkeyAceEnabled)
            {
                Towers.CreateCrosspathsForTower(Game.instance.model.GetTower(MonkeyAce));
            }

            if (HeliPilotEnabled)
            {
                Towers.CreateCrosspathsForTower(Game.instance.model.GetTower(HeliPilot));
            }

            if (MortarMonkeyEnabled)
            {
                Towers.CreateCrosspathsForTower(Game.instance.model.GetTower(MortarMonkey));
            }

            if (DartlingGunnerEnabled)
            {
                Towers.CreateCrosspathsForTower(Game.instance.model.GetTower(DartlingGunner));
            }

            if (WizardMonkeyEnabled)
            {
                Towers.CreateCrosspathsForTower(Game.instance.model.GetTower(WizardMonkey));
            }

            if (SuperMonkeyEnabled)
            {
                Towers.CreateCrosspathsForTower(Game.instance.model.GetTower(SuperMonkey));
            }

            if (NinjaMonkeyEnabled)
            {
                Towers.CreateCrosspathsForTower(Game.instance.model.GetTower(NinjaMonkey));
            }

            /*if (AlchemistEnabled)
            {
                Towers.CreateCrosspathsForTower(Game.instance.model.GetTower(Alchemist));
            }*/

            if (DruidEnabled)
            {
                Towers.CreateCrosspathsForTower(Game.instance.model.GetTower(Druid));
            }

            if (BananaFarmEnabled)
            {
                Towers.CreateCrosspathsForTower(Game.instance.model.GetTower(BananaFarm));
            }

            if (SpikeFactoryEnabled)
            {
                Towers.CreateCrosspathsForTower(Game.instance.model.GetTower(SpikeFactory));
            }

            if (MonkeyVillageEnabled)
            {
                Towers.CreateCrosspathsForTower(Game.instance.model.GetTower(MonkeyVillage));
            }

            if (EngineerMonkeyEnabled)
            {
                Towers.CreateCrosspathsForTower(Game.instance.model.GetTower(EngineerMonkey));
            }
        }

        [HarmonyPatch(typeof(TowerSelectionMenu), nameof(TowerSelectionMenu.IsUpgradePathClosed))]
        internal class TowerSelectionMenu_IsUpgradePathClosed
        {
            [HarmonyPostfix]
            internal static void Postfix(TowerSelectionMenu __instance, ref bool __result)
            {
                __result = false;
            }
        }

        [HarmonyPatch(typeof(TowerSelectionMenu), nameof(TowerSelectionMenu.UpgradeTower), typeof(UpgradeModel),
            typeof(int), typeof(float))]
        internal class TowerSelectionMenu_UpdateTower
        {
            [HarmonyPostfix]
            internal static void Postfix(TowerSelectionMenu __instance)
            {
                TowerSelectionMenu.instance.SelectionChanged(TowerSelectionMenu.instance.selectedTower);
            }
        }

        [HarmonyPatch(typeof(UpgradeObject), nameof(UpgradeObject.CheckBlockedPath))]
        internal class UpgradeObject_CheckBlockedPath
        {
            [HarmonyPostfix]
            internal static void Postfix(UpgradeObject __instance, ref int __result)
            {
                var tier = __instance.tier;
                var tiers = __instance.tts.Def.tiers;
                var sum = tiers.Sum();
                var remainingTiers = MaxTiers - sum;
                __result = tier + remainingTiers;
                if (__result > 5)
                {
                    __result = 5;
                }
            }
        }
    }
}