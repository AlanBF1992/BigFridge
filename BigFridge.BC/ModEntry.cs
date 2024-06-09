using Leclair.Stardew.Common.Inventory;
using StardewModdingAPI;
using StardewValley.Objects;

namespace BigFridge.BC
{
    internal sealed class ModEntry : Mod
    {
        public static IMonitor LogMonitor { get; private set; } = null!;
        public override void Entry(IModHelper helper)
        {
            LogMonitor = Monitor;

            // RegisterInventoryProvider esta en la API, pero el resto no, así que a la mala.
            var ChangeChestProvider = new ChestProvider(any: true);
            var AllowedTypes = ChangeChestProvider.AllowedTypes;
            AllowedTypes = AllowedTypes.Append((Chest.SpecialChestTypes)9).ToArray();
            ChangeChestProvider.SetInstanceField("AllowedTypes", AllowedTypes);
            Leclair.Stardew.BetterCrafting.ModEntry.Instance.RegisterInventoryProvider(typeof(Chest), ChangeChestProvider);
        }
    }
}
