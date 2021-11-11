using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Simulation.Objects;
using Assets.Scripts.Simulation.Towers.Behaviors;
using Assets.Scripts.Unity.UI_New.InGame.TowerSelectionMenu;
using Assets.Scripts.Unity.UI_New.Popups;
using Assets.Scripts.Utils;
using BTD_Mod_Helper;
using BTD_Mod_Helper.Api;
using BTD_Mod_Helper.Api.ModOptions;
using BTD_Mod_Helper.Extensions;
using HarmonyLib;
using MelonLoader;
using UltimateCrosspathing.Merging;
using static Assets.Scripts.Models.Towers.TowerType;

[assembly: MelonInfo(typeof(UltimateCrosspathing.Main), "Ultimate Crosspathing", "1.2.1", "doombubbles")]
[assembly: MelonGame("Ninja Kiwi", "BloonsTD6")]

namespace UltimateCrosspathing
{
    public class Main : BloonsTD6Mod
    {
        public override string MelonInfoCsURL =>
            "https://raw.githubusercontent.com/doombubbles/ultimate-crosspathing/main/Main.cs";

        public override string LatestURL =>
            "https://github.com/doombubbles/ultimate-crosspathing/raw/main/UltimateCrosspathing.dll";

        private static readonly ModSettingBool AffectModdedTowers = false;

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

        private static readonly ModSettingBool AlchemistEnabled = false;

        private static readonly ModSettingBool DruidEnabled = true;
        private static readonly ModSettingBool BananaFarmEnabled = true;
        private static readonly ModSettingBool SpikeFactoryEnabled = true;
        private static readonly ModSettingBool MonkeyVillageEnabled = true;
        private static readonly ModSettingBool EngineerMonkeyEnabled = true;

        private static readonly ModSettingBool RegenerateTowers = new ModSettingBool(false)
        {
            displayName = "DEBUG: Regenerate Towers"
        };

        public static readonly ModSettingBool PostMergeRegenerate = new ModSettingBool(true)
        {
            displayName = "DEBUG: PostMerge While Regenerating"
        };

        private static readonly ModSettingBool ExportTowerJsons = new ModSettingBool(false)
        {
            IsButton = true,
            displayName = "DEBUG: Export Tower JSONs"
        };

        private static readonly ModSettingBool ExportTowerBytes = new ModSettingBool(false)
        {
            IsButton = true,
            displayName = "DEBUG: Export Tower Bytes"
        };

        public const int TowerBatchSize = 8;

        private static readonly ModSettingBool ExecuteOrder66 = new ModSettingBool(false)
        {
            displayName = "Execute Order 66"
        };

        public static int MaxTiers => ExecuteOrder66 ? 15 : 7;

        public static readonly Dictionary<string, ModSettingBool> TowersEnabled = new Dictionary<string, ModSettingBool>
        {
            { DartMonkey, DartMonkeyEnabled },
            { BoomerangMonkey, BoomerangMonkeyEnabled },
            { BombShooter, BombShooterEnabled },
            { TackShooter, TackShooterEnabled },
            { IceMonkey, IceMonkeyEnabled },
            { GlueGunner, GlueGunnerEnabled },
            { SniperMonkey, SniperMonkeyEnabled },
            { MonkeySub, MonkeySubEnabled },
            { MonkeyBuccaneer, MonkeyBuccaneerEnabled },
            { MonkeyAce, MonkeyAceEnabled },
            { HeliPilot, HeliPilotEnabled },
            { MortarMonkey, MortarMonkeyEnabled },
            { DartlingGunner, DartlingGunnerEnabled },
            { WizardMonkey, WizardMonkeyEnabled },
            { SuperMonkey, SuperMonkeyEnabled },
            { NinjaMonkey, NinjaMonkeyEnabled },
            { Alchemist, AlchemistEnabled },
            { Druid, DruidEnabled },
            { BananaFarm, BananaFarmEnabled },
            { SpikeFactory, SpikeFactoryEnabled },
            { MonkeyVillage, MonkeyVillageEnabled },
            { EngineerMonkey, EngineerMonkeyEnabled }
        };

