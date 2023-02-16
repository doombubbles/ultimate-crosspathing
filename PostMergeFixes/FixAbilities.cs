using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Models.Towers.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Abilities.Behaviors;
using BTD_Mod_Helper.Api.Enums;
using BTD_Mod_Helper.Extensions;

namespace UltimateCrosspathing.PostMergeFixes;

public class FixAbilities : PostMergeFix
{
    public override void Apply(TowerModel model)
    {
        foreach (var ability in model.GetAbilities().Where(abilityModel =>
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
}
