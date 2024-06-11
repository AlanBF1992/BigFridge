using StardewModdingAPI;
using StardewValley.Menus;
using StardewValley.Objects;
using StardewValley;
using StardewValley.Locations;
using Microsoft.Xna.Framework;
using StardewValley.ItemTypeDefinitions;
using Microsoft.Xna.Framework.Graphics;

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

                /**************
                * Item Fridges
                ***************/
                if (__instance.specialChestType.Value == (Chest.SpecialChestTypes)9 || __instance.QualifiedItemId == "(BC)216")
                {
                    Game1.activeClickableMenu = new ItemGrabMenu(__instance.GetItemsForPlayer(), reverseGrab: false, showReceivingMenu: true, InventoryMenu.highlightAllItems, __instance.grabItemFromInventory, null, __instance.grabItemFromChest, snapToBottom: false, canBeExitedWithKey: true, playRightClickSound: true, allowRightClick: true, showOrganizeButton: true, 1, __instance, -1, __instance);

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
                    // Farm
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
                Monitor.Log($"Failed in {nameof(ShowMenuPrefix)}:\n{ex}", LogLevel.Error);
                return true; // run original logic
            }
        }

        internal static bool drawPrefix(Chest __instance, SpriteBatch spriteBatch, int x, int y, float alpha)
        {
            if (!__instance.playerChest.Value) return true;
            if (__instance.QualifiedItemId != "(BC)AlanBF.BigFridge" && __instance.QualifiedItemId != "(BC)216") return true;

            float num = x;
            float num2 = y;

            if (__instance.localKickStartTile.HasValue)
            {
                num = Utility.Lerp(__instance.localKickStartTile.Value.X, num, __instance.kickProgress);
                num2 = Utility.Lerp(__instance.localKickStartTile.Value.Y, num2, __instance.kickProgress);
            }

            float num3 = Math.Max(0f, ((num2 + 1f) * 64f - 24f) / 10000f) + num * 1E-05f;

            if (__instance.localKickStartTile.HasValue)
            {
                spriteBatch.Draw(Game1.shadowTexture, Game1.GlobalToLocal(Game1.viewport, new Vector2((num + 0.5f) * 64f, (num2 + 0.5f) * 64f)), Game1.shadowTexture.Bounds, Color.Black * 0.5f, 0f, new Vector2(Game1.shadowTexture.Bounds.Center.X, Game1.shadowTexture.Bounds.Center.Y), 4f, SpriteEffects.None, 0.0001f);
                num2 -= (float)Math.Sin((double)__instance.kickProgress * Math.PI) * 0.5f;
            }

            int currentLidFrame = (int)__instance.GetInstanceField("currentLidFrame")!;

            ParsedItemData dataOrErrorItem = ItemRegistry.GetDataOrErrorItem(__instance.QualifiedItemId);
            Texture2D texture = dataOrErrorItem.GetTexture();

            int baseFridge = __instance.ParentSheetIndex;
            int fridgeDoor = currentLidFrame;

            Rectangle sourceBaseFridge = dataOrErrorItem.GetSourceRect(0, baseFridge);
            Rectangle sourceOpenDoor = dataOrErrorItem.GetSourceRect(0, fridgeDoor);

            if (__instance.playerChoiceColor.Value.Equals(Color.Black))
            {
                //Base, Animación Puerta
                spriteBatch.Draw(texture, Game1.GlobalToLocal(Game1.viewport, new Vector2(num * 64f + ((__instance.shakeTimer > 0) ? Game1.random.Next(-1, 2) : 0), (num2 - 1f) * 64f)), sourceBaseFridge, __instance.Tint * alpha, 0f, Vector2.Zero, 4f, SpriteEffects.None, num3);
                spriteBatch.Draw(texture, Game1.GlobalToLocal(Game1.viewport, new Vector2(num * 64f, (num2 - 1f) * 64f + ((__instance.shakeTimer > 0) ? Game1.random.Next(-1, 2) : 0))), sourceOpenDoor, __instance.Tint * alpha * alpha, 0f, Vector2.Zero, 4f, SpriteEffects.None, num3 + 1E-05f);
                return false;
            }

            int colorFridge = __instance.ParentSheetIndex + 3;
            int colorFridgeDoor = currentLidFrame + 3;
            int fridgePostIts = currentLidFrame + 6;

            Rectangle sourceColorFridge = dataOrErrorItem.GetSourceRect(0, colorFridge);
            Rectangle sourcePostIt = dataOrErrorItem.GetSourceRect(0, fridgePostIts);
            Rectangle sourceColorFridgeDoor = dataOrErrorItem.GetSourceRect(0, colorFridgeDoor);

            //Base, PostIt, Puerta
            spriteBatch.Draw(texture, Game1.GlobalToLocal(Game1.viewport, new Vector2(num * 64f, (num2 - 1f) * 64f)), sourceColorFridge, __instance.playerChoiceColor.Value * alpha, 0f, Vector2.Zero, 4f, SpriteEffects.None, num3);
            spriteBatch.Draw(texture, Game1.GlobalToLocal(Game1.viewport, new Vector2(num * 64f, (num2 - 1f) * 64f + ((__instance.shakeTimer > 0) ? Game1.random.Next(-1, 2) : 0))), sourcePostIt, Color.White * alpha * alpha, 0f, Vector2.Zero, 4f, SpriteEffects.None, num3 + 2E-05f);
            spriteBatch.Draw(texture, Game1.GlobalToLocal(Game1.viewport, new Vector2(num * 64f, (num2 - 1f) * 64f + ((__instance.shakeTimer > 0) ? Game1.random.Next(-1, 2) : 0))), sourceColorFridgeDoor, __instance.playerChoiceColor.Value * alpha * alpha, 0f, Vector2.Zero, 4f, SpriteEffects.None, num3 + 1E-05f);
            return false;
        }

        internal static bool drawLocalPrefix(Chest __instance, SpriteBatch spriteBatch, int x, int y, float alpha, bool local)
        {
            if (!__instance.playerChest.Value) return true;
            if (__instance.QualifiedItemId != "(BC)AlanBF.BigFridge" && __instance.QualifiedItemId != "(BC)216") return true;

            if (__instance.playerChoiceColor.Equals(Color.Black))
            {
                ParsedItemData dataOrErrorItem = ItemRegistry.GetDataOrErrorItem(__instance.QualifiedItemId);
                //Base
                spriteBatch.Draw(dataOrErrorItem.GetTexture(), local ? new Vector2(x, y - 64) : Game1.GlobalToLocal(Game1.viewport, new Vector2(x * 64 + ((__instance.shakeTimer > 0) ? Game1.random.Next(-1, 2) : 0), (y - 1) * 64)), dataOrErrorItem.GetSourceRect(), __instance.Tint * alpha, 0f, Vector2.Zero, 4f, SpriteEffects.None, local ? 0.89f : ((float)(y * 64 + 4) / 10000f));
                return false;
            }

            ParsedItemData data = ItemRegistry.GetData(__instance.QualifiedItemId);
            if (data == null) return false;
            int num = __instance.ParentSheetIndex + 3;

            Rectangle sourceRect = data.GetSourceRect(0, num);
            Rectangle sourceRect2 = data.GetSourceRect(0, num + 3);

            Texture2D texture = data.GetTexture();

            //Base, PostIt
            spriteBatch.Draw(texture, local ? new Vector2(x, y - 64) : Game1.GlobalToLocal(Game1.viewport, new Vector2(x * 64, (y - 1) * 64 + ((__instance.shakeTimer > 0) ? Game1.random.Next(-1, 2) : 0))), sourceRect, __instance.playerChoiceColor.Value * alpha, 0f, Vector2.Zero, 4f, SpriteEffects.None, local ? 0.9f : ((float)(y * 64 + 4) / 10000f));
            spriteBatch.Draw(texture, local ? new Vector2(x, y - 64) : Game1.GlobalToLocal(Game1.viewport, new Vector2(x * 64, (y - 1) * 64 + ((__instance.shakeTimer > 0) ? Game1.random.Next(-1, 2) : 0))), sourceRect2, Color.White * alpha, 0f, Vector2.Zero, 4f, SpriteEffects.None, local ? 0.89f : ((float)(y * 64 + 4) / 10000f));

            return false;
        }
    }
}
