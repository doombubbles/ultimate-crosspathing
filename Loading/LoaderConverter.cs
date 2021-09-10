using System.IO;
using System.Text.RegularExpressions;
using Assets.Scripts.Models;
using Assets.Scripts.Models.Towers;
using UltimateCrosspathing.Merging;
using UnhollowerBaseLib;

namespace UltimateCrosspathing.Loading
{
    public static class LoaderConverter
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
        

        public static void ExportTowers(string bytesName, string loaderName)
        {
            var flatFileCodeGen = new FlatFileCodeGen();

            var dummy = new TowerModel
            {
                behaviors = new Il2CppReferenceArray<Model>(AsyncMerging.FinishedTowerModels.Count)
            };
            
            for (var i = 0; i < AsyncMerging.FinishedTowerModels.Count; i++)
            {
                dummy.behaviors[i] = AsyncMerging.FinishedTowerModels[i];
                dummy.AddChildDependant(AsyncMerging.FinishedTowerModels[i]);
            }

            flatFileCodeGen.Generate(dummy, bytesName, loaderName);
        }
        
        public static void ConvertLoader(string loaderPath, string fixPath)
        {
            using (var reader = new StreamReader(loaderPath))
            {
                var loader = reader.ReadToEnd();

                loader = "using UnhollowerBaseLib;\n" +
                         "using UnhollowerRuntimeLib;\n" +
                         "using BTD_Mod_Helper.Extensions;\n" + loader;
                loader = loader.Replace("TowerModelLoader : IGameModelLoader", "TowersLoader");
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