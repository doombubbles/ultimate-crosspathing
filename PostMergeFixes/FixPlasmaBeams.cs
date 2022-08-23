using Assets.Scripts.Models.Towers;
using Assets.Scripts.Models.Towers.Behaviors.Attack;
using Assets.Scripts.Models.Towers.Behaviors.Emissions;
using BTD_Mod_Helper.Api.Enums;
using BTD_Mod_Helper.Extensions;

namespace UltimateCrosspathing.PostMergeFixes;

public class FixPlasmaBeams : PostMergeFix
{
    public override void Apply(TowerModel model)
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
            var behaviors = Enumerable.ToList(model.behaviors);
            behaviors.RemoveAll(m => m.IsType<AttackModel>(out var attackModel) &&
                                     attackModel.weapons.Any(weaponModel =>
                                         weaponModel.emission.IsType<LineProjectileEmissionModel>()));
            behaviors.Add(lineProjectileAttacks[0]);
            model.behaviors = behaviors.ToIl2CppReferenceArray();
        }
    }
}