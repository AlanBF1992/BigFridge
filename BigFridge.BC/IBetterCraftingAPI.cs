using BigFridge.BC.NoIdea;
using BigFridge.Compatibility.BetterCrafting;

namespace BigFridge.BC
{
    public interface IBetterCraftingAPI
    {
        /// <summary>
        /// Register an inventory provider with Better Crafting. Inventory
        /// providers are used for interfacing with chests and other objects
        /// in the world that contain items.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="provider"></param>
        void RegisterInventoryProvider(Type type, IInventoryProvider provider);

        /// <summary>
        /// Unregister an inventory provider.
        /// </summary>
        /// <param name="type"></param>
        void UnregisterInventoryProvider(Type type);
    }
}
