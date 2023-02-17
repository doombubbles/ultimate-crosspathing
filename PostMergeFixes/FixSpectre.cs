using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Models.Towers.Projectiles;
using Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors;
using BTD_Mod_Helper.Api.Enums;
using BTD_Mod_Helper.Extensions;

namespace UltimateCrosspathing.PostMergeFixes;

public class FixSpectre : PostMergeFix
{
    public override void Apply(TowerModel model)
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
}
