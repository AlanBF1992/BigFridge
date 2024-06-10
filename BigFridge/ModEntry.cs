using BigFridge.Compatibility.BetterCrafting;
using BigFridge.Compatibility.GenericModConfigMenu;
using BigFridge.Patches;
using HarmonyLib;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.GameData.BigCraftables;
using StardewValley.GameData.Shops;
using StardewValley.Objects;

namespace BigFridge
{
    internal sealed class ModEntry : Mod
    {
        public static ModConfig Config = new();
        public static IMonitor LogMonitor { get; private set; } = null!;

        public override void Entry(IModHelper helper)
        {
            Config = helper.ReadConfig<ModConfig>();
            LogMonitor = Monitor;

            // Agregar el item
            helper.Events.Content.AssetRequested += OnAssetRequested;

            // Patches
            var harmony = new Harmony(this.ModManifest.UniqueID);

            // Lo asigna como Big Fridge al colocarlo
            harmony.Patch(
                original: AccessTools.Method(typeof(StardewValley.Object), nameof(StardewValley.Object.placementAction)),
                prefix: new HarmonyMethod(typeof(ObjectPatches), nameof(ObjectPatches.placementActionPrefix))
            );

            // Modifica los refrigeradores originales y los nuevos
            harmony.Patch(
                original: AccessTools.Method(typeof(Chest), nameof(Chest.GetActualCapacity)),
                prefix: new HarmonyMethod(typeof(ChestPatches), nameof(ChestPatches.getActualCapacityPrefix))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(Chest), nameof(Chest.ShowMenu)),
                prefix: new HarmonyMethod(typeof(ChestPatches), nameof(ChestPatches.ShowMenuPrefix))
            );

            // Reemplaza un Mini-Fridge por un Big Fridge
            harmony.Patch(
                original: AccessTools.Method(typeof(GameLocation), nameof(GameLocation.checkAction)),
                prefix: new HarmonyMethod(typeof(GameLocationPatches), nameof(GameLocationPatches.checkActionPrefix))
            );

            // Config Menu
            helper.Events.GameLoop.GameLaunched += OnGameLaunched;
        }

        private void OnAssetRequested(object? sender, AssetRequestedEventArgs e)
        {
            // Lo agrega como item
            if (e.Name.IsEquivalentTo("Data/BigCraftables"))
            {
                e.Edit(rawInfo =>
                {
                    var data = rawInfo.AsDictionary<string, BigCraftableData>().Data;

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
                });
            }

            // La imagen del item
            if (e.NameWithoutLocale.IsEquivalentTo("TileSheets/BigFridge"))
                e.LoadFromModFile<Texture2D>("assets/BigFridge.png", AssetLoadPriority.Exclusive);

            // Lo agrega a la tienda de Robin
            if (e.Name.IsEquivalentTo("Data/Shops"))
            {
                e.Edit(rawInfo =>
                {
                    IDictionary<string, ShopData> data = rawInfo.AsDictionary<string, ShopData>().Data;

                    const string House2 = "PLAYER_FARMHOUSE_UPGRADE Current 2";
                    string HearthsRobin = $"PLAYER_FRIENDSHIP_POINTS Current Robin {Config.HearthsWithRobin * 250}";

                    string condition = Config.ItemFridgeWithHearths?
                        $"ANY \"{House2}\" \"{HearthsRobin}\"":
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

            IBetterCraftingAPI? apiBetterCrafting = Helper.ModRegistry.GetApi<IBetterCraftingAPI>("leclair.bettercrafting");
            if (apiBetterCrafting != null)
            {
                apiBetterCrafting.UnregisterInventoryProvider(typeof(Chest));
                apiBetterCrafting.RegisterInventoryProvider(typeof(Chest), new ChestProvider());
            }
        }
    }
}
