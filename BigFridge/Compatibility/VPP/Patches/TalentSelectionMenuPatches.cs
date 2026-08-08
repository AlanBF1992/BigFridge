using HarmonyLib;
using StardewModdingAPI;
using StardewValley;
using System.Reflection;

namespace BigFridge.Compatibility.VPP.Patches
{
    internal static class TalentSelectionMenuPatches
    {
        internal readonly static IMonitor LogMonitor = ModEntry.LogMonitor;
        internal static readonly FieldInfo TalentMiniFridgeBigSpace = AccessTools.Field("VanillaPlusProfessions.Constants:Talent_MiniFridgeBigSpace");
        internal static readonly MethodInfo VPPCurrentPlayerHasTalent = AccessTools.Method("VanillaPlusProfessions.Utilities.TalentUtility:CurrentPlayerHasTalent");
        internal static bool UnlockedSkill { get; set; }
        internal static bool UnlockedMiniRecipe { get; set; }
        internal static bool UnlockedBigRecipes { get; set; }

        internal static void receiveLeftClickPostfix()
        {
            if (!UnlockedSkill && (bool)VPPCurrentPlayerHasTalent.Invoke(null, [TalentMiniFridgeBigSpace.GetValue(null), -1, null, true])!)
            {
                UnlockedSkill = true;

                if (Game1.player.HouseUpgradeLevel >= 1)
                {
                    Game1.player.craftingRecipes.TryAdd("Mini-Fridge", 0);
                    UnlockedMiniRecipe = true;

                    if (GameStateQuery.CheckConditions(VanillaLoader.bigFridgeUnlockCondition))
                    {
                        Game1.player.craftingRecipes.TryAdd("Big Fridge 1", 0);
                        Game1.player.craftingRecipes.TryAdd("Big Fridge 2", 0);
                        UnlockedBigRecipes = true;
                    }

                    //ModEntry.ModHelper.GameContent.InvalidateCache("Data\\CraftingRecipes");
                }
            }
        }
    }
}
