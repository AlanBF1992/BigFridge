using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.ItemTypeDefinitions;
using StardewValley.Objects;

namespace BigFridge.Compatibility.ConvenientInventory.Patches
{
    internal static class TypedChestPatches
    {
        internal static bool DrawNormalChestTooltipPrefix(dynamic __instance, SpriteBatch spriteBatch, int x, int y, ref int __result)
        {
            Chest chest = __instance.Chest;

            if (chest.QualifiedItemId != "(BC)AlanBF.BigFridge" && (chest.QualifiedItemId != "(BC)216" || !ModEntry.Config.ReskinMiniFridge))
            {
                return true;
            }

            ParsedItemData dataOrErrorItem = ItemRegistry.GetDataOrErrorItem(chest.QualifiedItemId);
            Texture2D texture = dataOrErrorItem.GetTexture();

            spriteBatch.Draw(texture,
                new Vector2(x, y),
                chest.playerChoiceColor.Value.Equals(Color.Black) ? dataOrErrorItem.GetSourceRect(0, 0) : dataOrErrorItem.GetSourceRect(0, 3),
                chest.playerChoiceColor.Value.Equals(Color.Black) ? chest.Tint : chest.playerChoiceColor.Value,
                0f, Vector2.Zero, 2f, SpriteEffects.None, 1f);

            spriteBatch.Draw(texture,
                new Vector2(x, y),
                new Rectangle(0, 32, 16, 32),
                Color.White,
                0f, Vector2.Zero, 2f, SpriteEffects.None, 1f - 1E-05f);

            __result = 0;

            return false;
        }
    }
}
