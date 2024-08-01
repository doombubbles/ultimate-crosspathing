using System;
using System.Collections.Generic;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Models.Towers.Upgrades;
using Il2CppAssets.Scripts.Unity;
using BTD_Mod_Helper.Extensions;
using HarmonyLib;
using Il2CppInterop.Runtime;
using UltimateCrosspathing.Merging;
using UltimateCrosspathing.PostMergeFixes;


namespace UltimateCrosspathing
{
    public class Towers
    {
        /// <summary>
        /// Determines which paths takes precedence when determining for visuals and certain other behavior
        /// </summary>
        public static readonly Dictionary<string, (int, int, int)> PathPriorities =
            new()
            {
                { TowerType.DartMonkey, (0, 2, 1) }, // This means Top Path > Bottom Path > Middle Path
                { TowerType.TackShooter, (0, 1, 2) },
                { TowerType.BoomerangMonkey, (1, 2, 0) },
                { TowerType.BombShooter, (2, 1, 0) },
                { TowerType.MonkeyBuccaneer, (0, 1, 2) },
                { TowerType.NinjaMonkey, (0, 2, 1) },
                { TowerType.SniperMonkey, (1, 2, 0) },
                { TowerType.DartlingGunner, (2, 0, 1) },
                { TowerType.IceMonkey, (2, 0, 1) },
                { TowerType.SuperMonkey, (0, 1, 2) },
                { TowerType.GlueGunner, (2, 0, 1) },
                { TowerType.WizardMonkey, (0, 2, 1) },
                { TowerType.BananaFarm, (1, 2, 0) },
                { TowerType.SpikeFactory, (2, 1, 0) },
                { TowerType.Mermonkey, (1, 2, 0) }
            };

        /// <summary>
        /// Some probably overcomplicated logic to determine which different tiers of tower model should be merged together to create the new one
        /// </summary>
        /// <param name="baseId">Tower's id</param>
        /// <param name="top">Top path tier</param>
        /// <param name="mid">Mid Path tier</param>
        /// <param name="bot">Bot path tier</param>
        /// <param name="leftTiers">The outputed tiers for the left merge tower</param>
        /// <param name="rightTiers">The outputed tiers for the right merge tower</param>
        /// <returns>Whether this new tower should even be created</returns>
        private static bool GetTiersForMerging(string baseId, int top, int mid, int bot, out int[] leftTiers,
            out int[] rightTiers)
        {
            var (high, medium, low) = PathPriorities.ContainsKey(baseId)
                ? PathPriorities[baseId]
                : PathPriorities[TowerType.DartMonkey];
            var tiers = new[] { top, mid, bot };
            leftTiers = new[] { top, mid, bot };
            rightTiers = new[] { top, mid, bot };
            var orderedTiers = tiers.OrderBy(num => -num).ToArray();

            if (tiers.All(i => i == orderedTiers[0]))
            {
                leftTiers[low] = 0;
                rightTiers[medium] = 0;
            }
            else if (orderedTiers[0] == orderedTiers[1] && orderedTiers[0] >= 3) // Towers with two 3rd tier upgrades
            {
                if (tiers[low] == orderedTiers[2]) // If the lowest priority path isn't one of the 3rd tiers
                {
                    leftTiers[medium] = Math.Min(2, leftTiers[medium]);
                    rightTiers[high] = Math.Min(2, rightTiers[high]);
                }
                else if (tiers[medium] == orderedTiers[2]) // If the middle priority path isn't one of the 3rd tiers
                {
                    leftTiers[low] = Math.Min(2, leftTiers[low]);
                    rightTiers[high] = Math.Min(2, rightTiers[high]);
                }
                else if (tiers[high] == orderedTiers[2]) // If the highest priority path isn't one of the 3rd tiers
                {
                    leftTiers[low] = Math.Min(2, leftTiers[low]);
                    rightTiers[medium] = Math.Min(2, rightTiers[medium]);
                }
            }
            else if (tiers.Count(t => t > 0) == 3) // Towers with at least one upgrade in all paths
            {
                if (orderedTiers[1] == orderedTiers[2])
                {
                    if (orderedTiers[0] == tiers[high])
                    {
                        leftTiers[low] = 0;
                        rightTiers[medium] = 0;
                    }
                    else if (orderedTiers[0] == tiers[medium])
                    {
                        leftTiers[low] = 0;
                        rightTiers[high] = 0;
                    }
                    else if (orderedTiers[0] == tiers[low])
                    {
                        leftTiers[medium] = 0;
                        rightTiers[high] = 0;
                    }
                }
                else
                {
                    for (var i = 0; i < 3; i++)
                    {
                        if (tiers[i] == orderedTiers[2])
                        {
                            leftTiers[i] = 0;
                            break;
                        }
                    }

                    for (var i = 2; i >= 0; i--)
                    {
                        if (tiers[i] == orderedTiers[1])
                        {
                            rightTiers[i] = 0;
                            break;
                        }
                    }
                }
            }
            else // All other towers
            {
                for (var i = 0; i < 3; i++)
                {
                    if (tiers[i] == orderedTiers[1])
                    {
                        leftTiers[i] = Math.Min(2, leftTiers[i]);
                        break;
                    }
                }

                for (var i = 2; i >= 0; i--)
                {
                    if (tiers[i] == orderedTiers[0])
                    {
                        rightTiers[i] = Math.Min(2, rightTiers[i]);
                        break;
                    }
                }
            }

            return true;
        }

