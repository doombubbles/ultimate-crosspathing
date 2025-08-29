using System.Collections.Generic;
using Il2CppAssets.Scripts.Models.Towers;
using BTD_Mod_Helper.Api.ModOptions;
using BTD_Mod_Helper.Api.Towers;
using Il2CppAssets.Scripts.Unity;

#if RELEASE
using UltimateCrosspathing.Loaders;
#else
using Il2CppAssets.Scripts.Models;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using UltimateCrosspathing.Tasks;
using Path = System.IO.Path;
#endif

namespace UltimateCrosspathing;

public abstract class LoadInfo : ModContent
{
    private static readonly Dictionary<string, LoadInfo> Cache = new();

    public virtual ModByteLoader<TowerModel> Loader { get; }

    private ModSettingBool enabled = null!;

    public bool Enabled
    {
        get => enabled;
        set => enabled.SetValue(value);
    }

    public virtual bool PowerPro => false;

    public virtual int TopPath => PowerPro ? 3 : 5;
    public virtual int MidPath => PowerPro ? 3 : 5;
    public virtual int BotPath => PowerPro ? 3 : 5;

    public bool? loaded = null;

    protected override int Order => TowerType.towers.IndexOf(Name) is var index and >= 0 ? index : 99;

    public override IEnumerable<ModContent> Load()
    {
        enabled = new ModSettingBool(true)
        {
            button = true,
            category = Settings.IndividualTowers,
            displayName = $"[{Name}]",
            requiresRestart = true,
        };
        mod.ModSettings[Name + "Enabled"] = enabled;
        return base.Load();
    }

    public sealed override void Register()
    {
        Cache[Name] = this;
        enabled.icon = Game.instance.model.GetTowerWithName(Name).icon.AssetGUID;
    }

    public static bool TryFind(string baseId, out LoadInfo loadInfo) => Cache.TryGetValue(baseId, out loadInfo);

    public static bool ShouldWork(string baseId) => TryFind(baseId, out var loadInfo)
                                                        ? loadInfo.Enabled && loadInfo.loaded != false
                                                        : Find<ModTower>(baseId) != null;

#if DEBUG
    public static void ExportTowers()
    {
        foreach (var info in GetContent<LoadInfo>().Where(info => info.Enabled))
        {
            info.Export();
        }

        ModHelper.Msg<UltimateCrosspathingMod>("Finished exporting!");
    }

    public void Export()
    {
        var towerModels = GenerateTask.TowerModels.Where(model => model.baseId == Name).ToList();
        var dummy = ModTowerHelper.CreateTowerModel(Id);
        dummy.behaviors = new Il2CppReferenceArray<Model>(towerModels.Count);

        for (var i = 0; i < towerModels.Count; i++)
        {
            dummy.behaviors[i] = towerModels[i];
            dummy.AddChildDependant(towerModels[i]);
        }

        ModByteLoader.Generate(dummy,
            Path.Combine(GetInstance<UltimateCrosspathingMod>().ModSourcesPath, "Loaders", Name + "Loader.cs"),
            Path.Combine(GetInstance<UltimateCrosspathingMod>().ModSourcesPath, "Bytes", Name + "s.bytes"),
            "UltimateCrosspathing.Loaders"
        );
    }
#endif
}

public class Alchemist : LoadInfo
{
#if RELEASE
    public override ModByteLoader<TowerModel> Loader => GetInstance<AlchemistLoader>();
#endif
}

public class BananaFarm : LoadInfo
{
#if RELEASE
    public override ModByteLoader<TowerModel> Loader => GetInstance<BananaFarmLoader>();
#endif
}

public class BeastHandler : LoadInfo
{
#if RELEASE
    public override ModByteLoader<TowerModel> Loader => GetInstance<BeastHandlerLoader>();
#endif
}

public class BombShooter : LoadInfo
{
#if RELEASE
    public override ModByteLoader<TowerModel> Loader => GetInstance<BombShooterLoader>();
#endif
}

public class BoomerangMonkey : LoadInfo
{
#if RELEASE
    public override ModByteLoader<TowerModel> Loader => GetInstance<BoomerangMonkeyLoader>();
#endif
}

