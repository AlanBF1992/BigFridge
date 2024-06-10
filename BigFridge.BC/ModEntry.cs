using BigFridge.BC.NoIdea;
using Leclair.Stardew.Common.Inventory;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley.Objects;

namespace BigFridge.BC
{
    internal sealed class ModEntry : Mod
    {
        public static IMonitor LogMonitor { get; private set; } = null!;
        public override void Entry(IModHelper helper)
        {
            LogMonitor = Monitor;

            //// RegisterInventoryProvider esta en la API, pero el resto no, así que a la mala.
            //var ChangeChestProvider = new ChestProvider(any: true);
            //var AllowedTypes = ChangeChestProvider.AllowedTypes;
            //AllowedTypes = AllowedTypes.Append((Chest.SpecialChestTypes)9).ToArray();
            //ChangeChestProvider.SetInstanceField("AllowedTypes", AllowedTypes);
            //Leclair.Stardew.BetterCrafting.ModEntry.Instance.RegisterInventoryProvider(typeof(Chest), ChangeChestProvider);

            helper.Events.GameLoop.GameLaunched += OnGameLaunched;
        }

        private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
        {
            IBetterCraftingAPI? api = Helper.ModRegistry.GetApi<IBetterCraftingAPI>("leclair.bettercrafting");
            if (api == null) return;

            api.UnregisterInventoryProvider(typeof(Chest));
            api.RegisterInventoryProvider(typeof(Chest), new NewChestProvider());
        }
    }
}
