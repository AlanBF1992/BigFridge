using StardewModdingAPI;
using StardewValley.Menus;
using StardewValley.Objects;
using StardewValley;
using StardewValley.Locations;
using Microsoft.Xna.Framework;

namespace BigFridge.Patches
{
    internal static class ChestPatches
    {
        private static IMonitor Monitor = null!;

        internal static void Initialize(IMonitor monitor)
        {
            Monitor = monitor;
        }

        internal static bool getActualCapacityPrefix(Chest __instance, ref int __result)
        {
            try
            {
                if (__instance.specialChestType.Value == (Chest.SpecialChestTypes)9)
                {
                    __result = 70;
                    return false;
                }
                if (ModEntry.Config.HouseFridgeProgressive && !GameStateQuery.CheckConditions("PLAYER_FARMHOUSE_UPGRADE Current 2"))
                {
                    return true;
                }
                if (__instance.fridge.Value)
                {
                    if (__instance.Location is not FarmHouse home2)
                    {
                        if (__instance.Location is IslandFarmHouse home && home.fridgePosition != Point.Zero && __instance.TileLocation.ToPoint() == Point.Zero)
                        {
                            __result = 70;
                            return false;
                        }
                    }
                    else if (home2.fridgePosition != Point.Zero && __instance.TileLocation.ToPoint() == Point.Zero)
                    {
                        __result = 70;
                        return false;
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Monitor.Log($"Failed in {nameof(getActualCapacityPrefix)}:\n{ex}", LogLevel.Error);
                return true; // run original logic
            }
        }

        internal static bool ShowMenuPrefix(Chest __instance)
        {
            try
            {
                ItemGrabMenu? oldMenu = Game1.activeClickableMenu as ItemGrabMenu;

                if (__instance.specialChestType.Value == (Chest.SpecialChestTypes)9)
                {
                    Game1.activeClickableMenu = new ItemGrabMenu(__instance.GetItemsForPlayer(), reverseGrab: false, showReceivingMenu: true, InventoryMenu.highlightAllItems, __instance.grabItemFromInventory, null, __instance.grabItemFromChest, snapToBottom: false, canBeExitedWithKey: true, playRightClickSound: true, allowRightClick: true, showOrganizeButton: true, 1, __instance, -1, __instance); // El anterior instance, hace que no muestre nada

                    if (oldMenu != null && Game1.activeClickableMenu is ItemGrabMenu newMenu)
                    {
                        // Ni idea, redefine los sonidos, pero encuentro que son los mismos
                        newMenu.inventory.moveItemSound = oldMenu.inventory.moveItemSound;
                        newMenu.inventory.highlightMethod = oldMenu.inventory.highlightMethod;
                    }

                    return false;
                }
                if (ModEntry.Config.HouseFridgeProgressive && !GameStateQuery.CheckConditions("PLAYER_FARMHOUSE_UPGRADE Current 2"))
                {
                    return true;
                }

                /***************
                 * Home Fridges
                 ***************/
                // Change their SpecialChestType
                if (__instance.fridge.Value)
                {
                    if (__instance.Location is not FarmHouse home2)
                    {
                        // Ginger Island
                        if (__instance.Location is IslandFarmHouse home && home.fridgePosition != Point.Zero && __instance.TileLocation.ToPoint() == Point.Zero)
                        {
                            __instance.lidFrameCount.Value = 2;
                            __instance.SpecialChestType = (Chest.SpecialChestTypes)9;

                            Game1.activeClickableMenu = new ItemGrabMenu(__instance.GetItemsForPlayer(), reverseGrab: false, showReceivingMenu: true, InventoryMenu.highlightAllItems, __instance.grabItemFromInventory, null, __instance.grabItemFromChest, snapToBottom: false, canBeExitedWithKey: true, playRightClickSound: true, allowRightClick: true, showOrganizeButton: true, 1, __instance, -1, __instance); // El anterior instance, hace que no muestre nada

                            if (oldMenu != null && Game1.activeClickableMenu is ItemGrabMenu newMenu)
                            {
                                // Ni idea, redefine los sonidos, pero encuentro que son los mismos
                                newMenu.inventory.moveItemSound = oldMenu.inventory.moveItemSound;
                                newMenu.inventory.highlightMethod = oldMenu.inventory.highlightMethod;
                            }
                            return false;
                        }
                    }
                    // Farmh
                    else if (home2.fridgePosition != Point.Zero && __instance.TileLocation.ToPoint() == Point.Zero)
                    {
                        __instance.lidFrameCount.Value = 2;
                        __instance.SpecialChestType = (Chest.SpecialChestTypes)9;

                        Game1.activeClickableMenu = new ItemGrabMenu(__instance.GetItemsForPlayer(), reverseGrab: false, showReceivingMenu: true, InventoryMenu.highlightAllItems, __instance.grabItemFromInventory, null, __instance.grabItemFromChest, snapToBottom: false, canBeExitedWithKey: true, playRightClickSound: true, allowRightClick: true, showOrganizeButton: true, 1, __instance, -1, __instance); // El anterior instance, hace que no muestre nada

                        if (oldMenu != null && Game1.activeClickableMenu is ItemGrabMenu newMenu)
                        {
                            // Ni idea, redefine los sonidos, pero encuentro que son los mismos
                            newMenu.inventory.moveItemSound = oldMenu.inventory.moveItemSound;
                            newMenu.inventory.highlightMethod = oldMenu.inventory.highlightMethod;
                        }

                        return false;
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Monitor.Log($"Failed in {nameof(getActualCapacityPrefix)}:\n{ex}", LogLevel.Error);
                return true; // run original logic
            }
        }
    }
}
