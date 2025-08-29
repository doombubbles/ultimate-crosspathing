using BTD_Mod_Helper.Api.Enums;
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Models.Towers.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Abilities.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Weapons;

namespace UltimateCrosspathing.PostMergeFixes;

public class FixPowersPro : PostMergeFix
{
    public override void Apply(TowerModel model)
    {
        model.RemoveBehaviors<PowerProTowerModel>();
        model.AddBehavior(new PowerProTowerModel(model.name, model.powerName));

        if (model.baseId == TowerType.SuperMonkeyBeacon)
        {
            model.GetDescendant<ActivateAttackModel>().Lifespan =
                model.appliedUpgrades.Contains(UpgradeType.Reverberation) ? 3.05f
                : model.appliedUpgrades.Contains(UpgradeType.StormTremors) ? 2.05f
                : 0;

            foreach (var weapon in model.GetDescendants<WeaponModel>().AsIEnumerable())
            {
                // TODO why does the Rate get reset if I try to change it here?
                if (weapon.name.Contains("FollowUp"))
                {
                    weapon.customStartCooldown = 2;
                } else if (weapon.name.Contains("Third"))
                {
                    weapon.customStartCooldown = 3;
                }
            }
        }

    }
}