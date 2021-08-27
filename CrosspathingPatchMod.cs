using Assets.Scripts.Models.Towers;
using BTD_Mod_Helper;

namespace UltimateCrosspathing
{
    public abstract class CrosspathingPatchMod : BloonsTD6Mod
    {
        public abstract void Postmerge(TowerModel towerModel, string baseId, int topPath, int middlePath, int bottomPath);
    }
}