using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Assets.Scripts.Models;
using Assets.Scripts.Models.Towers;
using Assets.Scripts.Unity;
using BTD_Mod_Helper.Extensions;
using MelonLoader;
using UltimateCrosspathing.Merging;
using UnhollowerBaseLib;
using static Assets.Scripts.Models.Towers.TowerType;

namespace UltimateCrosspathing
{
    public static class Loading
    {
        private static readonly string[] References = {
            "Assets.Scripts.Models.Towers.TargetType"
        };

        private static readonly string[] Structs =
        {
            "Assets.Scripts.Models.Map.AreaType",
            "Assets.Scripts.Simulation.SMath.Vector3"
        };

        private static readonly string[] Enums =
        {
            "Assets.Scripts.Models.Towers.TowerModel.TowerSize"
        };

        public static readonly Dictionary<string, TowersLoader> Loaders = new Dictionary<string, TowersLoader>
        {
            { DartMonkey, new DartMonkeyLoader() },
            { BoomerangMonkey, new BoomerangMonkeyLoader() },
            { BombShooter, new BombShooterLoader() },
            { TackShooter, new TackShooterLoader() },
            { IceMonkey, new IceMonkeyLoader() },
            { GlueGunner, new GlueGunnerLoader() },
            { SniperMonkey, new SniperMonkeyLoader() },
            { MonkeySub, new MonkeySubLoader() },
            { MonkeyBuccaneer, new MonkeyBuccaneerLoader() },
            { MonkeyAce, new MonkeyAceLoader() },
            { HeliPilot, new HeliPilotLoader() },
            { MortarMonkey, new MortarMonkeyLoader() },
            { DartlingGunner, new DartlingGunnerLoader() },
            { WizardMonkey, new WizardMonkeyLoader() },
            { SuperMonkey, new SuperMonkeyLoader() },
            { NinjaMonkey, new NinjaMonkeyLoader() },
            { Alchemist, new AlchemistLoader() },
            { Druid, new DruidLoader() },
            { BananaFarm, new BananaFarmLoader() },
            { SpikeFactory, new SpikeFactoryLoader() },
            { MonkeyVillage, new MonkeyVillageLoader() },
            { EngineerMonkey, new EngineerMonkeyLoader() },
        };


        public static void ExportTowers()
        {
            foreach (var (tower, _) in Main.TowersEnabled.Where(pair => pair.Value))
            {
                var bytesName = $"Bytes\\{tower}s.bytes";
                var loaderOriginalName = $"Original\\{tower}Loader.cs";
                var loaderName = $"Loaders\\{tower}Loader.cs";
                
                var flatFileCodeGen = new FlatFileCodeGen();
                
                MelonLogger.Msg($"Creating holder for {tower}s");
            
                var towerModels = AsyncMerging.FinishedTowerModels.Where(model => model.baseId == tower).ToList();
                var dummy = new TowerModel
                {
                    behaviors = new Il2CppReferenceArray<Model>(towerModels.Count)
                };
            
                MelonLogger.Msg($"Bundling towers into holder for {tower}s");

                for (var i = 0; i < towerModels.Count; i++)
                {
                    dummy.behaviors[i] = towerModels[i];
                    dummy.AddChildDependant(towerModels[i]);
                }

                MelonLogger.Msg($"Generating bytes for {tower}s");
            
                flatFileCodeGen.Generate(dummy, bytesName, loaderOriginalName);

                MelonLogger.Msg($"Finished generating bytes for {tower}s");
                
                ConvertLoader(Environment.CurrentDirectory + "\\" + loaderOriginalName,
                    Environment.CurrentDirectory + "\\" + loaderName, $"{tower}Loader");
            }
            
            MelonLogger.Msg("Finished exporting!");
        }

        public static void LoadTower(Stream resource, string tower)
        {
            var memoryStream = new MemoryStream();
            resource.CopyTo(memoryStream);

            var towerModelLoader = Loaders[tower];
            var dummy = towerModelLoader.Load(memoryStream.ToArray());
            var towers = dummy.behaviors.Where(model => model != null)
                .Select(model => model.TryCast<TowerModel>()).ToArray();

            foreach (var model in towers)
            {
                Towers.PostMerge(model);
                foreach (var crosspathingPatchMod in MelonHandler.Mods.OfType<CrosspathingPatchMod>())
                {
                    crosspathingPatchMod.Postmerge(model, model.baseId, model.tiers[0], model.tiers[1],
                        model.tiers[2]);
                }
            }

            Game.instance.model.AddTowersToGame(towers);
            foreach (var towerModel in towers)
            {
                Towers.AddUpgradeToPrevs(towerModel);
            }

            towerModelLoader.m = null;
        }

