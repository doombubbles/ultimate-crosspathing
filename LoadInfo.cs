using System.Linq;
using Assets.Scripts.Models;
using Assets.Scripts.Models.Towers;
using BTD_Mod_Helper.Api;
using UltimateCrosspathing.Loaders;
using UltimateCrosspathing.Merging;
using UnhollowerBaseLib;

namespace UltimateCrosspathing
{
    public abstract class LoadInfo : ModContent
    {
        public bool loaded;
        
        public abstract ModByteLoader<TowerModel> Loader { get; }

        public abstract bool Enabled { get; }

        protected sealed override void Register()
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
        public override bool Enabled => Main.AlchemistEnabled;
    }
    
    public class BananaFarm : LoadInfo<BananaFarmLoader>
    {
        public override bool Enabled => Main.BananaFarmEnabled;
    }
    
    public class BombShooter : LoadInfo<BombShooterLoader>
    {
        public override bool Enabled => Main.BombShooterEnabled;
    }
    
    public class BoomerangMonkey : LoadInfo<BoomerangMonkeyLoader>
    {
        public override bool Enabled => Main.BoomerangMonkeyEnabled;
    }
    
    public class DartlingGunner : LoadInfo<DartlingGunnerLoader>
    {
        public override bool Enabled => Main.DartlingGunnerEnabled;
    }
    
    public class DartMonkey : LoadInfo<DartMonkeyLoader>
    {
        public override bool Enabled => Main.DartMonkeyEnabled;
    }
    
    public class Druid : LoadInfo<DruidLoader>
    {
        public override bool Enabled => Main.DruidEnabled;
    }
    
    public class EngineerMonkey : LoadInfo<EngineerMonkeyLoader>
    {
        public override bool Enabled => Main.EngineerMonkeyEnabled;
    }
    
    public class GlueGunner : LoadInfo<DartMonkeyLoader>
    {
        public override bool Enabled => Main.GlueGunnerEnabled;
    }
    
    public class HeliPilot : LoadInfo<HeliPilotLoader>
    {
        public override bool Enabled => Main.HeliPilotEnabled;
    }
    
    public class IceMonkey : LoadInfo<IceMonkeyLoader>
    {
        public override bool Enabled => Main.IceMonkeyEnabled;
    }
    
    public class MonkeyAce : LoadInfo<MonkeyAceLoader>
    {
        public override bool Enabled => Main.MonkeyAceEnabled;
    }
    
    public class MonkeyBuccaneer : LoadInfo<MonkeyBuccaneerLoader>
    {
        public override bool Enabled => Main.MonkeyBuccaneerEnabled;
    }
    
    public class MonkeySub : LoadInfo<MonkeySubLoader>
    {
        public override bool Enabled => Main.MonkeySubEnabled;
    }
    
    public class MonkeyVillage : LoadInfo<MonkeyVillageLoader>
    {
        public override bool Enabled => Main.MonkeyVillageEnabled;
    }
    
    public class MortarMonkey : LoadInfo<MortarMonkeyLoader>
    {
        public override bool Enabled => Main.MortarMonkeyEnabled;
    }
    
    public class NinjaMonkey : LoadInfo<NinjaMonkeyLoader>
    {
        public override bool Enabled => Main.NinjaMonkeyEnabled;
    }
    
    public class SniperMonkey : LoadInfo<SniperMonkeyLoader>
    {
        public override bool Enabled => Main.SniperMonkeyEnabled;
    }
    
    public class SpikeFactory : LoadInfo<SpikeFactoryLoader>
    {
        public override bool Enabled => Main.SpikeFactoryEnabled;
    }
    
    public class SuperMonkey : LoadInfo<SuperMonkeyLoader>
    {
        public override bool Enabled => Main.SuperMonkeyEnabled;
    }
    
    public class TackShooter : LoadInfo<TackShooterLoader>
    {
        public override bool Enabled => Main.TackShooterEnabled;
    }
    
    public class WizardMonkey : LoadInfo<WizardMonkeyLoader>
    {
        public override bool Enabled => Main.WizardMonkeyEnabled;
    }
}