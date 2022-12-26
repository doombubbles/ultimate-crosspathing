using Il2CppAssets.Scripts.Models.Towers;

namespace UltimateCrosspathing.PostMergeFixes;

public abstract class PostMergeFix : ModContent
{
    protected override float RegistrationPriority => 0;
    public override int RegisterPerFrame => 999;

    public sealed override void Register()
    {
    }

    public abstract void Apply(TowerModel model);
}