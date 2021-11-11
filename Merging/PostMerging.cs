using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Models.GenericBehaviors;
using Assets.Scripts.Models.Towers;
using Assets.Scripts.Models.Towers.Behaviors;
using Assets.Scripts.Models.Towers.Behaviors.Abilities.Behaviors;
using Assets.Scripts.Models.Towers.Behaviors.Attack;
using Assets.Scripts.Models.Towers.Behaviors.Attack.Behaviors;
using Assets.Scripts.Models.Towers.Behaviors.Emissions;
using Assets.Scripts.Models.Towers.Behaviors.Emissions.Behaviors;
using Assets.Scripts.Models.Towers.Filters;
using Assets.Scripts.Models.Towers.Projectiles;
using Assets.Scripts.Models.Towers.Projectiles.Behaviors;
using Assets.Scripts.Models.Towers.Weapons;
using Assets.Scripts.Models.Towers.Weapons.Behaviors;
using BTD_Mod_Helper.Api.Enums;
using BTD_Mod_Helper.Extensions;
using Il2CppSystem;
using MelonLoader;
using UnhollowerBaseLib;

namespace UltimateCrosspathing.Merging
{
    public class PostMerging
    {
        public static void FixHomingProjectiles(TowerModel model)
        {
            foreach (var projectileModel in model.GetDescendants<ProjectileModel>().ToList()
                .Where(projectileModel => projectileModel.GetBehavior<RetargetOnContactModel>() != null))
            {
                projectileModel.RemoveBehavior<FollowPathModel>();
            }
        }

        public static void FixSmallRanges(TowerModel model)
        {
            // TODO: are there any cases where this shouldn't be the case?
            var attackModels = model.GetAttackModels();
            foreach (var attackModel in attackModels.Where(attackModel => attackModel.range < model.range))
            {
                attackModel.range = model.range;
            }
        }

        public static void FixCannonShips(TowerModel model)
        {
            if (model.appliedUpgrades.Contains(UpgradeType.CannonShip))
            {
                if (model.appliedUpgrades.Contains(UpgradeType.AircraftCarrier))
                {
                    {
                        //TODO fix the attack angles without crashing
                    }
                }

                if (model.appliedUpgrades.Contains(UpgradeType.Destroyer)) // TODO apply rate buffs to all weapons
                {
                    model.GetWeapon(4).Rate /= 5f;
                    model.GetWeapon(5).Rate /= 5f;
                }
            }
        }

        public static void FixSpectre(TowerModel model)
        {
            if (model.appliedUpgrades.Contains(UpgradeType.NevaMissTargeting))
            {
                var trackTargetModel = model.GetDescendant<TrackTargetModel>();
                model.GetAttackModels().ForEach(attackModel =>
                {
                    attackModel.GetDescendants<ProjectileModel>().ForEach(projectileModel =>
                    {
                        if (projectileModel.HasBehavior<TravelStraitModel>())
                        {
                            var travelStraitModel = projectileModel.GetBehavior<TravelStraitModel>();
                            if (!projectileModel.HasBehavior<TrackTargetModel>())
                            {
                                var targetModel = trackTargetModel.Duplicate();
                                projectileModel.AddBehavior(targetModel);
                            }

                            projectileModel.GetBehavior<TrackTargetModel>().TurnRate = travelStraitModel.Speed * 2;
                        }
                    });
                });
            }
        }

        public static void FixBananas(TowerModel model)
        {
            if (model.appliedUpgrades.Contains(UpgradeType.Marketplace) ||
                model.appliedUpgrades.Contains(UpgradeType.MonkeyBank)) // TODO smartly leave this out in MergeArray
            {
                model.GetWeapon().projectile.RemoveBehaviors<PickupModel>();
                model.GetWeapon().projectile.RemoveBehaviors<ArriveAtTargetModel>();
                model.GetWeapon().projectile.RemoveBehaviors<ScaleProjectileModel>();
                if (model.GetWeapon().projectile.GetBehavior<AgeModel>() is AgeModel ageModel)
                {
                    ageModel.Lifespan = 0;
                }

                if (model.GetDescendant<CreateTextEffectModel>() is CreateTextEffectModel createTextEffectModel)
                {
                    createTextEffectModel.useTowerPosition = true;
                }
            }
        }

