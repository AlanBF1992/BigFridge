using BigFridge.Compatibility.VPP.Patches;
using HarmonyLib;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace BigFridge.Compatibility.VPP
{
    internal static class VPPLoader
    {
        private static bool Unpatcher => false;

        internal static void Loader(IModHelper helper, Harmony harmony)
        {
            VPPPatches(harmony);

            helper.Events.Content.AssetRequested += OnAssetRequested;
            helper.Events.GameLoop.SaveLoaded += CheckUnlockedRecipes;
            helper.Events.GameLoop.ReturnedToTitle += DeleteDayEvent;
        }

        private static void VPPPatches(Harmony harmony)
        {
            //Change what the fridge thing do
            harmony.Patch(
                original: AccessTools.Method("VanillaPlusProfessions.Talents.Patchers.MiscPatcher:SetSpecialChestType_Postfix"),
                prefix: new HarmonyMethod(AccessTools.PropertyGetter(typeof(VPPLoader), nameof(Unpatcher)))
            );

            harmony.Patch(
                original: AccessTools.Method("VanillaPlusProfessions.ModEntry:OnButtonPressed"),
                transpiler: new HarmonyMethod(typeof(ModEntryPatches), nameof(ModEntryPatches.OnButtonPressedTranspiler))
            );

            harmony.Patch(
                original: AccessTools.Method("VanillaPlusProfessions.DayStartHandler:OnDayStarted"),
                transpiler: new HarmonyMethod(typeof(DayStartHandlerPatches), nameof(DayStartHandlerPatches.OnDayStartedTranspiler))
            );

            harmony.Patch(
                original: AccessTools.Method("VanillaPlusProfessions.DisplayHandler:OnMenuChanged"),
                transpiler: new HarmonyMethod(typeof(DisplayHandlerPatches), nameof(DisplayHandlerPatches.OnMenuChangedTranspiler))
            );

            //Reset assets
            harmony.Patch(
                original: AccessTools.Method("VanillaPlusProfessions.Talents.UI.TalentSelectionMenu:receiveLeftClick"),
                postfix: new HarmonyMethod(typeof(TalentSelectionMenuPatches), nameof(TalentSelectionMenuPatches.receiveLeftClickPostfix))
            );
        }

        private static void OnAssetRequested(object? sender, AssetRequestedEventArgs e)
        {
            if (e.NameWithoutLocale.IsEquivalentTo("Data/CraftingRecipes"))
            {
                e.Edit(rawInfo =>
                {
                    var data = rawInfo.AsDictionary<string, string>().Data;

                    data["Mini-Fridge"] = "(BC)130 1 84 1 334 1/Home/(BC)216 1/true/none/";
                    data["Big Fridge 1"] = "(BC)BigChest 1 84 1 334 2 336 1/Home/(BC)AlanBF.BigFridge 1/true/none/";
                    data["Big Fridge 2"] = "(BC)216 1 334 1 336 1/Home/(BC)AlanBF.BigFridge 1/true/none/";
                });
            }
        }

        private static void CheckUnlockedRecipes(object? sender, SaveLoadedEventArgs e)
        {
            TalentSelectionMenuPatches.UnlockedMiniRecipe = Game1.player.craftingRecipes.ContainsKey("Mini-Fridge");
            TalentSelectionMenuPatches.UnlockedBigRecipes = Game1.player.craftingRecipes.ContainsKey("Big Fridge 1");

            if (!TalentSelectionMenuPatches.UnlockedMiniRecipe || !TalentSelectionMenuPatches.UnlockedBigRecipes)
            {
                ModEntry.ModHelper.Events.GameLoop.DayStarted += UnlockRecipesDaily;
            }
        }

        private static void DeleteDayEvent(object? sender, ReturnedToTitleEventArgs e)
        {
            ModEntry.ModHelper.Events.GameLoop.DayStarted -= UnlockRecipesDaily;
        }

        private static void UnlockRecipesDaily(object? sender, DayStartedEventArgs e)
        {
            if (!TalentSelectionMenuPatches.UnlockedSkill) return;

            if (!TalentSelectionMenuPatches.UnlockedMiniRecipe
                && Game1.player.HouseUpgradeLevel >= 1)
            {
                Game1.player.craftingRecipes.TryAdd("Mini-Fridge", 0);
                TalentSelectionMenuPatches.UnlockedMiniRecipe = true;

                //ModEntry.ModHelper.GameContent.InvalidateCache("Data\\CraftingRecipes");
            }

            if (!TalentSelectionMenuPatches.UnlockedBigRecipes
                && GameStateQuery.CheckConditions(VanillaLoader.bigFridgeUnlockCondition))
            {
                Game1.player.craftingRecipes.TryAdd("Big Fridge 1", 0);
                Game1.player.craftingRecipes.TryAdd("Big Fridge 2", 0);
                TalentSelectionMenuPatches.UnlockedBigRecipes = true;
                ModEntry.ModHelper.Events.GameLoop.DayStarted -= UnlockRecipesDaily;

                //ModEntry.ModHelper.GameContent.InvalidateCache("Data\\CraftingRecipes");
            }
        }
    }
}
