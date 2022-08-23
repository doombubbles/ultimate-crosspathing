using Assets.Scripts.Models.Towers;
using Assets.Scripts.Models.Towers.Behaviors.Attack.Behaviors;
using BTD_Mod_Helper.Api.Enums;
using BTD_Mod_Helper.Extensions;

namespace UltimateCrosspathing.PostMergeFixes;

public class FixAdvancedIntel : PostMergeFix
{
    public override void Apply(TowerModel model)
    {
        if (model.appliedUpgrades.Contains(UpgradeType.AdvancedIntel))
        {
            model.GetDescendants<TargetFirstSharedRangeModel>().ForEach(m => m.isSharedRangeEnabled = true);
            model.GetDescendants<TargetLastSharedRangeModel>().ForEach(m => m.isSharedRangeEnabled = true);
            model.GetDescendants<TargetCloseSharedRangeModel>().ForEach(m => m.isSharedRangeEnabled = true);
            model.GetDescendants<TargetStrongSharedRangeModel>().ForEach(m => m.isSharedRangeEnabled = true);
        }
    }
}