using StardewValley.Menus;
using StardewValley;
using StardewValley.Objects;
using Microsoft.Xna.Framework;
using StardewModdingAPI;

namespace BigFridge.Patches
{
    internal static class ItemGrabMenuPatches
    {
        private static IMonitor Monitor = null!;

        internal static void Initialize(IMonitor monitor)
        {
            Monitor = monitor;
        }
        internal static void ItemGrabMenuPostfix(ItemGrabMenu __instance, Item sourceItem)
        {
            try
            {
                if (sourceItem is not Chest chest) return;
                if (chest.QualifiedItemId == "(BC)AlanBF.BigFridge")
                {
                    __instance._sourceItemInCurrentLocation = sourceItem != null && Game1.currentLocation.objects.Values.Contains(sourceItem);

                    Chest fridge = new(playerChest: true, chest.ItemId)
                    {
                        shakeTimer = 50,
                        SpecialChestType = (Chest.SpecialChestTypes)9
                    };

                    __instance.chestColorPicker = new DiscreteColorPicker(__instance.xPositionOnScreen, __instance.yPositionOnScreen - 64 - 42 - IClickableMenu.borderWidth * 2, chest.playerChoiceColor.Value, fridge);
                    fridge.playerChoiceColor.Value = DiscreteColorPicker.getColorFromSelection(__instance.chestColorPicker.colorSelection);

                    __instance.colorPickerToggleButton = new ClickableTextureComponent(new Rectangle(__instance.xPositionOnScreen + __instance.width, __instance.yPositionOnScreen + __instance.height / 3 - 64 + -160, 64, 64), Game1.mouseCursors, new Rectangle(119, 469, 16, 16), 4f)
                    {
                        hoverText = Game1.content.LoadString("Strings\\UI:Toggle_ColorPicker"),
                        myID = 27346,
                        downNeighborID = -99998,
                        leftNeighborID = 53921,
                        region = 15923
                    };

                    __instance.RepositionSideButtons();

                    if (__instance.chestColorPicker != null)
                    {
                        __instance.discreteColorPickerCC = new List<ClickableComponent>();
                        for (int j = 0; j < DiscreteColorPicker.totalColors; j++)
                        {
                            List<ClickableComponent> list = __instance.discreteColorPickerCC;
                            ClickableComponent obj = new(new Rectangle(__instance.chestColorPicker.xPositionOnScreen + IClickableMenu.borderWidth / 2 + j * 9 * 4, __instance.chestColorPicker.yPositionOnScreen + IClickableMenu.borderWidth / 2, 36, 28), "")
                            {
                                myID = j + 4343,
                                rightNeighborID = ((j < DiscreteColorPicker.totalColors - 1) ? (j + 4343 + 1) : (-1)),
                                leftNeighborID = ((j > 0) ? (j + 4343 - 1) : (-1)),
                                downNeighborID = ((__instance.ItemsToGrabMenu?.inventory.Count > 0) ? 53910 : 0)
                            };
                            list.Add(obj);
                        }
                    }

                    __instance.populateClickableComponentList();
                    if (Game1.options.SnappyMenus)
                    {
                        __instance.snapToDefaultClickableComponent();
                    }

                    __instance.SetupBorderNeighbors();
                }
            }
            catch (Exception ex)
            {
                Monitor.Log($"Failed in {nameof(ItemGrabMenuPostfix)}:\n{ex}", LogLevel.Error);
            }
        }

        internal static void setSourceItemPostfix(ItemGrabMenu __instance)
        {
            try
            {
                if (__instance.source != 1 || __instance.sourceItem is not Chest chest) return;
                if (chest.QualifiedItemId == "(BC)AlanBF.BigFridge")
                {
                    Chest fridge = new(playerChest: true, chest.ItemId);
                    __instance.chestColorPicker = new DiscreteColorPicker(__instance.xPositionOnScreen, __instance.yPositionOnScreen - 64 - IClickableMenu.borderWidth * 2, chest.playerChoiceColor.Value, fridge);
                    fridge.playerChoiceColor.Value = DiscreteColorPicker.getColorFromSelection(__instance.chestColorPicker.colorSelection);
                    __instance.chestColorPicker.yPositionOnScreen -= 42;

                    __instance.colorPickerToggleButton = new ClickableTextureComponent(new Rectangle(__instance.xPositionOnScreen + __instance.width, __instance.yPositionOnScreen + __instance.height / 3 - 64 + -160, 64, 64), Game1.mouseCursors, new Rectangle(119, 469, 16, 16), 4f)
                    {
                        hoverText = Game1.content.LoadString("Strings\\UI:Toggle_ColorPicker")
                    };
                    __instance.RepositionSideButtons();
                }
            }
            catch (Exception ex)
            {
                Monitor.Log($"Failed in {nameof(setSourceItemPostfix)}:\n{ex}", LogLevel.Error);
            }
        }
    }
}
