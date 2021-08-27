using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Models;
using Assets.Scripts.Models.Towers;
using Assets.Scripts.Models.Towers.Behaviors;
using Assets.Scripts.Models.Towers.Behaviors.Attack;
using Assets.Scripts.Models.Towers.Behaviors.Emissions;
using Assets.Scripts.Models.Towers.Behaviors.Emissions.Behaviors;
using Assets.Scripts.Models.Towers.Mods;
using Assets.Scripts.Models.Towers.Projectiles;
using Assets.Scripts.Models.Towers.Projectiles.Behaviors;
using Assets.Scripts.Models.Towers.Upgrades;
using Assets.Scripts.Models.Towers.Weapons;
using Assets.Scripts.Unity;
using Assets.Scripts.Utils;
using BTD_Mod_Helper;
using BTD_Mod_Helper.Extensions;
using Harmony;
using HarmonyLib;
using MelonLoader;
using UnhollowerBaseLib;
using UnhollowerRuntimeLib;
using Math = System.Math;
using static Assets.Scripts.Models.Towers.TowerType;
using Il2CppType = UnhollowerRuntimeLib.Il2CppType;


namespace UltimateCrosspathing
{
    public class Towers
    {
        /// <summary>
        /// Determines which paths takes precedence when determining for visuals and certain other behavior
        /// </summary>
        private static readonly Dictionary<string, (int, int, int)> PATH_PRIORITIES =
            new Dictionary<string, (int, int, int)>
            {
                { DartMonkey, (0, 2, 1) }, // This means Top Path > Bottom Path > Middle Path
                { TackShooter, (0, 1, 2) },
                { BoomerangMonkey, (1, 2, 0) },
                { BombShooter, (1, 2, 0) },
                { MonkeyBuccaneer, (0, 1, 2) },
                { NinjaMonkey, (0, 2, 1) },
                { SniperMonkey, (0, 2, 1) },
                { DartlingGunner, (0, 1, 2) },
                { IceMonkey, (2, 0, 1) },
                { SuperMonkey, (1, 0, 2) }
            };

        /// <summary>
        /// Some probably overcomplicated logic to determine which different tiers of tower model should be merged together to create the new one
        /// </summary>
        /// <param name="baseId">Tower's id</param>
        /// <param name="i">Top path tier</param>
        /// <param name="j">Mid Path tier</param>
        /// <param name="k">Bot path tier</param>
        /// <param name="aboveThree">Whether to allow cross pathing involving third tier or higher towers</param>
        /// <param name="leftTiers">The outputed tiers for the left merge tower</param>
        /// <param name="rightTiers">The outputed tiers for the right merge tower</param>
        /// <returns>Whether this new tower should even be created</returns>
        private static bool GetTiersForMerging(string baseId, int i, int j, int k, bool aboveThree, out int[] leftTiers,
            out int[] rightTiers)
        {
            var (highest, middle, lowest) = PATH_PRIORITIES.ContainsKey(baseId)
                ? PATH_PRIORITIES[baseId]
                : PATH_PRIORITIES[DartMonkey];
            var tiers = new[] { i, j, k };
            leftTiers = new[] { i, j, k };
            rightTiers = new[] { i, j, k };
            var orderedTiers = tiers.OrderBy(num => -num).ToArray();
            if (orderedTiers[1] > 2 && !aboveThree) return false;

            if (orderedTiers[0] == orderedTiers[1] && orderedTiers[0] == 3) // Towers with two 3rd tier upgrades
            {
                if (tiers[lowest] == orderedTiers[2]) // If the lowest priority path isn't one of the 3rd tiers
                {
                    leftTiers[middle] = Math.Min(2, leftTiers[middle]);
                    rightTiers[highest] = Math.Min(2, rightTiers[highest]);
                }
                else if (tiers[middle] == orderedTiers[2]) // If the middle priority path isn't one of the 3rd tiers
                {
                    leftTiers[lowest] = Math.Min(2, leftTiers[lowest]);
                    rightTiers[highest] = Math.Min(2, rightTiers[highest]);
                }
                else if (tiers[highest] == orderedTiers[2]) // If the highest priority path isn't one of the 3rd tiers
                {
                    leftTiers[lowest] = Math.Min(2, leftTiers[lowest]);
                    rightTiers[middle] = Math.Min(2, rightTiers[middle]);
                }
            }
            else if (tiers.Count(t => t > 0) == 3) // Towers with at least one upgrade in all paths
            {
                for (var l = 0; l < 3; l++)
                {
                    if (tiers[l] == orderedTiers[1])
                    {
                        leftTiers[l] = 0;
                        break;
                    }
                }

                for (var l = 2; l >= 0; l--)
                {
                    if (tiers[l] == orderedTiers[0])
                    {
                        rightTiers[l] = 0;
                        break;
                    }
                }
            }
            else // All other towers
            {
                for (var l = 0; l < 3; l++)
                {
                    if (tiers[l] == orderedTiers[1])
                    {
                        leftTiers[l] = Math.Min(2, leftTiers[l]);
                        break;
                    }
                }

                for (var l = 2; l >= 0; l--)
                {
                    if (tiers[l] == orderedTiers[0])
                    {
                        rightTiers[l] = Math.Min(2, rightTiers[l]);
                        break;
                    }
                }
            }

            return true;
        }

