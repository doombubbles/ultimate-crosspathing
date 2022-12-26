using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Models.Towers.Behaviors;
using BTD_Mod_Helper.Extensions;

namespace UltimateCrosspathing.PostMergeFixes;

public class FixIceMonkeyRange : PostMergeFix
{
    public override void Apply(TowerModel model)
    {
        if (model.GetDescendant<SlowBloonsZoneModel>().IsType<SlowBloonsZoneModel>(out var slow))
        {
            slow.zoneRadius = model.range + 5;
        }
    }
}