        private static void ConvertLoader(string loaderPath, string fixPath, string className)
        {
            using (var reader = new StreamReader(loaderPath))
            {
                var loader = reader.ReadToEnd();

                loader = "using UnhollowerBaseLib;\n" +
                         "using UnhollowerRuntimeLib;\n" +
                         "using BTD_Mod_Helper.Extensions;\n" +
                         "using UltimateCrosspathing;\n" + loader;
                loader = loader.Replace("TowerModelLoader : IGameModelLoader", className + " : TowersLoader");
                loader = loader.Replace("using System", "using Il2CppSystem");
                loader = loader.Replace("using Il2CppSystem.IO", "using System.IO");
                loader = Regex.Replace(loader, @"private void (.*)<T>\(\) {", @"private void $1<T>() where T : Il2CppObjectBase {");

                loader = loader.Replace("samplesField.SetValue(v,(System.Single[]) m[br.ReadInt32()]);", "v.samples = (System.Single[]) m[br.ReadInt32()];");
                
                loader = loader.Replace("T[]", "Il2CppReferenceArray<T>");
                loader = loader.Replace("new string[arrCount]", "new Il2CppStringArray(arrCount)");
                loader = loader.Replace("System.String[]", "Il2CppStringArray");
                loader = loader.Replace("System.Single[]", "Il2CppStructArray<float>");
                loader = loader.Replace("System.Int32[]", "Il2CppStructArray<int>");
                loader = loader.Replace("new int[arrCount]", "new Il2CppStructArray<int>(arrCount)");
                loader = loader.Replace("new float[arrCount]", "new Il2CppStructArray<float>(arrCount)");
                loader = loader.Replace("new T[br.ReadInt32()]", "new Il2CppReferenceArray<T>(br.ReadInt32())");
                loader = loader.Replace("(T)FormatterServices.GetUninitializedObject(t)", "FormatterServices.GetUninitializedObject(t).Cast<T>()");
                loader = Regex.Replace(loader, @"typeof\(([A-Z].*)\)", @"Il2CppType.Of<$1>()");
                loader = loader.Replace("arr[j] = new Assets.Scripts.Models.Towers.TargetType(br.ReadString(), br.ReadBoolean())", "arr[j] = new Assets.Scripts.Models.Towers.TargetType {id = br.ReadString(), isActionable = br.ReadBoolean()}");
                loader = loader.Replace("v.targetType = new Assets.Scripts.Models.Towers.TargetType(br.ReadString(), br.ReadBoolean());", "v.targetType.id = br.ReadString();\n\t\t\tv.targetType.actionOnCreate = br.ReadBoolean();");
                loader = loader.Replace("public Assets.Scripts.Models.Towers.TowerModel Load",
                    "public override Assets.Scripts.Models.Towers.TowerModel Load");
                loader = loader.Replace("object[] m;", "");

                
                foreach (var reference in References)
                {
                    loader = loader.Replace($"{reference}[arrCount]", $"Il2CppReferenceArray<{reference}>(arrCount)");
                    loader = loader.Replace($"{reference}[]", $"Il2CppReferenceArray<{reference}>");
                }
                
                foreach (var strukt in Structs)
                {
                    loader = loader.Replace($"{strukt}[arrCount]", $"Il2CppStructArray<{strukt}>(arrCount)");
                    loader = loader.Replace($"{strukt}[]", $"Il2CppStructArray<{strukt}>");
                }
                
                foreach (var enuhm in Enums)
                {
                    loader = Regex.Replace(loader, $@"\({enuhm}\) \((br\.ReadInt32\(\))\)", @"$1");
                }

                loader = Regex.Replace(loader, @"SetValue\(v\,br\.ReadInt32\(\)\)", @"SetValue(v,br.ReadInt32().ToIl2Cpp())");
                loader = Regex.Replace(loader, @"SetValue\(v\,br\.ReadSingle\(\)\)", @"SetValue(v,br.ReadSingle().ToIl2Cpp())");
                loader = Regex.Replace(loader, @"SetValue\(v\,br\.ReadBoolean\(\)\)", @"SetValue(v,br.ReadBoolean().ToIl2Cpp())");

                loader = Regex.Replace(loader, @"\(([A-Z].+)\[\]\)", @"(Il2CppReferenceArray<$1>)");

                using (var writer = new StreamWriter(fixPath))
                {
                    writer.Write(loader);
                }
            }
            
        }
    }
}