        public static void CreateCrosspathsForTower(TowerModel baseTower, bool aboveThree = true)
        {
            var baseId = baseTower.baseId;
            // var (highest, middle, lowest) = PATH_PRIORITIES[baseId];
            for (var i = 0; i <= 5; i++)
            {
                for (var j = 0; j <= 5; j++)
                {
                    for (var k = 0; k <= 5; k++)
                    {
                        var newTowerName = $"{baseId}-{i}{j}{k}";
                        if (Game.instance.model.towers.All(model => model.name != newTowerName) && i + j + k <= 7 &&
                            i + j + k > 0)
                        {
                            if (!GetTiersForMerging(baseId, i, j, k, aboveThree, out var leftTiers, out var rightTiers))
                            {
                                // Don't make the tower
                                continue;
                            }

                            try
                            {
                                var leftName = $"{baseId}-{leftTiers[0]}{leftTiers[1]}{leftTiers[2]}";
                                var leftTowerModel = Game.instance.model.GetTowerFromId(leftName).Duplicate();

                                try
                                {
                                    var rightName = $"{baseId}-{rightTiers[0]}{rightTiers[1]}{rightTiers[2]}";
                                    var rightTowerModel = Game.instance.model.GetTowerFromId(rightName).Duplicate();


                                    try
                                    {
                                        var newTowerModel = Merge(leftTowerModel, rightTowerModel);
                                        MelonLogger.Msg(
                                            $"Successfully made {newTowerName} from {leftName} and {rightName}");
                                        Game.instance.model.AddTowerToGame(newTowerModel);
                                    }
                                    catch (Exception e)
                                    {
                                        if (Main.DebugSaveTowerJSON)
                                        {
                                            FileIOUtil.SaveFile($"MergedTowers/{leftTowerModel.baseId}/{newTowerName}.json", e.ToString());
                                        }

                                        MelonLogger.Msg($"Failed making {newTowerName}");
                                    }
                                }
                                catch (Exception)
                                {
                                    MelonLogger.Msg(
                                        $"Failed finding right tower for {(rightTiers[0], rightTiers[1], rightTiers[2])}");
                                }
                            }
                            catch (Exception)
                            {
                                MelonLogger.Msg(
                                    $"Failed finding left tower for {(leftTiers[0], leftTiers[1], leftTiers[2])}");
                            }
                        }
                    }
                }
            }
        }

