using Il2CppAssets.Scripts.Models.Towers;

namespace UltimateCrosspathing.PostMergeFixes;

public class FixBehaviorNames : PostMergeFix
{
    public override void Apply(TowerModel model)
    {
        var behaviors = model.behaviors.ToList();
        for (var i = 0; i < behaviors.Count; i++)
        {
            var behavior = behaviors[i];
            if (behaviors.Take(i).Any(b => b.name == behavior.name))
            {
                behavior.name += i;
            }
        }
    }
}
