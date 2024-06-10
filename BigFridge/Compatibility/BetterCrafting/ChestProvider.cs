using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Inventories;
using StardewValley.Network;
using StardewValley.Objects;
using System.Diagnostics.CodeAnalysis;
using SObject = StardewValley.Object;

namespace BigFridge.Compatibility.BetterCrafting
{
    public class ChestProvider : IInventoryProvider
    {
        public bool CanExtractItems(object obj, GameLocation? location, Farmer? who)
        {
            return obj is Chest && IsValid(obj, location, who);
        }
        public bool CanInsertItems(object obj, GameLocation? location, Farmer? who)
        {
            return obj is Chest && IsValid(obj, location, who);
        }

        public void CleanInventory(object obj, GameLocation? location, Farmer? who)
        {
            if (obj is Chest tobj) tobj.clearNulls();
        }

        public int GetActualCapacity(object obj, GameLocation? location, Farmer? who)
        {
            return obj is Chest tobj ? tobj.GetActualCapacity() : 0;
        }

        public IInventory? GetInventory(object obj, GameLocation? location, Farmer? who)
        {
            if (obj is not Chest tobj) return null;
            if (who == null) return tobj.Items;
            return tobj.GetItemsForPlayer(who.UniqueMultiplayerID);
        }

        public IList<Item?>? GetItems(object obj, GameLocation? location, Farmer? who)
        {
            if (obj is not Chest tobj) return null;
            if (who == null) return tobj.Items;
            return tobj.GetItemsForPlayer(who.UniqueMultiplayerID);
        }

        public Rectangle? GetMultiTileRegion(object obj, GameLocation? location, Farmer? who)
        {
            return null;
        }

        public NetMutex? GetMutex(object obj, GameLocation? location, Farmer? who)
        {
            if (obj is not Chest tobj) return null;
            return tobj.GetMutex();
        }

        public Vector2? GetTilePosition(object obj, GameLocation? location, Farmer? who)
        {
            if (obj is not Chest tobj) return null;

            location ??= tobj.Location;

            if (location is not null && location.GetFridge(false) == tobj && location.GetFridgePosition() is Point pos)
                return new Vector2(pos.X, pos.Y);

            return tobj.TileLocation;
        }

        public bool IsValid(object obj, GameLocation? location, Farmer? who)
        {
            if (obj is not Chest tobj) return false;

            if (location != null)
            {
                Vector2? pos = GetTilePosition(obj, location, who);
                if (!pos.HasValue)
                    return false;
                if (!GetObjectAtPosition(location, pos.Value, out var sobj) || sobj != tobj)
                    return false;
            }

            return true;
        }

        internal static bool GetObjectAtPosition(GameLocation location, Vector2 position, [NotNullWhen(true)] out SObject? obj)
        {
            if (location.GetFridgePosition() is Point pos && pos.X == position.X && pos.Y == position.Y)
            {
                obj = location.GetFridge(false);
                return obj is not null;
            }

            return location.Objects.TryGetValue(position, out obj);
        }
    }
}
