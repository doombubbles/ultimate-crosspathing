using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Models.Towers.Projectiles;
using Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors;
using BTD_Mod_Helper.Extensions;

namespace UltimateCrosspathing.PostMergeFixes;

public class FixHomingProjectiles : PostMergeFix
{
    public override void Apply(TowerModel model)
    {
        foreach (var projectileModel in model.GetDescendants<ProjectileModel>().ToIl2CppList()
                     .Where(projectileModel => projectileModel.GetBehavior<RetargetOnContactModel>() != null))
        {
            projectileModel.RemoveBehavior<FollowPathModel>();
        }
    }
}
