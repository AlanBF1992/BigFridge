using StardewModdingAPI;
using StardewValley.Locations;
using StardewValley;
using Microsoft.Xna.Framework;
using StardewValley.Objects;

namespace BigFridge.Patches
{
    internal static class ObjectPatches
    {
        private static readonly IMonitor Monitor = ModEntry.LogMonitor;

        internal static bool placementActionPrefix(StardewValley.Object __instance, ref bool __result, GameLocation location, int x, int y, Farmer? who = null)
        {
            if (__instance.QualifiedItemId != "(BC)AlanBF.BigFridge") return true;

            try
            {
                Vector2 placementTile = new(x / 64, y / 64);
                __instance.Location = location;
                __instance.TileLocation = placementTile;
                __instance.owner.Value = who?.UniqueMultiplayerID ?? Game1.player.UniqueMultiplayerID;

                // Things now
                if (location.objects.ContainsKey(placementTile))
                {
                    Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.13053"));
                    __result = false;
                    return false;
                }

                if (!location.TryGetMapPropertyAs("AllowMiniFridges", out bool parsed, required: false))
                {
                    if (location is FarmHouse farmHouse && farmHouse.upgradeLevel < 1)
                    {
                        Game1.showRedMessage(Game1.content.LoadString("Strings\\UI:MiniFridge_NoKitchen"));
                        __result = false;
                        return false;
                    }

                    parsed = location is FarmHouse || location is IslandFarmHouse;
                }

                if (!parsed)
                {
                    Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.13053"));
                    __result = false;
                    return false;
                }

                Chest bigFridge = new("AlanBF.BigFridge", placementTile, 1, 2)
                {
                    shakeTimer = 50
                };
                bigFridge.fridge.Value = true;
                location.objects.Add(placementTile, bigFridge);
                location.playSound("hammer");

                __result = true;
                return false;
            }
            catch (Exception ex)
            {
                Monitor.Log($"Failed in {nameof(placementActionPrefix)}:\n{ex}", LogLevel.Error);
                return true;
            }
        }
    }
}
