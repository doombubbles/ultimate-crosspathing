using System.Linq;
using Il2CppAssets.Scripts.Models.Towers;
using BTD_Mod_Helper.Extensions;

namespace UltimateCrosspathing.PostMergeFixes;

public class FixSmallRanges : PostMergeFix
{
    public override void Apply(TowerModel model)
    {
        // TODO: are there any cases where this shouldn't be the case?
        var attackModels = model.GetAttackModels();
        foreach (var attackModel in attackModels.Where(attackModel => attackModel.range < model.range))
        {
            attackModel.range = model.range;
        }
    }
}