        public static IEnumerable<MergeInfo> GetMergeInfo(string baseId)
        {
            for (var i = 0; i <= 5; i++)
            {
                for (var j = 0; j <= 5; j++)
                {
                    for (var k = 0; k <= 5; k++)
                    {
                        var newTowerName = $"{baseId}-{i}{j}{k}";
                        if (Game.instance.model.GetTowerWithName(newTowerName) == null &&
                            i + j + k <= Settings.MaxTiers && i + j + k > 0)
                        {
                            if (!GetTiersForMerging(baseId, i, j, k, out var leftTiers, out var rightTiers))
                            {
                                // Don't make the tower
                                continue;
                            }

                            var leftName = $"{baseId}-{leftTiers[0]}{leftTiers[1]}{leftTiers[2]}";
                            var leftTowerModel = Game.instance.model.GetTowerWithName(leftName);

                            var rightName = $"{baseId}-{rightTiers[0]}{rightTiers[1]}{rightTiers[2]}";
                            var rightTowerModel = Game.instance.model.GetTowerWithName(rightName);

                            if (leftTowerModel != null && rightTowerModel != null)
                            {
                                var result = leftTowerModel.Duplicate();
                                var left = leftTowerModel.Duplicate();
                                var right = rightTowerModel.Duplicate();
                                var commonAncestorTiers = new[]
                                {
                                    Math.Min(left.tiers[0], right.tiers[0]),
                                    Math.Min(left.tiers[1], right.tiers[1]),
                                    Math.Min(left.tiers[2], right.tiers[2])
                                };
                                var commonAncestorName = $"{result.baseId}-{commonAncestorTiers.Printed()}";
                                var commonAncestor = Game.instance.model.GetTowerWithName(commonAncestorName);
                                if (commonAncestor != null)
                                {
                                    ModHelper.Msg<UltimateCrosspathingMod>($"Creating {newTowerName} from {leftName} and {rightName}");
                                    yield return new MergeInfo(result, left, right, commonAncestor);
                                }
                            }
                        }
                    }
                }
            }
        }

        public static TowerModel NewMerge(MergeInfo mergeInfo)
        {
            var (result, left, right, commonAncestor) = mergeInfo;
            for (var i = 0; i < 3; i++)
            {
                result.tiers[i] = Math.Max(left.tiers[i], right.tiers[i]);
            }

            result.tier = Math.Max(left.tier, right.tier);
            result.name = $"{result.baseId}-{result.tiers[0]}{result.tiers[1]}{result.tiers[2]}";
            MergeUpgrades(result, left, right);
            var range = Il2CppType.Of<TowerModel>().GetField("range");
            DeepMerging.MergeField(range, result, right, commonAncestor);

            var mods = Il2CppType.Of<TowerModel>().GetField("mods");
            DeepMerging.MergeField(mods, result, right, commonAncestor);

            var behaviorsInfo = Il2CppType.Of<TowerModel>().GetField("behaviors");
            DeepMerging.MergeField(behaviorsInfo, result, right, commonAncestor);

            return result;
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
                    Math.Max(int.Parse(tierString[2].ToString()), towerModel.tiers[2])
                };
                if (upgradeTiers.Sum() <= Settings.MaxTiers)
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
        public static void AddUpgradeToPrevs(TowerModel towerModel)
        {
            for (var i = 0; i < 3; i++)
            {
                var tier = towerModel.tiers[i];
                if (tier > 0)
                {
                    var newTiers = towerModel.tiers.ToArray();
                    newTiers[i]--;

                    var prevName = $"{towerModel.baseId}-{newTiers.Printed()}";
                    var prevTowerModel = Game.instance.model.GetTowerWithName(prevName);

                    if (prevTowerModel == null)
                    {
                        continue;
                    }

                    var upgradeNeeded = towerModel.appliedUpgrades.FirstOrDefault(upgrade =>
                        !prevTowerModel.appliedUpgrades.Contains(upgrade));

                    if (upgradeNeeded == default)
                    {
                        continue;
                    }

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
        public static void PostMerge(TowerModel model)
        {
            foreach (var postMergeFix in ModContent.GetContent<PostMergeFix>())
            {
                postMergeFix.Apply(model);
            }
        }
    }
}
