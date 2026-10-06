using BigFridge.Compatibility.UnlimitedChestColours.Patches;
using HarmonyLib;
using StardewModdingAPI;

namespace BigFridge.Compatibility.UnlimitedChestColours
{
    internal static class UCCLoader
    {
        internal static void Loader(IModHelper _, Harmony harmony)
        {
            UnlimitedChestColoursPatches(harmony);
        }

        internal static void UnlimitedChestColoursPatches(Harmony harmony)
        {
            // Set which chest or fridges to show
            harmony.Patch(
                original: AccessTools.Method("UnlimitedChestColours.ModEntry:OpenColourPicker"),
                transpiler: new HarmonyMethod(typeof(ModEntryPatches), nameof(ModEntryPatches.OpenColourPickerTranspiler))
            );

            // Configure the draw for fridges
            harmony.Patch(
                original: AccessTools.Method("UnlimitedChestColours.ModEntry:drawChest"),
                transpiler: new HarmonyMethod(typeof(ModEntryPatches), nameof(ModEntryPatches.drawChestTranspiler))
            );
        }
    }
}
