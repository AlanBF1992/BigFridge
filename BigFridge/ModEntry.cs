using BigFridge.Compatibility.GenericModConfigMenu;
using BigFridge.Patches;
using HarmonyLib;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley.GameData.BigCraftables;
using StardewValley.GameData.Shops;
using StardewValley.Menus;
using StardewValley.Objects;

namespace BigFridge
{
    internal sealed class ModEntry : Mod
    {
        public static ModConfig Config { get; private set; } = null!;
        public static IMonitor LogMonitor { get; private set; } = null!;
        public static IModHelper ModHelper { get; private set; } = null!;

        public override void Entry(IModHelper helper)
        {
            Config = helper.ReadConfig<ModConfig>();
            LogMonitor = Monitor;
            ModHelper = helper;

            // Agregar el item
            helper.Events.Content.AssetRequested += OnAssetRequested;

            // Patches
            var harmony = new Harmony(this.ModManifest.UniqueID);
            applyPatches(harmony);

            // Config Menu
            helper.Events.GameLoop.GameLaunched += OnGameLaunched;
        }

        private static void applyPatches(Harmony harmony)
        {
            // Fix the change of SpecialType. Will be deleted later.
            harmony.Patch(
                original: AccessTools.Method(typeof(Chest), nameof(Chest.ShowMenu)),
                prefix: new HarmonyMethod(typeof(ChestPatches), nameof(ChestPatches.ShowMenuPrefix))
            );

            // Allows the game to place the Big Fridge with the correct Id
            harmony.Patch(
                original: AccessTools.Method(typeof(StardewValley.Object), nameof(StardewValley.Object.placementAction)),
                prefix: new HarmonyMethod(typeof(ObjectPatches), nameof(ObjectPatches.placementActionPrefix))
            );

            // Put a Big Fridge on top of a Mini Fridge
            harmony.Patch(
                original: AccessTools.Method(typeof(Chest), nameof(Chest.performObjectDropInAction)),
                prefix: new HarmonyMethod(typeof(ChestPatches), nameof(ChestPatches.performObjectDropInActionPrefix))
            );

            // Change fridge reported capacity
            harmony.Patch(
                original: AccessTools.Method(typeof(Chest), nameof(Chest.GetActualCapacity)),
                prefix: new HarmonyMethod(typeof(ChestPatches), nameof(ChestPatches.getActualCapacityPrefix))
            );

            // Allow to show the color picker
            harmony.Patch(
                original: AccessTools.Method(typeof(ItemGrabMenu), nameof(ItemGrabMenu.CanHaveColorPicker)),
                prefix: new HarmonyMethod(typeof(ItemGrabMenuPatches), nameof(ItemGrabMenuPatches.CanHaveColorPickerPrefix))
            );

            // Fix color picker Fridge drawing
            harmony.Patch(
                original: AccessTools.Method(typeof(Chest), nameof(Chest.draw), [typeof(SpriteBatch), typeof(int), typeof(int), typeof(float), typeof(bool)]),
                prefix: new HarmonyMethod(typeof(ChestPatches), nameof(ChestPatches.drawLocalPrefix))
            );

            // Fix color picker changing position with a Big Fridge
            harmony.Patch(
                original: AccessTools.Method(typeof(ItemGrabMenu), nameof(ItemGrabMenu.setSourceItem)),
                postfix: new HarmonyMethod(typeof(ItemGrabMenuPatches), nameof(ItemGrabMenuPatches.setSourceItemPostfix))
            );

            // Actually apply the color
            harmony.Patch(
                original: AccessTools.Method(typeof(Chest), nameof(Chest.draw), [typeof(SpriteBatch), typeof(int), typeof(int), typeof(float)]),
                prefix: new HarmonyMethod(typeof(ChestPatches), nameof(ChestPatches.drawPrefix))
            );
        }

