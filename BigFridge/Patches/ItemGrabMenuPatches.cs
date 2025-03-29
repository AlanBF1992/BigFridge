using StardewValley;
using StardewValley.Menus;
using StardewValley.Objects;

namespace BigFridge.Patches
{
    internal static class ItemGrabMenuPatches
    {
        internal static bool CanHaveColorPickerPrefix(ItemGrabMenu __instance, ref bool __result)
        {
            if (__instance.source == 1 && __instance.sourceItem is Chest chest && chest.QualifiedItemId is "(BC)216" or "(BC)AlanBF.BigFridge")
            {
                __result = true;
                return false;
            }
            return true;
        }

        internal static void setSourceItemPostfix(ItemGrabMenu __instance, Item item)
        {
            if (__instance.sourceItem is not Chest fridge || fridge.QualifiedItemId != "(BC)AlanBF.BigFridge") return;

            __instance.chestColorPicker.yPositionOnScreen -= 42;
        }
    }
}