        public static void FixPlasmaBeams(TowerModel model)
        {
            if (model.appliedUpgrades.Contains(UpgradeType.PlasmaAccelerator))
            {
                model.GetWeapon().projectile.radius = 2;
                //model.GetWeapon().projectile.RemoveBehavior<TravelStraightSlowdownModel>();
                //model.GetWeapon().projectile.RemoveBehavior<KnockbackModel>();
            }

            var lineProjectileAttacks = model.GetAttackModels().Where(attackModel =>
                    attackModel.weapons.Any(weaponModel => weaponModel.emission.IsType<LineProjectileEmissionModel>()))
                .ToList();
            if (lineProjectileAttacks.Count > 1)
            {
                var behaviors = model.behaviors.ToList();
                behaviors.RemoveAll(m => m.IsType<AttackModel>(out var attackModel) &&
                                         attackModel.weapons.Any(weaponModel =>
                                             weaponModel.emission.IsType<LineProjectileEmissionModel>()));
                behaviors.Add(lineProjectileAttacks[0]);
                model.behaviors = behaviors.ToIl2CppReferenceArray();
            }
        }

        public static void FixTemples(TowerModel model)
        {
            if (model.appliedUpgrades.Contains(UpgradeType.SunTemple))
            {
                foreach (var attackModel in model.GetAttackModels())
                {
                    var rotateToTargetModel = attackModel.GetBehavior<RotateToTargetModel>();
                    if (rotateToTargetModel != null)
                    {
                        rotateToTargetModel.rotateTower = false;
                    }

                    attackModel.RemoveBehaviors<RotateToMiddleOfTargetsModel>();
                }
            }
        }

        public static void FixIceMonkeyRange(TowerModel model)
        {
            if (model.GetDescendant<SlowBloonsZoneModel>().IsType<SlowBloonsZoneModel>(out var slow))
            {
                slow.zoneRadius = model.range + 5;
            }
        }

        public static void FixGlueGunnerOverriding(TowerModel model)
        {
            if (model.appliedUpgrades.Contains(UpgradeType.MOABGlue))
            {
                model.GetDescendants<ProjectileModel>().ForEach(projectileModel =>
                {
                    var behaviors = projectileModel.behaviors.ToList();
                    behaviors.RemoveAll(m => m.name == "SlowModifierForTagModel_");
                    projectileModel.behaviors = behaviors.ToIl2CppReferenceArray();
                });

                var lifeSpan = 0f;
                model.GetDescendants<SlowForBloonModel>().ForEach(m => { lifeSpan = Math.Max(lifeSpan, m.Lifespan); });
                model.GetDescendants<SlowModel>().ForEach(m => { lifeSpan = Math.Max(lifeSpan, m.Lifespan); });
                model.GetDescendants<AddBehaviorToBloonModel>().ForEach(m =>
                {
                    lifeSpan = Math.Max(lifeSpan, m.lifespan);
                });

                model.GetDescendants<SlowForBloonModel>().ForEach(m => { m.Lifespan = lifeSpan; });
                model.GetDescendants<SlowModel>().ForEach(m => { m.Lifespan = lifeSpan; });
                model.GetDescendants<AddBehaviorToBloonModel>().ForEach(m => { m.lifespan = lifeSpan; });

                model.GetDescendants<AttackFilterModel>().ForEach(filterModel =>
                {
                    var models = filterModel.filters.ToList();
                    models.RemoveAll(m => m.IsType<FilterOutTagModel>());
                    filterModel.filters = models.ToIl2CppReferenceArray();
                });

                model.GetDescendants<ProjectileFilterModel>().ForEach(filterModel =>
                {
                    var models = filterModel.filters.ToList();
                    models.RemoveAll(m => m.IsType<FilterOutTagModel>());
                    filterModel.filters = models.ToIl2CppReferenceArray();
                });
            }
        }


        public static void FixBehaviorNames(TowerModel model)
        {
            var behaviors = model.behaviors.ToList();
            for (var i = 0; i < behaviors.Count; i++)
            {
                var behavior = behaviors[i];
                if (behaviors.Take(i).Any(b => b.name == behavior.name))
                {
                    behavior.name += i;
                }
            }
        }

