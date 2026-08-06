using System;
using BTD_Mod_Helper.Api.Commands;
#if DEBUG
using System.Collections.Generic;
using System.Linq;
using CommandLine;
#endif

namespace UltimateCrosspathing;

#if DEBUG
internal class GenerateByteLoadersCommand : ModCommand<GenerateCommand>
{
    public override string Command => "uc";
    public override string Help => "Generate Ultimate Crosspathing byte loaders, optionally for specific towers";

    [Value(0, Required = false, MetaName = "TowerIds",
        HelpText = "Tower base IDs to generate. If omitted, generates every enabled tower.")]
    public IEnumerable<string> TowerIds { get; set; } = [];

    public override bool Execute(ref string resultText)
    {
        try
        {
            return LoadInfo.ExportTowers(TowerIds, out resultText);
        }
        catch (Exception e)
        {
            ModHelper.Error<UltimateCrosspathingMod>(e);
            resultText = e.Message;
            return false;
        }
    }

    public override IEnumerable<string> SuggestionsForValue(int index) =>
        GetContent<LoadInfo>().Select(info => info.Name);

    public static string[] GetBatchTowerIds()
    {
        var args = Environment.GetCommandLineArgs();
        for (var i = 0; i < args.Length - 1; i++)
        {
            if (!args[i].Equals("generate", StringComparison.OrdinalIgnoreCase) ||
                !args[i + 1].Equals("uc", StringComparison.OrdinalIgnoreCase)) continue;

            return args.Skip(i + 2)
                .TakeWhile(arg => !arg.StartsWith('-'))
                .ToArray();
        }

        return [];
    }
}
#endif
