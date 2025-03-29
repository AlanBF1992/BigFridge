using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.ItemTypeDefinitions;
using StardewValley.Locations;
using StardewValley.Objects;

namespace BigFridge.Patches
{
    internal static class ChestPatches
    {
        private static readonly IMonitor Monitor = ModEntry.LogMonitor;

        internal static bool getActualCapacityPrefix(Chest __instance, ref int __result)
        {
            if (!__instance.fridge.Value || __instance.QualifiedItemId == "(BC)216") return true;

            try
            {
                if (__instance.QualifiedItemId == "(BC)AlanBF.BigFridge")
                {
                    __result = 70;
                    return false;
                }
                else if (!(ModEntry.Config.HouseFridgeProgressive && !GameStateQuery.CheckConditions("PLAYER_FARMHOUSE_UPGRADE Current 2")))
                {
                    if (__instance.Location is not FarmHouse baseHome)
                    {
                        if (__instance.Location is IslandFarmHouse islandHome && islandHome.fridgePosition != Point.Zero && __instance.TileLocation.ToPoint() == Point.Zero)
                        {
                            __instance.lidFrameCount.Value = 2;
                            __result = 70;
                            return false;
                        }
                    }
                    else if (baseHome.fridgePosition != Point.Zero && __instance.TileLocation.ToPoint() == Point.Zero)
                    {
                        __instance.lidFrameCount.Value = 2;
                        __result = 70;
                        return false;
                    }
                }
                
                return true;
            }
            catch (Exception ex)
            {
                Monitor.Log($"Failed in {nameof(getActualCapacityPrefix)}:\n{ex}", LogLevel.Error);
                return true;
            }
        }

        internal static bool ShowMenuPrefix(Chest __instance)
        {
            if (!__instance.fridge.Value) return true;

            try
            {

                if(__instance.SpecialChestType == (Chest.SpecialChestTypes)9) {
                    __instance.SpecialChestType = Chest.SpecialChestTypes.None;
                    Monitor.Log($"Chest Type Restored", LogLevel.Info);
                }

                return true;
                
            }
            catch (Exception ex)
            {
                Monitor.Log($"Failed in {nameof(ShowMenuPrefix)}:\n{ex}", LogLevel.Error);
                return true;
            }
        }

