using Assets.Scripts.Models.Towers;
using Assets.Scripts.Models.Towers.Projectiles.Behaviors;
using BTD_Mod_Helper.Api.Enums;
using BTD_Mod_Helper.Extensions;

namespace UltimateCrosspathing.PostMergeFixes;

public class FixBananas : PostMergeFix
{
    public override void Apply(TowerModel model)
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
}