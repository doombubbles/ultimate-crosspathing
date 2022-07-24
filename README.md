<a href="https://github.com/doombubbles/ultimate-crosspathing/raw/main/UltimateCrosspathing.dll"><img align="right" alt="Download" height="75" src="https://github.com/doombubbles/BTD6-Mods/blob/main/download.png?raw=true"></a>

# Ultimate Crosspathing

## Requires [BTD6 Mod Helper](https://github.com/gurrenm3/BTD-Mod-Helper/releases/latest)

<img alt="Screenshot" height="400" src="https://github.com/doombubbles/ultimate-crosspathing/blob/main/screenshot.png?raw=true"/>

# Quick Answers

### It says "Failed to load Assembly ... No Compaitibility Layer Found!"

[BloonsTD6 Mod Helper](https://github.com/gurrenm3/BTD-Mod-Helper/releases/latest) is not installed correctly. Make sure that you have the latest version and that you've extracted the zip file so that there's "BloonsTD6 Mod Helper.dll" in your mods folder with that exact name.

### The cost of an upgrade is some some ridiculously high number?!?

That's the Ninja Kiwi way of saying the tower doesn't exist for you to upgrade to, which in this case means Mod Helper isn't installed correctly. See previous answer.

### How do I get 555s?

Open the Bloons TD 6 Settings screen, click the Mod Settings button in the bottom right, click on Ultimate Crosspathing in the mods list, scroll all the way down the options and enable "Execute Order 66".

### Loading the mod takes more like 30 minutes than 30 seconds

Make sure that the "Debug: Regenerate Towers" setting near the bottom of the options is DISABLED. If you don't want to sit through the load time, you can change the `RegenerateTowers` value in the  "...\Mods\BloonsTD6 Mod Helper\Mod Settings\Ultimate Crosspathing.json" file to `false`.

# Summary

This is a prototype I've been working on for algorithmically generating more crosspathing combinations.

With this mod, instead of your Towers being restricted to 5 Upgrades in one path and 2 in another, you can take your
upgrades in any path.

The default mode is keep the restriction of just 7 total upgrades so you can do 430s, 331s, 322s, 511s, etc, but if
you're really adventurous, there's a setting at the bottom of the options list you can enable to allow for all 15
Upgrades. You can also individually toggle which Towers get Ultimate Crosspathing, and changing any settings no longer
requires a restart.