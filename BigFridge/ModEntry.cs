using BigFridge.Compatibility.VPP;
using HarmonyLib;
using StardewModdingAPI;

namespace BigFridge
{
    internal sealed class ModEntry : Mod
    {
        public static ModConfig Config { get; internal set; } = null!;
        public static IMonitor LogMonitor { get; internal set; } = null!;
        public static IModHelper ModHelper { get; internal set; } = null!;
        new public static IManifest ModManifest { get; internal set; } = null!;
        public static string[] AvailableFolders { get; internal set; } = null!;

        public override void Entry(IModHelper helper)
        {
            Config = helper.ReadConfig<ModConfig>();
            LogMonitor = Monitor;
            ModHelper = helper;
            ModManifest = base.ModManifest;

            Harmony harmony = new(ModManifest.UniqueID);

            AvailableFolders = [.. new DirectoryInfo(Path.Combine(ModHelper.DirectoryPath, "assets")).GetDirectories().Select(x => x.Name)];

            // Patches
            VanillaLoader.Loader(ModHelper, harmony);
            LogMonitor.Log("Base Patches Loaded", LogLevel.Info);

            if (ModHelper.ModRegistry.IsLoaded("KediDili.VanillaPlusProfessions"))
            {
                VPPLoader.Loader(ModHelper, harmony);
                LogMonitor.Log("VPP Patches Loaded", LogLevel.Info);
            }
        }
    }
}
