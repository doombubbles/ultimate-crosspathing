using BTD_Mod_Helper.Api.Enums;
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Abilities.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Attack.Behaviors;

namespace UltimateCrosspathing.PostMergeFixes;

public class FixMermonkey : PostMergeFix
{
    public override void Apply(TowerModel model)
    {
        if (model.appliedUpgrades.Contains("Symphonic Resonance")) // TODO replace with UpgradeType.SymphonicResonance
        {
            model.towerSelectionMenuThemeId = "MermonkeyTrance";
            if (model.appliedUpgrades.Contains("UpgradeType.SymphonicResonance")) // TODO replace with UpgradeType.ArcticKnight
            {
                var attack = model.GetDescendant<ActivateAttackModel>().attacks[0];
                attack.RemoveBehavior<TargetSelectedPointOrDefaultModel>();
                attack.RemoveChildDependant(attack.targetProvider);
                attack.targetProvider = null;
            }
        }
    }
}