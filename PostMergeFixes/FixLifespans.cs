using Assets.Scripts.Models.Towers;
using Assets.Scripts.Models.Towers.Projectiles.Behaviors;
using BTD_Mod_Helper.Extensions;

namespace UltimateCrosspathing.PostMergeFixes;

public class FixLifespans : PostMergeFix
{
    public override void Apply(TowerModel model)
    {
        model.GetDescendants<AgeModel>().ForEach(ageModel =>
        {
            ageModel.Lifespan = System.Math.Max(ageModel.Lifespan, ageModel.lifespanFrames / 60f);
        });
    }
}