public class DartlingGunner : LoadInfo
{
#if RELEASE
    public override ModByteLoader<TowerModel> Loader => GetInstance<DartlingGunnerLoader>();
#endif
}

public class DartMonkey : LoadInfo
{
#if RELEASE
    public override ModByteLoader<TowerModel> Loader => GetInstance<DartMonkeyLoader>();
#endif
}

public class Druid : LoadInfo
{
#if RELEASE
    public override ModByteLoader<TowerModel> Loader => GetInstance<DruidLoader>();
#endif
}

public class EngineerMonkey : LoadInfo
{
#if RELEASE
    public override ModByteLoader<TowerModel> Loader => GetInstance<EngineerMonkeyLoader>();
#endif
}

public class GlueGunner : LoadInfo
{
#if RELEASE
    public override ModByteLoader<TowerModel> Loader => GetInstance<GlueGunnerLoader>();
#endif
}

public class Desperado : LoadInfo
{
#if RELEASE
    public override ModByteLoader<TowerModel> Loader => GetInstance<DesperadoLoader>();
#endif
}

public class HeliPilot : LoadInfo
{
#if RELEASE
    public override ModByteLoader<TowerModel> Loader => GetInstance<HeliPilotLoader>();
#endif
}

public class IceMonkey : LoadInfo
{
#if RELEASE
    public override ModByteLoader<TowerModel> Loader => GetInstance<IceMonkeyLoader>();
#endif
}

public class MonkeyAce : LoadInfo
{
#if RELEASE
    public override ModByteLoader<TowerModel> Loader => GetInstance<MonkeyAceLoader>();
#endif
}

public class MonkeyBuccaneer : LoadInfo
{
#if RELEASE
    public override ModByteLoader<TowerModel> Loader => GetInstance<MonkeyBuccaneerLoader>();
#endif
}

public class MonkeySub : LoadInfo
{
#if RELEASE
    public override ModByteLoader<TowerModel> Loader => GetInstance<MonkeySubLoader>();
#endif
}

public class MonkeyVillage : LoadInfo
{
#if RELEASE
    public override ModByteLoader<TowerModel> Loader => GetInstance<MonkeyVillageLoader>();
#endif
}

public class MortarMonkey : LoadInfo
{
#if RELEASE
    public override ModByteLoader<TowerModel> Loader => GetInstance<MortarMonkeyLoader>();
#endif
}

public class NinjaMonkey : LoadInfo
{
#if RELEASE
    public override ModByteLoader<TowerModel> Loader => GetInstance<NinjaMonkeyLoader>();
#endif
}

public class SniperMonkey : LoadInfo
{
#if RELEASE
    public override ModByteLoader<TowerModel> Loader => GetInstance<SniperMonkeyLoader>();
#endif
}

public class SpikeFactory : LoadInfo
{
#if RELEASE
    public override ModByteLoader<TowerModel> Loader => GetInstance<SpikeFactoryLoader>();
#endif
}

public class SuperMonkey : LoadInfo
{
#if RELEASE
    public override ModByteLoader<TowerModel> Loader => GetInstance<SuperMonkeyLoader>();
#endif
}

public class TackShooter : LoadInfo
{
#if RELEASE
    public override ModByteLoader<TowerModel> Loader => GetInstance<TackShooterLoader>();
#endif
}

public class WizardMonkey : LoadInfo
{
#if RELEASE
    public override ModByteLoader<TowerModel> Loader => GetInstance<WizardMonkeyLoader>();
#endif
}

public class Mermonkey : LoadInfo
{
#if RELEASE
    public override ModByteLoader<TowerModel> Loader => GetInstance<MermonkeyLoader>();
#endif
}

public class BananaFarmerPro : LoadInfo
{
#if RELEASE
    public override ModByteLoader<TowerModel> Loader => GetInstance<BananaFarmerProLoader>();
#endif
    public override bool PowerPro => true;
}

public class SuperMonkeyBeacon : LoadInfo
{
#if RELEASE
    public override ModByteLoader<TowerModel> Loader => GetInstance<SuperMonkeyBeaconLoader>();
#endif
    public override bool PowerPro => true;
}