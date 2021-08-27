using System.Collections.Generic;
using Assets.Scripts.Models.Towers;
using MelonLoader;
using UltimateCrosspathing;

[assembly: MelonInfo(typeof(CrosspathingPatchTemplate.CrosspathingPatchTemplate),
    "Crosspathing Patch Template", "1.0.0", "doombubbles")]
[assembly: MelonGame("Ninja Kiwi", "BloonsTD6")]

namespace CrosspathingPatchTemplate
{
    public class CrosspathingPatchTemplate : CrosspathingPatchMod
    {
        public override void Postmerge(TowerModel towerModel, string baseId, int topPath, int middlePath,
            int bottomPath)
        {
            if (baseId == TowerType.DartMonkey && topPath >= 3 && middlePath >= 3)
            {
                towerModel.baseId = TowerType.DartMonkey;
                MelonLogger.Msg("I'm doing my part!");
            }
        }

        public override void ModifyPathPriorities(Dictionary<string, (int, int, int)> pathPriorities)
        {
            pathPriorities[TowerType.TackShooter] = (2, 0, 1);
        }
    }
}