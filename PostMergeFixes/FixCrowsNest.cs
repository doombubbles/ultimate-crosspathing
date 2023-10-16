using BTD_Mod_Helper.Api.Enums;
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Models.Towers.Filters;

namespace UltimateCrosspathing.PostMergeFixes;

public class FixCrowsNest : PostMergeFix
{
    public override void Apply(TowerModel model)
    {
        if (model.appliedUpgrades.Contains(UpgradeType.CrowsNest))
        {
            model.GetDescendants<FilterInvisibleModel>().ForEach(filter => filter.isActive = false);
        }
    }
}