using StardewModdingAPI;
using StardewValley.Objects;
using StardewValley;
using Microsoft.Xna.Framework;
using xTile.Dimensions;

namespace BigFridge.Patches
{
    internal static class GameLocationPatches
    {
        private static IMonitor Monitor = null!;

        internal static void Initialize(IMonitor monitor)
        {
            Monitor = monitor;
        }

        internal static bool checkActionPrefix(GameLocation __instance, Location tileLocation, Farmer who, ref bool __result)
        {
            try
            {
                Vector2 tilePos = new(tileLocation.X, tileLocation.Y);
                if (__instance.objects.TryGetValue(tilePos, out var obj)) // Test if there is another object in position
                {
                    // If that object is a fridge and the item I want to put in there is a BigFridge
                    if (who.CurrentItem?.QualifiedItemId == "(BC)AlanBF.BigFridge" && obj.QualifiedItemId == "(BC)216")
                    {
                        StardewValley.Object old_held_object = obj.heldObject.Value;
                        obj.heldObject.Value = null;
                        bool probe_returned_true = false;

                        // 1st performObjectDropInAction replaced
                        GameLocation location = obj.Location;
                        if (location != null)
                        {
                            probe_returned_true = true;
                        }

                        obj.heldObject.Value = old_held_object;
                        bool perform_returned_true = false;

                        // 2nd performObjectDropInAction replaced
                        location = obj.Location;
                        if (location != null)
                        {
                            Chest fridge = new(who.CurrentItem.ItemId, obj.TileLocation, 1, 2)
                            {
                                SpecialChestType = (Chest.SpecialChestTypes)9
                            };
                            fridge.fridge.Value = true;
                            location.Objects.Remove(obj.TileLocation);
                            fridge.netItems.Value = ((Chest)obj).netItems.Value;
                            location.Objects.Add(obj.TileLocation, fridge);

                            Game1.createMultipleItemDebris(ItemRegistry.Create(obj.QualifiedItemId), (obj.TileLocation * 64f) + new Vector2(32f), -1);

                            perform_returned_true = true;
                        }
                        if ((probe_returned_true || perform_returned_true) && who.isMoving())
                        {
                            Game1.haltAfterCheck = false;
                        }
                        if (who.ignoreItemConsumptionThisFrame)
                        {
                            __result = true;
                            return false;
                        }
                        if (perform_returned_true)
                        {
                            who.reduceActiveItemByOne();
                            __result = true;
                            return false;
                        }
                        __result = obj.checkForAction(who) || probe_returned_true;
                        return false;
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Monitor.Log($"Failed in {nameof(checkActionPrefix)}:\n{ex}", LogLevel.Error);
                return true; // run original logic
            }
        }
    }
}
