using Il2CppAssets.Scripts.Models.GenericBehaviors;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Attack;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Attack.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Emissions;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Emissions.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Weapons;
using Il2CppAssets.Scripts.Models.Towers.Weapons.Behaviors;
using BTD_Mod_Helper.Api.Enums;
using BTD_Mod_Helper.Extensions;

namespace UltimateCrosspathing.PostMergeFixes;

public class FixAircraftCarriers : PostMergeFix
{
    public override void Apply(TowerModel model)
    {
        if (model.appliedUpgrades.Contains(UpgradeType.AircraftCarrier))
        {
            if (model.appliedUpgrades.Contains(UpgradeType.MonkeyPirates))
            {
                model.GetAbility().GetDescendant<AttackModel>().RemoveBehavior<RotateToTargetModel>();
            }

            model.GetDescendants<WeaponModel>().ForEach(weaponModel =>
            {
                var createTowerModel = weaponModel.GetDescendant<CreateTowerModel>();
                var filter = weaponModel.GetDescendant<SubTowerFilterModel>();
                if (createTowerModel != null && filter != null)
                {
                    filter.baseSubTowerId = createTowerModel.tower.baseId;
                    filter.baseSubTowerIds = new[] {filter.baseSubTowerId};
                }
            });

            model.GetAttackModels().ForEach(attackModel =>
            {
                attackModel.GetDescendants<RotateToTargetModel>()
                    .ForEach(targetModel => targetModel.rotateTower = false);

                attackModel.GetDescendants<EmissionModel>().ForEach(emissionModel =>
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
                                 .GetItemsOfType<EmissionBehaviorModel,
                                     EmissionArcRotationOffTowerDirectionModel>())
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

                if (!attackModel.HasBehavior<DisplayModel>())
                {
                    attackModel.AddBehavior(new DisplayModel("DisplayModel_AttackDisplay",
                        ModContent.CreatePrefabReference(""), 0));
                }
            });
        }
    }
}
