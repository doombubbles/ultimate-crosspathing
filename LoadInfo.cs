using System.Collections.Generic;
using Il2CppAssets.Scripts.Models.Towers;
using BTD_Mod_Helper.Api.ModOptions;
using BTD_Mod_Helper.Api.Towers;

#if RELEASE
using UltimateCrosspathing.Loaders;
#endif

namespace UltimateCrosspathing
{
    public abstract class LoadInfo : ModContent
    {
        private static readonly Dictionary<string, LoadInfo> Cache = new();

        public virtual ModByteLoader<TowerModel> Loader { get; }

        public abstract ModSettingBool Enabled { get; }

        public bool? loaded = null;

        public sealed override void Register()
        {
            Cache[Name] = this;
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

        public override ModSettingBool Enabled => TowerSettings.AlchemistEnabled;
    }

    public class BananaFarm : LoadInfo
    {
#if RELEASE
        public override ModByteLoader<TowerModel> Loader => GetInstance<BananaFarmLoader>();
#endif

        public override ModSettingBool Enabled => TowerSettings.BananaFarmEnabled;
    }
    
    public class BeastHandler : LoadInfo
    {
#if RELEASE
        public override ModByteLoader<TowerModel> Loader => GetInstance<BeastHandlerLoader>();
#endif

        public override ModSettingBool Enabled => TowerSettings.BeastHandlerEnabled;
    }

    public class BombShooter : LoadInfo
    {
#if RELEASE
        public override ModByteLoader<TowerModel> Loader => GetInstance<BombShooterLoader>();
#endif

        public override ModSettingBool Enabled => TowerSettings.BombShooterEnabled;
    }

    public class BoomerangMonkey : LoadInfo
    {
#if RELEASE
        public override ModByteLoader<TowerModel> Loader => GetInstance<BoomerangMonkeyLoader>();
#endif

        public override ModSettingBool Enabled => TowerSettings.BoomerangMonkeyEnabled;
    }

    public class DartlingGunner : LoadInfo
    {
#if RELEASE
        public override ModByteLoader<TowerModel> Loader => GetInstance<DartlingGunnerLoader>();
#endif

        public override ModSettingBool Enabled => TowerSettings.DartlingGunnerEnabled;
    }

    public class DartMonkey : LoadInfo
    {
#if RELEASE
        public override ModByteLoader<TowerModel> Loader => GetInstance<DartMonkeyLoader>();
#endif

        public override ModSettingBool Enabled => TowerSettings.DartMonkeyEnabled;
    }

    public class Druid : LoadInfo
    {
#if RELEASE
        public override ModByteLoader<TowerModel> Loader => GetInstance<DruidLoader>();
#endif

        public override ModSettingBool Enabled => TowerSettings.DruidEnabled;
    }

    public class EngineerMonkey : LoadInfo
    {
#if RELEASE
        public override ModByteLoader<TowerModel> Loader => GetInstance<EngineerMonkeyLoader>();
#endif

        public override ModSettingBool Enabled => TowerSettings.EngineerMonkeyEnabled;
    }

    public class GlueGunner : LoadInfo
    {
#if RELEASE
        public override ModByteLoader<TowerModel> Loader => GetInstance<GlueGunnerLoader>();
#endif

        public override ModSettingBool Enabled => TowerSettings.GlueGunnerEnabled;
    }

    public class HeliPilot : LoadInfo
    {
#if RELEASE
        public override ModByteLoader<TowerModel> Loader => GetInstance<HeliPilotLoader>();
#endif

        public override ModSettingBool Enabled => TowerSettings.HeliPilotEnabled;
    }

    public class IceMonkey : LoadInfo
    {
#if RELEASE
        public override ModByteLoader<TowerModel> Loader => GetInstance<IceMonkeyLoader>();
#endif

        public override ModSettingBool Enabled => TowerSettings.IceMonkeyEnabled;
    }

    public class MonkeyAce : LoadInfo
    {
#if RELEASE
        public override ModByteLoader<TowerModel> Loader => GetInstance<MonkeyAceLoader>();
#endif

        public override ModSettingBool Enabled => TowerSettings.MonkeyAceEnabled;
    }

    public class MonkeyBuccaneer : LoadInfo
    {
#if RELEASE
        public override ModByteLoader<TowerModel> Loader => GetInstance<MonkeyBuccaneerLoader>();
#endif

        public override ModSettingBool Enabled => TowerSettings.MonkeyBuccaneerEnabled;
    }

    public class MonkeySub : LoadInfo
    {
#if RELEASE
        public override ModByteLoader<TowerModel> Loader => GetInstance<MonkeySubLoader>();
#endif

        public override ModSettingBool Enabled => TowerSettings.MonkeySubEnabled;
    }

    public class MonkeyVillage : LoadInfo
    {
#if RELEASE
        public override ModByteLoader<TowerModel> Loader => GetInstance<MonkeyVillageLoader>();
#endif

        public override ModSettingBool Enabled => TowerSettings.MonkeyVillageEnabled;
    }

    public class MortarMonkey : LoadInfo
    {
#if RELEASE
        public override ModByteLoader<TowerModel> Loader => GetInstance<MortarMonkeyLoader>();
#endif

        public override ModSettingBool Enabled => TowerSettings.MortarMonkeyEnabled;
    }

    public class NinjaMonkey : LoadInfo
    {
#if RELEASE
        public override ModByteLoader<TowerModel> Loader => GetInstance<NinjaMonkeyLoader>();
#endif

        public override ModSettingBool Enabled => TowerSettings.NinjaMonkeyEnabled;
    }

    public class SniperMonkey : LoadInfo
    {
#if RELEASE
        public override ModByteLoader<TowerModel> Loader => GetInstance<SniperMonkeyLoader>();
#endif

        public override ModSettingBool Enabled => TowerSettings.SniperMonkeyEnabled;
    }

    public class SpikeFactory : LoadInfo
    {
#if RELEASE
        public override ModByteLoader<TowerModel> Loader => GetInstance<SpikeFactoryLoader>();
#endif

        public override ModSettingBool Enabled => TowerSettings.SpikeFactoryEnabled;
    }

    public class SuperMonkey : LoadInfo
    {
#if RELEASE
        public override ModByteLoader<TowerModel> Loader => GetInstance<SuperMonkeyLoader>();
#endif

        public override ModSettingBool Enabled => TowerSettings.SuperMonkeyEnabled;
    }

    public class TackShooter : LoadInfo
    {
#if RELEASE
        public override ModByteLoader<TowerModel> Loader => GetInstance<TackShooterLoader>();
#endif

        public override ModSettingBool Enabled => TowerSettings.TackShooterEnabled;
    }

    public class WizardMonkey : LoadInfo
    {
#if RELEASE
        public override ModByteLoader<TowerModel> Loader => GetInstance<WizardMonkeyLoader>();
#endif

        public override ModSettingBool Enabled => TowerSettings.WizardMonkeyEnabled;
    }
}