using System.Linq;
using Assets.Scripts.Models;
using Assets.Scripts.Models.Towers;
using BTD_Mod_Helper.Api;
using BTD_Mod_Helper.Api.ModOptions;
using UltimateCrosspathing.Loaders;
using UltimateCrosspathing.Merging;
using UnhollowerBaseLib;

namespace UltimateCrosspathing
{
    public abstract class LoadInfo : ModContent
    {
        public bool loaded;
        
        public abstract ModByteLoader<TowerModel> Loader { get; }

        public abstract ModSettingBool Enabled { get; }

        public sealed override void Register()
        {
        }

        public void Export()
        {
            var towerModels = AsyncMerging.FinishedTowerModels.Where(model => model.baseId == Name).ToList();
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
                $"C:\\Users\\jpgale\\RiderProjects2\\UltimateCrosspathing\\Loaders\\${Name}Loader.cs", 
                $"C:\\Users\\jpgale\\RiderProjects2\\UltimateCrosspathing\\Bytes\\{Name}s.bytes");
        }
    }
    
    public abstract class LoadInfo<T> : LoadInfo where T : ModByteLoader<TowerModel>
    {
        public override ModByteLoader<TowerModel> Loader => GetInstance<T>();
    }

    public class Alchemist : LoadInfo<AlchemistLoader>
    {
        public override ModSettingBool Enabled => UltimateCrosspathingMod.AlchemistEnabled;
    }
    
    public class BananaFarm : LoadInfo<BananaFarmLoader>
    {
        public override ModSettingBool Enabled => UltimateCrosspathingMod.BananaFarmEnabled;
    }
    
    public class BombShooter : LoadInfo<BombShooterLoader>
    {
        public override ModSettingBool Enabled => UltimateCrosspathingMod.BombShooterEnabled;
    }
    
    public class BoomerangMonkey : LoadInfo<BoomerangMonkeyLoader>
    {
        public override ModSettingBool Enabled => UltimateCrosspathingMod.BoomerangMonkeyEnabled;
    }
    
    public class DartlingGunner : LoadInfo<DartlingGunnerLoader>
    {
        public override ModSettingBool Enabled => UltimateCrosspathingMod.DartlingGunnerEnabled;
    }
    
    public class DartMonkey : LoadInfo<DartMonkeyLoader>
    {
        public override ModSettingBool Enabled => UltimateCrosspathingMod.DartMonkeyEnabled;
    }
    
    public class Druid : LoadInfo<DruidLoader>
    {
        public override ModSettingBool Enabled => UltimateCrosspathingMod.DruidEnabled;
    }
    
    public class EngineerMonkey : LoadInfo<EngineerMonkeyLoader>
    {
        public override ModSettingBool Enabled => UltimateCrosspathingMod.EngineerMonkeyEnabled;
    }
    
    public class GlueGunner : LoadInfo<GlueGunnerLoader>
    {
        public override ModSettingBool Enabled => UltimateCrosspathingMod.GlueGunnerEnabled;
    }
    
    public class HeliPilot : LoadInfo<HeliPilotLoader>
    {
        public override ModSettingBool Enabled => UltimateCrosspathingMod.HeliPilotEnabled;
    }
    
    public class IceMonkey : LoadInfo<IceMonkeyLoader>
    {
        public override ModSettingBool Enabled => UltimateCrosspathingMod.IceMonkeyEnabled;
    }
    
    public class MonkeyAce : LoadInfo<MonkeyAceLoader>
    {
        public override ModSettingBool Enabled => UltimateCrosspathingMod.MonkeyAceEnabled;
    }
    
    public class MonkeyBuccaneer : LoadInfo<MonkeyBuccaneerLoader>
    {
        public override ModSettingBool Enabled => UltimateCrosspathingMod.MonkeyBuccaneerEnabled;
    }
    
    public class MonkeySub : LoadInfo<MonkeySubLoader>
    {
        public override ModSettingBool Enabled => UltimateCrosspathingMod.MonkeySubEnabled;
    }
    
    public class MonkeyVillage : LoadInfo<MonkeyVillageLoader>
    {
        public override ModSettingBool Enabled => UltimateCrosspathingMod.MonkeyVillageEnabled;
    }
    
    public class MortarMonkey : LoadInfo<MortarMonkeyLoader>
    {
        public override ModSettingBool Enabled => UltimateCrosspathingMod.MortarMonkeyEnabled;
    }
    
    public class NinjaMonkey : LoadInfo<NinjaMonkeyLoader>
    {
        public override ModSettingBool Enabled => UltimateCrosspathingMod.NinjaMonkeyEnabled;
    }
    
    public class SniperMonkey : LoadInfo<SniperMonkeyLoader>
    {
        public override ModSettingBool Enabled => UltimateCrosspathingMod.SniperMonkeyEnabled;
    }
    
    public class SpikeFactory : LoadInfo<SpikeFactoryLoader>
    {
        public override ModSettingBool Enabled => UltimateCrosspathingMod.SpikeFactoryEnabled;
    }
    
    public class SuperMonkey : LoadInfo<SuperMonkeyLoader>
    {
        public override ModSettingBool Enabled => UltimateCrosspathingMod.SuperMonkeyEnabled;
    }
    
    public class TackShooter : LoadInfo<TackShooterLoader>
    {
        public override ModSettingBool Enabled => UltimateCrosspathingMod.TackShooterEnabled;
    }
    
    public class WizardMonkey : LoadInfo<WizardMonkeyLoader>
    {
        public override ModSettingBool Enabled => UltimateCrosspathingMod.WizardMonkeyEnabled;
    }
}