        public static TowerModel Merge(TowerModel first, TowerModel second)
        {
            var towerModel = first.Duplicate();
            for (var i = 0; i < 3; i++)
            {
                towerModel.tiers[i] = Math.Max(first.tiers[i], second.tiers[i]);
            }

            towerModel.tier = Math.Max(first.tier, second.tier);
            towerModel.name = $"{towerModel.baseId}-{towerModel.tiers[0]}{towerModel.tiers[1]}{towerModel.tiers[2]}";

            TowerModel loaded = null;
            /*try
            {
                loaded = FileIOUtil.LoadObject<TowerModel>($"MergedTowers/{towerModel.name}-{Main.VERSION}.json");
                if (loaded != null)
                {
                    towerModel = loaded;
                }
            }
            catch (Exception e)
            {
                FileIOUtil.SaveObject($"MergedTowers/{towerModel.name}.txt", e.ToString());
            }*/

            if (loaded == null)
            {
                MergeUpgrades(towerModel, first, second);
                var commonAncestor = Game.instance.model.GetTower(towerModel.baseId,
                    Math.Min(first.tiers[0], second.tiers[0]),
                    Math.Min(first.tiers[1], second.tiers[1]),
                    Math.Min(first.tiers[2], second.tiers[2]));

                var behaviorsInfo = Il2CppType.Of<TowerModel>().GetField("behaviors");
                DeepMerging.MergeField(behaviorsInfo, towerModel, second, commonAncestor);
                
                var range = Il2CppType.Of<TowerModel>().GetField("range");
                DeepMerging.MergeField(range, towerModel, second, commonAncestor);

                var mods = Il2CppType.Of<TowerModel>().GetField("mods");
                DeepMerging.MergeField(mods, towerModel, second, commonAncestor);
                
                PostMerge(towerModel);
                if (Main.DebugSaveTowerJSON)
                {
                    FileIOUtil.SaveObject($"MergedTowers/{towerModel.baseId}/{towerModel.name}.json", towerModel);
                }
            }

            AddUpgradeToPrevs(towerModel);

            return towerModel;
        }


        /// <summary>
        /// Gets the correct purchasable upgrades for a tower
        /// </summary>
        /// <param name="towerModel">The merged tower</param>
        /// <param name="first">It's left parent</param>
        /// <param name="second">It's right parent</param>
        private static void MergeUpgrades(TowerModel towerModel, TowerModel first, TowerModel second)
        {
            foreach (var secondAppliedUpgrade in second.appliedUpgrades)
            {
                if (!towerModel.appliedUpgrades.Contains(secondAppliedUpgrade))
                {
                    towerModel.appliedUpgrades = towerModel.appliedUpgrades.AddItem(secondAppliedUpgrade).ToArray();
                }
            }

            var allUpgrades = new Dictionary<string, UpgradePathModel>();
            foreach (var upgradePathModel in first.upgrades.Concat(second.upgrades))
            {
                if (!towerModel.appliedUpgrades.Contains(upgradePathModel.upgrade))
                {
                    allUpgrades[upgradePathModel.upgrade] = upgradePathModel.MemberwiseClone().Cast<UpgradePathModel>();
                }
            }

            foreach (var upgradePathModel in allUpgrades.Values)
            {
                var tierString = upgradePathModel.tower.Split('-')[1];
                var upgradeTiers = new[]
                {
                    Math.Max(int.Parse(tierString[0].ToString()), towerModel.tiers[0]),
                    Math.Max(int.Parse(tierString[1].ToString()), towerModel.tiers[1]),
                    Math.Max(int.Parse(tierString[2].ToString()), towerModel.tiers[2]),
                };
                if (upgradeTiers.Sum() <= 7)
                {
                    upgradePathModel.tower = $"{towerModel.baseId}-{upgradeTiers[0]}{upgradeTiers[1]}{upgradeTiers[2]}";
                    //upgradePathModel.numberOfPathsUsed = upgradeTiers.Count(i => i > 0);
                }
                else
                {
                    upgradePathModel.tower = null;
                }
            }

            towerModel.upgrades = allUpgrades.Values.Where(u => u.tower != null).ToIl2CppReferenceArray();
        }


        /// <summary>
        /// Gives the correct upgrade for a new crosspathed tower to all the previous towers that ought to have it
        /// </summary>
        /// <param name="towerModel">The new crosspathed tower</param>
        private static void AddUpgradeToPrevs(TowerModel towerModel)
        {
            for (var i = 0; i < 3; i++)
            {
                var tier = towerModel.tiers[i];
                if (tier > 0)
                {
                    var newTiers = towerModel.tiers.ToArray();
                    newTiers[i]--;

                    var prevTowerModel =
                        Game.instance.model.GetTower(towerModel.baseId, newTiers[0], newTiers[1], newTiers[2]);

                    if (prevTowerModel == null) continue;

                    var upgradeNeeded = towerModel.appliedUpgrades.FirstOrDefault(upgrade =>
                        !prevTowerModel.appliedUpgrades.Contains(upgrade));

                    if (upgradeNeeded == default) continue;

                    if (prevTowerModel.upgrades.All(upm => upm.upgrade != upgradeNeeded))
                    {
                        var newUpgrade = new UpgradePathModel(upgradeNeeded, towerModel.name);
                        prevTowerModel.upgrades = prevTowerModel.upgrades.AddTo(newUpgrade);
                    }
                }
            }
        }