        public override void OnTitleScreen()
        {
            ExportTowerBytes.OnInitialized.Add(option =>
            {
                var buttonOption = (ButtonOption)option;
                buttonOption.ButtonText.text = "Export";
                buttonOption.Button.AddOnClick(() => { Loading.ExportTowers(); });
            });

            ExportTowerJsons.OnInitialized.Add(option =>
            {
                var buttonOption = (ButtonOption)option;
                buttonOption.ButtonText.text = "Export";
                buttonOption.Button.AddOnClick(() =>
                {
                    if (AsyncMerging.FinishedTowerModels != null)
                    {
                        foreach (var towerModel in AsyncMerging.FinishedTowerModels)
                        {
                            try
                            {
                                var fileName = $"MergedTowers/{towerModel.baseId}/{towerModel.name}.json";
                                FileIOUtil.SaveObject(fileName, towerModel);
                            }
                            catch (Exception)
                            {
                                // ignored
                            }
                        }
                    }
                });
            });

            AffectModdedTowers.OnInitialized.Add(option =>
            {
                AffectModdedTowers.OnValueChanged = new List<Action<bool>>
                {
                    b =>
                    {
                        if (b && PopupScreen.instance != null)
                        {
                            PopupScreen.instance.ShowOkPopup(
                                "CLARIFICATION: This setting doesn't cause algorithmic merging of modded towers. " +
                                "It just activates any existing Ultimate Crosspathing integration that mod authors added themselves.");
                        }
                    }
                };
            });

            foreach (var crosspathingPatchMod in MelonHandler.Mods.OfType<CrosspathingPatchMod>())
            {
                crosspathingPatchMod.ModifyPathPriorities(Towers.PathPriorities);
            }

            if (RegenerateTowers)
            {
                StarterMessage();
                if (TowerBatchSize == 0)
                {
                    foreach (var tower in TowersEnabled.Keys.Where(tower => TowersEnabled[tower]))
                    {
                        Towers.CreateCrosspathsForTower(tower);
                    }
                }

                AsyncMerging.GetNextTowerBatch();
                AsyncMerging.AsyncCreateCrosspaths();
            }
            else
            {
                MelonLogger.Msg("Loading Towers...");

                foreach (var (tower, enabled) in TowersEnabled)
                {
                    MelonLogger.Msg($"{tower}s loading...");
                    var resource = Assembly.GetManifestResourceStream($"UltimateCrosspathing.Bytes.{tower}s.bytes");
                    if (resource == null)
                    {
                        MelonLogger.Warning($"No bytes for {tower}!");
                        continue;
                    }

                    Loading.LoadTower(resource, tower);
                }


                MelonLogger.Msg("All Done!");
            }
        }

        /*public override void OnKeyDown(KeyCode keyCode)
        {
            if (keyCode == KeyCode.RightShift)
            {
                InGame.instance.AddCash(100000);
            }
        }*/

        public override void OnUpdate()
        {
            if (AsyncMerging.pass == AsyncMerging.middlePass && AsyncMerging.pass > AsyncMerging.finishedPass)
            {
                AsyncMerging.FinishCreatingCrosspaths();
                AsyncMerging.finishedPass++;
            }
        }

        public override void OnMainMenu()
        {
            if (AsyncMerging.pass > AsyncMerging.finishedPass && PopupScreen.instance != null)
            {
                PopupScreen.instance.ShowOkPopup(
                    "Don't go into a game just yet, Ultimate Crosspathing is still setting up in the background.");
            }
        }

        public static void ShowFinishedPopup()
        {
            if (PopupScreen.instance != null)
            {
                PopupScreen.instance.HideAllPopups();
                TaskScheduler.ScheduleTask(() =>
                    PopupScreen.instance.ShowOkPopup(
                        "Ultimate Crosspathing has now finished merging.\n" +
                        "Happy Crosspathing!"));
            }
        }


        private static void StarterMessage()
        {
            var enabled = TowersEnabled.Where(kvp => kvp.Value).Select(kvp => kvp.Key).ToArray();
            var disabled = TowersEnabled.Where(kvp => !kvp.Value).Select(kvp => kvp.Key).ToArray();

            if (enabled.Any())
            {
                if (ExecuteOrder66)
                {
                    MelonLogger.Warning(
                        "Executing Order 66. Full 15 upgrade crosspathing is even less polished and tested than 7. You've been warned!");
                    MelonLogger.Msg("");
                }

                MelonLogger.Msg("Enabled Towers: ");
                MelonLogger.Msg(string.Join(", ", enabled));

                MelonLogger.Msg("");

                MelonLogger.Msg("Disabled Towers: ");
                MelonLogger.Msg(string.Join(", ", disabled));

                MelonLogger.Msg("");

                MelonLogger.Msg("Beginning Crosspath Creation");
            }
        }


        [HarmonyPatch(typeof(TowerSelectionMenu), nameof(TowerSelectionMenu.IsUpgradePathClosed))]
        internal class TowerSelectionMenu_IsUpgradePathClosed
        {
            [HarmonyPostfix]
            internal static void Postfix(TowerSelectionMenu __instance, ref bool __result)
            {
                if (TowersEnabled.TryGetValue(__instance.selectedTower.Def.baseId, out var enabled)
                    ? (bool)enabled
                    : AffectModdedTowers && __instance.selectedTower.Def.baseId != Alchemist)
                {
                    __result = __instance.selectedTower.Def.tiers.Sum() >= MaxTiers;
                }
            }
        }

        /*[HarmonyPatch(typeof(TowerSelectionMenu), nameof(TowerSelectionMenu.UpgradeTower), typeof(UpgradeModel),
            typeof(int), typeof(float))]
        internal class TowerSelectionMenu_UpdateTower
        {
            [HarmonyPostfix]
            internal static void Postfix(TowerSelectionMenu __instance)
            {
                TowerSelectionMenu.instance.SelectionChanged(TowerSelectionMenu.instance.selectedTower);
            }
        }*/


        [HarmonyPatch(typeof(UpgradeObject), nameof(UpgradeObject.CheckBlockedPath))]
        internal class UpgradeObject_CheckBlockedPath
        {
            [HarmonyPostfix]
            internal static void Postfix(UpgradeObject __instance, ref int __result)
            {
                if (TowersEnabled.TryGetValue(__instance.tts.Def.baseId, out var enabled)
                    ? (bool)enabled
                    : AffectModdedTowers && __instance.tts.Def.baseId != Alchemist)
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
}