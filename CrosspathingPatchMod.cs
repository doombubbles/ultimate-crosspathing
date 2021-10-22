using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Models.Towers;
using Assets.Scripts.Models.Towers.Behaviors.Attack;
using Assets.Scripts.Models.Towers.Behaviors.Attack.Behaviors;
using Assets.Scripts.Models.Towers.Behaviors.Emissions;
using Assets.Scripts.Models.Towers.Behaviors.Emissions.Behaviors;
using Assets.Scripts.Models.Towers.Projectiles;
using Assets.Scripts.Models.Towers.Projectiles.Behaviors;
using Assets.Scripts.Models.Towers.Weapons;
using BTD_Mod_Helper;
using BTD_Mod_Helper.Api.Enums;
using BTD_Mod_Helper.Extensions;
using HarmonyLib;
using MelonLoader;
using UnhollowerBaseLib;

namespace UltimateCrosspathing
{
    public abstract class CrosspathingPatchMod : BloonsTD6Mod
    {
        public abstract void Postmerge(TowerModel towerModel, string baseId, int topPath, int middlePath,
            int bottomPath);

        public virtual void ModifyPathPriorities(Dictionary<string, (int, int, int)> pathPriorities)
        {
        }

        internal static void DefaultPostmerge(TowerModel model)
        {
            foreach (var projectileModel in model.GetDescendants<ProjectileModel>().ToList()
                .Where(projectileModel => projectileModel.GetBehavior<RetargetOnContactModel>() != null))
            {
                projectileModel.RemoveBehavior<FollowPathModel>();
            }

            // TODO: are there any cases where this shouldn't be the case?
            var attackModels = model.GetAttackModels();
            foreach (var attackModel in attackModels.Where(attackModel => attackModel.range < model.range))
            {
                attackModel.range = model.range;
            }

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

            if (model.appliedUpgrades.Contains(UpgradeType.Spectre))
            {
                model.GetAttackModel().weapons = new Il2CppReferenceArray<WeaponModel>(0);
            }

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

            if (model.appliedUpgrades.Contains(UpgradeType.PlasmaAccelerator))
            {
                model.GetWeapon().projectile.radius = 2;
                //model.GetWeapon().projectile.RemoveBehavior<TravelStraightSlowdownModel>();
                //model.GetWeapon().projectile.RemoveBehavior<KnockbackModel>();
            }


            if (model.appliedUpgrades.Contains(UpgradeType.SunTemple))
            {
                foreach (var attackModel in attackModels)
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
    }
}