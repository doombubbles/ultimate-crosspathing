using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Models.Towers.Projectiles;
using Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors;
using BTD_Mod_Helper.Api.Enums;
using BTD_Mod_Helper.Extensions;

namespace UltimateCrosspathing.PostMergeFixes;

public class FixClusterMauling : PostMergeFix
{
    public override void Apply(TowerModel model)
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