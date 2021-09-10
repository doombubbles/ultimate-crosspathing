using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Assets.Scripts.Models.Towers;
using Assets.Scripts.Unity;
using Assets.Scripts.Unity.UI_New.InGame.TowerSelectionMenu;
using Assets.Scripts.Unity.UI_New.Popups;
using Assets.Scripts.Utils;
using BTD_Mod_Helper;
using BTD_Mod_Helper.Api;
using BTD_Mod_Helper.Api.ModOptions;
using BTD_Mod_Helper.Extensions;
using HarmonyLib;
using MelonLoader;
using UltimateCrosspathing.Loading;
using UltimateCrosspathing.Merging;
using static Assets.Scripts.Models.Towers.TowerType;
using Environment = System.Environment;

[assembly: MelonInfo(typeof(UltimateCrosspathing.Main), "Ultimate Crosspathing", "1.1.0", "doombubbles")]
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

        //private static readonly ModSettingBool AlchemistEnabled = false;
        private static readonly ModSettingBool DruidEnabled = true;
        private static readonly ModSettingBool BananaFarmEnabled = true;
        private static readonly ModSettingBool SpikeFactoryEnabled = true;
        private static readonly ModSettingBool MonkeyVillageEnabled = true;
        private static readonly ModSettingBool EngineerMonkeyEnabled = true;

        public static readonly ModSettingBool RegenerateTowers = new ModSettingBool(false)
        {
            displayName = "DEBUG: Regenerate Towers"
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

        public static readonly int TowerBatchSize = 8;

        public static readonly ModSettingBool ExecuteOrder66 = new ModSettingBool(false)
        {
            displayName = "Execute Order 66"
        };

        public static int MaxTiers => ExecuteOrder66 ? 15 : 7;

        public static Dictionary<string, ModSettingBool> TowersEnabled = new Dictionary<string, ModSettingBool>
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
            { Druid, DruidEnabled },
            { BananaFarm, BananaFarmEnabled },
            { SpikeFactory, SpikeFactoryEnabled },
            { MonkeyVillage, MonkeyVillageEnabled },
            { EngineerMonkey, EngineerMonkeyEnabled },
        };

        public override void OnTitleScreen()
        {
            ExportTowerBytes.OnInitialized.Add(option =>
            {
                var buttonOption = (ButtonOption)option;
                buttonOption.ButtonText.text = "Export";
                buttonOption.Button.AddOnClick(() =>
                {
                    LoaderConverter.ExportTowers("towers.bytes", "TowersLoaderOriginal.cs");
                    LoaderConverter.ConvertLoader(Environment.CurrentDirectory + "\\TowersLoaderOriginal.cs",
                        Environment.CurrentDirectory + "\\TowersLoader.cs");
                });
            });

            ExportTowerJsons.OnInitialized.Add(option =>
            {
                var buttonOption = (ButtonOption)option;
                buttonOption.ButtonText.text = "Export";
                buttonOption.Button.AddOnClick(() =>
                {
                    if (LoadedTowerModels != null)
                    {
                        foreach (var towerModel in LoadedTowerModels)
                        {
                            var fileName = $"MergedTowers/{towerModel.baseId}/{towerModel.name}.json";
                            FileIOUtil.SaveObject(fileName, towerModel);
                        }
                    }


                    if (AsyncMerging.FinishedTowerModels != null)
                    {
                        foreach (var towerModel in AsyncMerging.FinishedTowerModels)
                        {
                            var fileName = $"MergedTowers/{towerModel.baseId}/{towerModel.name}.json";
                            FileIOUtil.SaveObject(fileName, towerModel);
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
                else
                {
                    AsyncMerging.GetNextTowerBatch();
                    AsyncMerging.AsyncCreateCrosspaths();
                }
            }
            else
            {
                MelonLogger.Msg("Loading Towers...");
                var resource = Assembly.GetManifestResourceStream("UltimateCrosspathing.towers.bytes");
                var memoryStream = new MemoryStream();
                resource?.CopyTo(memoryStream);

                var towerModelLoader = new TowersLoader();
                var dummy = towerModelLoader.Load(memoryStream.ToArray());
                MelonLogger.Msg("Finished Loading Towers!");
                LoadedTowerModels = dummy.behaviors.Select(model => model.Cast<TowerModel>()).ToArray();

                MelonLogger.Msg("Applying PostMerge Fixes");
                foreach (var model in LoadedTowerModels)
                {
                    CrosspathingPatchMod.DefaultPostmerge(model);
                    foreach (var crosspathingPatchMod in MelonHandler.Mods.OfType<CrosspathingPatchMod>())
                    {
                        crosspathingPatchMod.Postmerge(model, model.baseId, model.tiers[0], model.tiers[1],
                            model.tiers[2]);
                    }
                }

                MelonLogger.Msg("Adding Towers to Game");
                Game.instance.model.AddTowersToGame(LoadedTowerModels);
                MelonLogger.Msg("Adding Upgrade Paths");
                foreach (var towerModel in LoadedTowerModels)
                {
                    Towers.AddUpgradeToPrevs(towerModel);
                }

                MelonLogger.Msg("Saving Towers to JSON...");
                MelonLogger.Msg("All Done!");
            }
        }

        private static TowerModel[] LoadedTowerModels;

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
            if ((AsyncMerging.pass > AsyncMerging.finishedPass) && PopupScreen.instance != null)
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
                    : (AffectModdedTowers && __instance.selectedTower.Def.baseId != Alchemist))
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
                    : (AffectModdedTowers && __instance.tts.Def.baseId != Alchemist))
                {
                    var tier = __instance.tier;
                    var tiers = __instance.tts.Def.tiers;
                    var sum = tiers.Sum();
                    var remainingTiers = Main.MaxTiers - sum;
                    __result = tier + remainingTiers;
                    if (__result > 5)
                    {
                        __result = 5;
                    }
                }
            }
        }
    }
}