        private void OnAssetRequested(object? sender, AssetRequestedEventArgs e)
        {
            // Lo agrega como item
            if (e.Name.IsEquivalentTo("Data/BigCraftables"))
            {
                e.Edit(rawInfo =>
                {
                    var data = rawInfo.AsDictionary<string, BigCraftableData>().Data;

                    // Add Big Fridge
                    BigCraftableData toAdd = new()
                    {
                        Name = "Big Fridge",
                        DisplayName = Helper.Translation.Get("DisplayName"),
                        Description = Helper.Translation.Get("Description"),
                        Price = Config.Price,
                        Fragility = 0,
                        CanBePlacedOutdoors = true,
                        CanBePlacedIndoors = true,
                        IsLamp = false,
                        Texture = "TileSheets/BigFridge",
                        SpriteIndex = 0,
                        ContextTags = null,
                        CustomFields = null
                    };

                    data.TryAdd("AlanBF.BigFridge", toAdd);

                    // Edit Small Fridge
                    data["216"].Texture = "TileSheets/SmallFridge";
                    data["216"].SpriteIndex = 0;
                });
            }

            // La imagen de los items
            else if (e.NameWithoutLocale.IsEquivalentTo("TileSheets/BigFridge"))
            {
                e.LoadFromModFile<Texture2D>("assets/BigFridge.png", AssetLoadPriority.Exclusive);
            }
            else if (e.NameWithoutLocale.IsEquivalentTo("TileSheets/SmallFridge"))
            {
                e.LoadFromModFile<Texture2D>("assets/SmallFridge.png", AssetLoadPriority.Exclusive);
            }

            // Lo agrega a la tienda de Robin
            else if (e.Name.IsEquivalentTo("Data/Shops"))
            {
                e.Edit(rawInfo =>
                {
                    IDictionary<string, ShopData> data = rawInfo.AsDictionary<string, ShopData>().Data;

                    const string House2 = "PLAYER_FARMHOUSE_UPGRADE Current 2";
                    string HearthsRobin = $"PLAYER_FRIENDSHIP_POINTS Current Robin {Config.HearthsWithRobin * 250}";

                    string condition = Config.ItemFridgeWithHearths ?
                        $"ANY \"{House2}\" \"{HearthsRobin}\"" :
                        House2;

                    ShopItemData toAdd = new()
                    {
                        Id = "AlanBF.BigFridge",
                        ItemId = "AlanBF.BigFridge",
                        Price = Config.Price,
                        Condition = condition
                    };
                    int index = data["Carpenter"].Items.FindIndex(a => a.Id == "(BC)216");
                    data["Carpenter"].Items.Insert(index + 1, toAdd);
                });
            }
        }

        private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
        {
            IGenericModConfigMenuAPI? apiGMCM = Helper.ModRegistry.GetApi<IGenericModConfigMenuAPI>("spacechase0.GenericModConfigMenu");
            if (apiGMCM != null)
            {
                apiGMCM.Register(ModManifest, () => Config = new(), () => Helper.WriteConfig(Config), false);
                apiGMCM.AddBoolOption(
                    mod: ModManifest,
                    getValue: () => Config.HouseFridgeProgressive,
                    setValue: (bool newValue) => Config.HouseFridgeProgressive = newValue,
                    name: () => Helper.Translation.Get("Config_HouseFridgeProgressive_Name"),
                    tooltip: () => Helper.Translation.Get("Config_HouseFridgeProgressive_Tooltip"),
                    fieldId: "AlanBF.BigFridge.HouseFridgeProgressive");
                apiGMCM.AddBoolOption(
                    mod: ModManifest,
                    getValue: () => Config.ItemFridgeWithHearths,
                    setValue: (bool newValue) => Config.ItemFridgeWithHearths = newValue,
                    name: () => Helper.Translation.Get("Config_ItemFridgeWithHearths_Name"),
                    tooltip: () => Helper.Translation.Get("Config_ItemFridgeWithHearths_Tooltip"),
                    fieldId: "AlanBF.BigFridge.ItemFridgeWithHearths");
                apiGMCM.AddNumberOption(
                    mod: ModManifest,
                    getValue: () => Config.HearthsWithRobin,
                    setValue: (int newValue) => Config.HearthsWithRobin = newValue,
                    name: () => Helper.Translation.Get("Config_HearthsWithRobin_Name"),
                    tooltip: () => Helper.Translation.Get("Config_HearthsWithRobin_Tooltip"),
                    min: 0,
                    max: 10,
                    interval: 1,
                    formatValue: (int value) => value.ToString(),
                    fieldId: "AlanBF.BigFridge.HearthsWithRobin");
                apiGMCM.AddNumberOption(
                    mod: ModManifest,
                    getValue: () => Config.Price,
                    setValue: (int newValue) => Config.Price = newValue,
                    name: () => Helper.Translation.Get("Config_Price_Name"),
                    tooltip: () => Helper.Translation.Get("Config_Price_Tooltip")
                    );
            }
        }
    }
}
