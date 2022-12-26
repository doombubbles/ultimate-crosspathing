using System;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Attack.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Filters;
using Il2CppAssets.Scripts.Models.Towers.Projectiles;
using Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors;
using BTD_Mod_Helper.Api.Enums;
using BTD_Mod_Helper.Extensions;

namespace UltimateCrosspathing.PostMergeFixes;

public class FixGlueOverriding : PostMergeFix
{
    public override void Apply(TowerModel model)
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
}