        internal static bool drawPrefix(Chest __instance, SpriteBatch spriteBatch, int x, int y, float alpha)
        {
            if (!__instance.playerChest.Value || (__instance.QualifiedItemId != "(BC)AlanBF.BigFridge" && __instance.QualifiedItemId != "(BC)216")) return true;

            try
            {
                float num = x;
                float num2 = y;
                float num3 = Math.Max(0f, ((num2 + 1f) * 64f - 24f) / 10000f) + num * 1E-05f;

                int baseLidFrame = __instance.ParentSheetIndex;
                int currentLidFrame = (int)__instance.GetInstanceField("currentLidFrame")!;

                if (__instance.QualifiedItemId == "(BC)216")
                {
                    baseLidFrame = 0;
                    currentLidFrame -= __instance.startingLidFrame.Value - 1;
                }

                ParsedItemData dataOrErrorItem = ItemRegistry.GetDataOrErrorItem(__instance.QualifiedItemId);
                Texture2D texture = dataOrErrorItem.GetTexture();

                if (__instance.playerChoiceColor.Value.Equals(Color.Black))
                {
                    int baseFridge = baseLidFrame;
                    int fridgeDoor = currentLidFrame;

                    Rectangle sourceBaseFridge = dataOrErrorItem.GetSourceRect(0, baseFridge);
                    Rectangle sourceOpenDoor = dataOrErrorItem.GetSourceRect(0, fridgeDoor);

                    //Base, Animación Puerta
                    spriteBatch.Draw(texture, Game1.GlobalToLocal(Game1.viewport, new Vector2(num * 64f + ((__instance.shakeTimer > 0) ? Game1.random.Next(-1, 2) : 0), (num2 - 1f) * 64f)), sourceBaseFridge, __instance.Tint * alpha, 0f, Vector2.Zero, 4f, SpriteEffects.None, num3);
                    spriteBatch.Draw(texture, Game1.GlobalToLocal(Game1.viewport, new Vector2(num * 64f, (num2 - 1f) * 64f + ((__instance.shakeTimer > 0) ? Game1.random.Next(-1, 2) : 0))), sourceOpenDoor, __instance.Tint * alpha * alpha, 0f, Vector2.Zero, 4f, SpriteEffects.None, num3 + 1E-05f);
                    return false;
                }

                int colorFridge = baseLidFrame + 3;
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
            catch (Exception ex)
            {
                Monitor.Log($"Failed in {nameof(drawPrefix)}:\n{ex}", LogLevel.Error);
                return true;
            }
        }

        internal static bool drawLocalPrefix(Chest __instance, SpriteBatch spriteBatch, int x, int y, float alpha, bool local)
        {
            if (!__instance.playerChest.Value || (__instance.QualifiedItemId != "(BC)AlanBF.BigFridge" && __instance.QualifiedItemId != "(BC)216")) return true;
            
            try
            {
                if (__instance.playerChoiceColor.Equals(Color.Black))
                {
                    ParsedItemData dataOrErrorItem = ItemRegistry.GetDataOrErrorItem(__instance.QualifiedItemId);
                    //Base
                    spriteBatch.Draw(dataOrErrorItem.GetTexture(), local ? new Vector2(x, y - 64) : Game1.GlobalToLocal(Game1.viewport, new Vector2(x * 64 + ((__instance.shakeTimer > 0) ? Game1.random.Next(-1, 2) : 0), (y - 1) * 64)), dataOrErrorItem.GetSourceRect(0,0), __instance.Tint * alpha, 0f, Vector2.Zero, 4f, SpriteEffects.None, local ? 0.89f : ((float)(y * 64 + 4) / 10000f));
                    return false;
                }

                ParsedItemData data = ItemRegistry.GetData(__instance.QualifiedItemId);
                if (data == null) return false;
                const int num = 3;

                Rectangle sourceRect = data.GetSourceRect(0, num);
                Rectangle sourceRect2 = data.GetSourceRect(0, num + 3);

                Texture2D texture = data.GetTexture();

                //Base, PostIt
                spriteBatch.Draw(texture, local ? new Vector2(x, y - 64) : Game1.GlobalToLocal(Game1.viewport, new Vector2(x * 64, (y - 1) * 64 + ((__instance.shakeTimer > 0) ? Game1.random.Next(-1, 2) : 0))), sourceRect, __instance.playerChoiceColor.Value * alpha, 0f, Vector2.Zero, 4f, SpriteEffects.None, local ? 0.9f : ((float)(y * 64 + 4) / 10000f));
                spriteBatch.Draw(texture, local ? new Vector2(x, y - 64) : Game1.GlobalToLocal(Game1.viewport, new Vector2(x * 64, (y - 1) * 64 + ((__instance.shakeTimer > 0) ? Game1.random.Next(-1, 2) : 0))), sourceRect2, Color.White * alpha, 0f, Vector2.Zero, 4f, SpriteEffects.None, local ? 0.89f : ((float)(y * 64 + 4) / 10000f));
                return false;
            }
            catch (Exception ex)
            {
                Monitor.Log($"Failed in {nameof(drawLocalPrefix)}:\n{ex}", LogLevel.Error);
                return true;
            }
        }

        internal static bool performObjectDropInActionPrefix(Chest __instance, Item dropInItem, bool probe, Farmer who, ref bool __result)
        {
            try
            {
                if (dropInItem == null || dropInItem.QualifiedItemId != "(BC)AlanBF.BigFridge" || __instance.QualifiedItemId != "(BC)216" || __instance.Location == null || probe) { return true; }

                if (__instance.GetMutex().IsLocked())
                {
                    __result = false;
                    return false;
                }

                Chest fridge = new(who.CurrentItem.ItemId, __instance.TileLocation, 1, 2)
                {
                    shakeTimer = 50,
                };

                fridge.fridge.Value = true;
                fridge.netItems.Value = __instance.netItems.Value;
                fridge.playerChoiceColor.Value = __instance.playerChoiceColor.Value;
                fridge.Tint = __instance.Tint;
                fridge.modData.CopyFrom(__instance.modData);
                GameLocation location = __instance.Location;
                location.Objects.Remove(__instance.TileLocation);
                location.Objects.Add(__instance.TileLocation, fridge);
                Game1.createMultipleItemDebris(ItemRegistry.Create(__instance.QualifiedItemId), __instance.TileLocation * 64f + new Vector2(32f), -1);
                location.playSound("axchop");

                __result = true;
                return false;
            }
            catch (Exception ex)
            {
                Monitor.Log($"Failed in {nameof(performObjectDropInActionPrefix)}:\n{ex}", LogLevel.Error);
                return true;
            }
        }
    }
}
