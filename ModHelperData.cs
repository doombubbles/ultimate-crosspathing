using System;
using System.Linq;
using BTD_Mod_Helper;
using MelonLoader;

namespace UltimateCrosspathing
{
    public static class ModHelperData
    {
        public const string Version = "1.3.2";
        public const string Name = "Ultimate Crosspathing";

        public const string Description =
            "Instead of your Towers being restricted to 5 Upgrades in one path and 2 in another, you can take your upgrades in any path, optionally up to all 15!";

        public const string RepoOwner = "doombubbles";
        public const string RepoName = "ultimate-crosspathing";


        public static bool CheckMelonLoader055()
        {
            try
            {
                MelonMod.RegisteredMelons.Count();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool CheckModHelper3()
        {
            try
            {
                ModHelper.Msg<UltimateCrosspathingMod>("Yay, you're actually on Mod Helper 3.0");
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}