        /// <summary>
        /// Apply final fixes to a new merged tower, after all the algorithmic merging is finished
        /// </summary>
        /// <param name="model"></param>
        private static void PostMerge(TowerModel model)
        {
            foreach (var projectileModel in model.GetDescendants<ProjectileModel>().ToList())
            {
                if (projectileModel.GetBehavior<RetargetOnContactModel>() != null)
                {
                    projectileModel.RemoveBehavior<FollowPathModel>();
                }
            }

            // TODO: are there any cases where this shouldn't be the case
            foreach (var attackModel in model.GetAttackModels())
            {
                if (attackModel.range < model.range)
                {
                    attackModel.range = model.range;
                }
            }

            if (model.appliedUpgrades.Contains("Buccaneer-Cannon Ship"))
            {
                if (model.appliedUpgrades.Contains("Buccaneer-Aircraft Carrier"))
                {
                    foreach (var emissionModel in model.GetDescendants<EmissionModel>().ToList())
                    {
                        if (emissionModel.behaviors != null)
                        {
                            emissionModel.behaviors =
                                emissionModel.behaviors
                                    .RemoveItemsOfType<EmissionBehaviorModel, EmissionRotationOffTowerDirectionModel>();
                            emissionModel.behaviors =
                                emissionModel.behaviors
                                    .RemoveItemsOfType<EmissionBehaviorModel,
                                        EmissionArcRotationOffTowerDirectionModel>();
                        }
                    }

                    model.GetWeapon(4).emission.behaviors = model.GetWeapon(4).emission.behaviors
                        .AddTo(new EmissionRotationOffDisplayModel("", 90));
                    model.GetWeapon(5).emission.behaviors = model.GetWeapon(5).emission.behaviors
                        .AddTo(new EmissionRotationOffDisplayModel("", -90));
                }

                if (model.appliedUpgrades.Contains("Buccaneer-Destroyer")) // TODO apply rate buffs to all weapons
                {
                    model.GetWeapon(4).Rate /= 5f;
                    model.GetWeapon(5).Rate /= 5f;
                }
                
                
            }

            if (model.appliedUpgrades.Contains("Spectre"))
            {
                model.GetAttackModel().weapons = new Il2CppReferenceArray<WeaponModel>(0);
            }

            if (model.appliedUpgrades.Contains("Robo Monkey"))  // TODO more logic about adding duplicate attacks
            {
                model.GetWeapon(1).projectile = model.GetWeapon(0).projectile.Duplicate();
                model.GetWeapon(1).emission = model.GetWeapon(0).emission.Duplicate();
            }

            if (model.appliedUpgrades.Contains("Marketplace") || model.appliedUpgrades.Contains("Monkey Bank")) // TODO smartly leave this out in MergeArray
            {
                model.GetWeapon().projectile.RemoveBehaviors<PickupModel>();
                model.GetWeapon().projectile.RemoveBehaviors<ArriveAtTargetModel>();
                model.GetDescendant<CreateTextEffectModel>().useTowerPosition = false;
            }

            if (model.appliedUpgrades.Contains("Plasma Accelerator"))
            {
                model.GetWeapon().projectile.radius = 2;
                //model.GetWeapon().projectile.RemoveBehavior<TravelStraightSlowdownModel>();
                //model.GetWeapon().projectile.RemoveBehavior<KnockbackModel>();
            }
            
            foreach (var crosspathingPatchMod in MelonHandler.Mods.OfType<CrosspathingPatchMod>())
            {
                crosspathingPatchMod.Postmerge(model, model.baseId, model.tiers[0], model.tiers[1], model.tiers[2]);
            }
        }
    }
}