using System.Linq;
using Assets.Scripts.Models;
using Assets.Scripts.Models.Towers;
using BTD_Mod_Helper;
using BTD_Mod_Helper.Api;
using BTD_Mod_Helper.Api.ModOptions;
using UltimateCrosspathing.Merging;
using UnhollowerBaseLib;
using Path = System.IO.Path;

#if RELEASE
using UltimateCrosspathing.Loaders;
#endif

namespace UltimateCrosspathing
{
    public abstract class LoadInfo : ModContent
    {
        public virtual ModByteLoader<TowerModel> Loader { get; }

        public abstract ModSettingBool Enabled { get; }

        public sealed override void Register()
        {
        }
        
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
            var dummy = new TowerModel
            {
                behaviors = new Il2CppReferenceArray<Model>(towerModels.Count)
            };

            for (var i = 0; i < towerModels.Count; i++)
            {
                dummy.behaviors[i] = towerModels[i];
                dummy.AddChildDependant(towerModels[i]);
            }

            ModByteLoader.Generate(dummy,
                Path.Combine(GetInstance<UltimateCrosspathingMod>().ModSourcesPath, "Loaders", Name + "Loader.cs"),
                Path.Combine(GetInstance<UltimateCrosspathingMod>().ModSourcesPath, "Bytes", Name + "s.bytes")
            );
        }
#else
        public bool loaded;
#endif
        
    }

    public class Alchemist : LoadInfo
    {
#if RELEASE
        public override ModByteLoader<TowerModel> Loader => GetInstance<AlchemistLoader>();
#endif

        public override ModSettingBool Enabled => UltimateCrosspathingMod.AlchemistEnabled;
    }

    public class BananaFarm : LoadInfo
    {
#if RELEASE
        public override ModByteLoader<TowerModel> Loader => GetInstance<BananaFarmLoader>();
#endif

        public override ModSettingBool Enabled => UltimateCrosspathingMod.BananaFarmEnabled;
    }

    public class BombShooter : LoadInfo
    {
#if RELEASE
        public override ModByteLoader<TowerModel> Loader => GetInstance<BombShooterLoader>();
#endif

        public override ModSettingBool Enabled => UltimateCrosspathingMod.BombShooterEnabled;
    }

    public class BoomerangMonkey : LoadInfo
    {
#if RELEASE
        public override ModByteLoader<TowerModel> Loader => GetInstance<BoomerangMonkeyLoader>();
#endif

        public override ModSettingBool Enabled => UltimateCrosspathingMod.BoomerangMonkeyEnabled;
    }

    public class DartlingGunner : LoadInfo
    {
#if RELEASE
        public override ModByteLoader<TowerModel> Loader => GetInstance<DartlingGunnerLoader>();
#endif

        public override ModSettingBool Enabled => UltimateCrosspathingMod.DartlingGunnerEnabled;
    }

    public class DartMonkey : LoadInfo
    {
#if RELEASE
        public override ModByteLoader<TowerModel> Loader => GetInstance<DartMonkeyLoader>();
#endif

        public override ModSettingBool Enabled => UltimateCrosspathingMod.DartMonkeyEnabled;
    }

    public class Druid : LoadInfo
    {
#if RELEASE
        public override ModByteLoader<TowerModel> Loader => GetInstance<DruidLoader>();
#endif

        public override ModSettingBool Enabled => UltimateCrosspathingMod.DruidEnabled;
    }

    public class EngineerMonkey : LoadInfo
    {
#if RELEASE
        public override ModByteLoader<TowerModel> Loader => GetInstance<EngineerMonkeyLoader>();
#endif

        public override ModSettingBool Enabled => UltimateCrosspathingMod.EngineerMonkeyEnabled;
    }

    public class GlueGunner : LoadInfo
    {
#if RELEASE
        public override ModByteLoader<TowerModel> Loader => GetInstance<GlueGunnerLoader>();
#endif

        public override ModSettingBool Enabled => UltimateCrosspathingMod.GlueGunnerEnabled;
    }

    public class HeliPilot : LoadInfo
    {
#if RELEASE
        public override ModByteLoader<TowerModel> Loader => GetInstance<HeliPilotLoader>();
#endif

        public override ModSettingBool Enabled => UltimateCrosspathingMod.HeliPilotEnabled;
    }

    public class IceMonkey : LoadInfo
    {
#if RELEASE
        public override ModByteLoader<TowerModel> Loader => GetInstance<IceMonkeyLoader>();
#endif

        public override ModSettingBool Enabled => UltimateCrosspathingMod.IceMonkeyEnabled;
    }

    public class MonkeyAce : LoadInfo
    {
#if RELEASE
        public override ModByteLoader<TowerModel> Loader => GetInstance<MonkeyAceLoader>();
#endif

        public override ModSettingBool Enabled => UltimateCrosspathingMod.MonkeyAceEnabled;
    }

    public class MonkeyBuccaneer : LoadInfo
    {
#if RELEASE
        public override ModByteLoader<TowerModel> Loader => GetInstance<MonkeyBuccaneerLoader>();
#endif

        public override ModSettingBool Enabled => UltimateCrosspathingMod.MonkeyBuccaneerEnabled;
    }

    public class MonkeySub : LoadInfo
    {
#if RELEASE
        public override ModByteLoader<TowerModel> Loader => GetInstance<MonkeySubLoader>();
#endif

        public override ModSettingBool Enabled => UltimateCrosspathingMod.MonkeySubEnabled;
    }

    public class MonkeyVillage : LoadInfo
    {
#if RELEASE
        public override ModByteLoader<TowerModel> Loader => GetInstance<MonkeyVillageLoader>();
#endif

        public override ModSettingBool Enabled => UltimateCrosspathingMod.MonkeyVillageEnabled;
    }

    public class MortarMonkey : LoadInfo
    {
#if RELEASE
        public override ModByteLoader<TowerModel> Loader => GetInstance<MortarMonkeyLoader>();
#endif

        public override ModSettingBool Enabled => UltimateCrosspathingMod.MortarMonkeyEnabled;
    }

    public class NinjaMonkey : LoadInfo
    {
#if RELEASE
        public override ModByteLoader<TowerModel> Loader => GetInstance<NinjaMonkeyLoader>();
#endif

        public override ModSettingBool Enabled => UltimateCrosspathingMod.NinjaMonkeyEnabled;
    }

    public class SniperMonkey : LoadInfo
    {
#if RELEASE
        public override ModByteLoader<TowerModel> Loader => GetInstance<SniperMonkeyLoader>();
#endif

        public override ModSettingBool Enabled => UltimateCrosspathingMod.SniperMonkeyEnabled;
    }

    public class SpikeFactory : LoadInfo
    {
#if RELEASE
        public override ModByteLoader<TowerModel> Loader => GetInstance<SpikeFactoryLoader>();
#endif

        public override ModSettingBool Enabled => UltimateCrosspathingMod.SpikeFactoryEnabled;
    }

    public class SuperMonkey : LoadInfo
    {
#if RELEASE
        public override ModByteLoader<TowerModel> Loader => GetInstance<SuperMonkeyLoader>();
#endif

        public override ModSettingBool Enabled => UltimateCrosspathingMod.SuperMonkeyEnabled;
    }

    public class TackShooter : LoadInfo
    {
#if RELEASE
        public override ModByteLoader<TowerModel> Loader => GetInstance<TackShooterLoader>();
#endif

        public override ModSettingBool Enabled => UltimateCrosspathingMod.TackShooterEnabled;
    }

    public class WizardMonkey : LoadInfo
    {
#if RELEASE
        public override ModByteLoader<TowerModel> Loader => GetInstance<WizardMonkeyLoader>();
#endif

        public override ModSettingBool Enabled => UltimateCrosspathingMod.WizardMonkeyEnabled;
    }
}