        public static void FixAircraftCarriers(TowerModel model)
        {
            if (model.appliedUpgrades.Contains(UpgradeType.AircraftCarrier))
            {
                model.GetDescendants<SubTowerFilterModel>().ForEach(filterModel =>
                {
                    if (!filterModel.baseSubTowerIds.Contains(filterModel.baseSubTowerId))
                    {
                        filterModel.baseSubTowerId = filterModel.baseSubTowerIds[0];
                    }
                });

                model.GetDescendants<RotateToTargetModel>().ForEach(targetModel => targetModel.rotateTower = false);

                model.GetDescendants<EmissionModel>().ForEach(emissionModel =>
                {
                    if (emissionModel.behaviors == null)
                    {
                        return;
                    }

                    var behaviors = emissionModel.behaviors.ToList();

                    foreach (var emission in emissionModel.behaviors
                        .GetItemsOfType<EmissionBehaviorModel, EmissionRotationOffTowerDirectionModel>())
                    {
                        behaviors.RemoveAll(behaviorModel => behaviorModel.name == emission.name);
                        emissionModel.RemoveChildDependant(emission);
                        var behavior = new EmissionRotationOffDisplayModel("EmissionRotationOffDisplayModel_",
                            emission.offsetRotation);
                        behaviors.Add(behavior);
                        emissionModel.AddChildDependant(behavior);
                    }

                    foreach (var emission in emissionModel.behaviors
                        .GetItemsOfType<EmissionBehaviorModel, EmissionArcRotationOffTowerDirectionModel>())
                    {
                        behaviors.RemoveAll(behaviorModel => behaviorModel.name == emission.name);
                        emissionModel.RemoveChildDependant(emission);
                        var behavior =
                            new EmissionArcRotationOffDisplayDirectionModel(
                                "EmissionArcRotationOffDisplayDirectionModel_", emission.offsetRotation);
                        behaviors.Add(behavior);
                        emissionModel.AddChildDependant(behavior);
                    }

                    emissionModel.behaviors = behaviors.ToIl2CppReferenceArray();
                });

                model.GetAttackModels().ForEach(attackModel =>
                {
                    if (!attackModel.HasBehavior<DisplayModel>())
                    {
                        attackModel.AddBehavior(new DisplayModel("DisplayModel_AttackDisplay", "", 0));
                    }
                });
            }
        }

        public static void FixAbilities(TowerModel model)
        {
            foreach (var ability in model.GetAbilites().Where(abilityModel =>
                abilityModel.displayName == "Supply Drop" || abilityModel.displayName == "Bomb Blitz"))
            {
                var activateAttackModel = ability.GetBehavior<ActivateAttackModel>();
                activateAttackModel.isOneShot = true;
            }

            if (model.appliedUpgrades.Contains(UpgradeType.EliteSniper))
            {
                model.RemoveBehavior<TargetSupplierSupportModel>();
            }
        }

        public static void FixAutoCollecting(TowerModel model)
        {
            if (model.appliedUpgrades.Contains(UpgradeType.BananaSalvage))
            {
                model.GetDescendants<BankModel>().ForEach(bankModel => { bankModel.autoCollect = true; });
            }
        }

        public static void FixLifespans(TowerModel model)
        {
            model.GetDescendants<AgeModel>().ForEach(ageModel =>
            {
                ageModel.Lifespan = System.Math.Max(ageModel.Lifespan, ageModel.lifespanFrames / 60f);
            });
        }

        public static void FixSpikeStorm(TowerModel model)
        {
            if (model.appliedUpgrades.Contains(UpgradeType.SpikeStorm))
            {
                var realProjectile = model.GetWeapon().projectile;
                model.GetAbility().GetBehavior<ActivateAttackModel>().attacks.ForEach(attackModel =>
                {
                    var projectileModel = attackModel.weapons[0].projectile;
                    var ageRandomModel = projectileModel.GetBehavior<AgeRandomModel>().Duplicate();

                    var newProjectile = realProjectile.Duplicate();
                    newProjectile.RemoveBehaviors<AgeModel>();
                    newProjectile.AddBehavior(ageRandomModel);

                    attackModel.weapons[0].projectile = newProjectile;
                });
            }
        }

        public static void FixClusterMauling(TowerModel model)
        {
            if (model.appliedUpgrades.Contains(UpgradeType.MOABMauler))
            {
                var damageModifierForTagModels = model.GetDescendants<DamageModifierForTagModel>().ToList();
                var moabage = damageModifierForTagModels.FirstOrDefault(m => m.tag == "Moabs");
                var ceramage = damageModifierForTagModels.FirstOrDefault(m => m.tag == "Ceramic");

                foreach (var projectileModel in model.GetDescendants<ProjectileModel>().ToList()
                    .Where(p => p.id == "Explosion"))
                {
                    projectileModel.RemoveBehaviors<DamageModifierForTagModel>();
                    if (moabage != null)
                    {
                        projectileModel.AddBehavior(moabage.Duplicate());
                    }

                    if (ceramage != null)
                    {
                        projectileModel.AddBehavior(ceramage.Duplicate());
                    }
                }
            }
        }
    }
}