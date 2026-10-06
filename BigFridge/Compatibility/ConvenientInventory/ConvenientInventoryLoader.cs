using BigFridge.Compatibility.ConvenientInventory.Patches;
using HarmonyLib;
using StardewModdingAPI;

namespace BigFridge.Compatibility.ConvenientInventory
{
    internal static class ConvenientInventoryLoader
    {
        internal static void Loader(IModHelper _, Harmony harmony)
        {
            ConvenientInventoryPatches(harmony);
        }

        internal static void ConvenientInventoryPatches(Harmony harmony)
        {
            harmony.Patch(
                original: AccessTools.Method("ConvenientInventory.TypedChests.TypedChest:DrawNormalChestTooltip"),
                prefix: new HarmonyMethod(typeof(TypedChestPatches), nameof(TypedChestPatches.DrawNormalChestTooltipPrefix))
            );
        }
    }
}
