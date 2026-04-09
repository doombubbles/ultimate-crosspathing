using BTD_Mod_Helper.Api.Enums;
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Models.Towers.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Abilities;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Abilities.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Attack;
using Il2CppAssets.Scripts.Models.Towers.Weapons;

namespace UltimateCrosspathing.PostMergeFixes;

public class FixPowersPro : PostMergeFix
{
    public override void Apply(TowerModel model)
    {
        if (string.IsNullOrEmpty(model.powerName)) return;

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
                }
                else if (weapon.name.Contains("Third"))
                {
                    weapon.customStartCooldown = 3;
                }
            }
        }

        if (model.baseId == "MonkeyBoostPro")
        {
            var ability = model.GetAbility();

            if (model.appliedUpgrades.Contains(UpgradeType.Resistance))
            {
                ability.Cooldown = 60;
            }

            var lifespan = model.appliedUpgrades.Contains(UpgradeType.Noquit)
                ? 30
                : model.appliedUpgrades.Contains(UpgradeType.Highreps)
                    ? 20
                    : 15;

            ability.GetDescendants<ActivateRateSupportZoneModel>().ForEach(m => m.lifespan = lifespan);
            ability.GetDescendants<ActivateRangeSupportZoneModel>().ForEach(m => m.lifespan = lifespan);
            ability.GetDescendants<ActivateVisibilitySupportZoneModel>().ForEach(m => m.lifespan = lifespan);
            ability.GetDescendants<ActivateAttackCollisionSupportZoneModel>().ForEach(m => m.lifespan = lifespan);
            ability.GetDescendants<ActivateSpreadSupportZoneModel>().ForEach(m => m.lifespan = lifespan);
            ability.GetDescendants<ActivateIgnoreStunSupportZoneModel>().ForEach(m => m.lifespan = lifespan);
        }

        if (model.baseId == TowerType.TechBotPrime)
        {
            if (model.appliedUpgrades.Contains(UpgradeType.NEUR0MAP))
            {
                model.GetDescendants<TechBotLinkModel>().ForEach(m => m.activateInRadius = true);
            }

            if (model.appliedUpgrades.Contains(UpgradeType.Droneambusnetwork))
            {
                model.RemoveBehavior<AttackModel>();
                model.RemoveBehavior<AbilityModel>("AmbushTechAbility");
                model.UpdateTargetProviders();
            }
        }
    }
}