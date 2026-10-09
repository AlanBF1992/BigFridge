namespace BigFridge
{
    public sealed class ModConfig
    {
        public bool HouseFridgeProgressive { get; set; } = false;
        public bool ItemFridgeWithHearths { get; set; } = true;

        public int HearthsWithRobin
        {
            get;
            set => field = Math.Clamp(value, 0, 10);
        } = 5;

        public int Price
        {
            get;
            set => field = Math.Max(0, value);
        } = 10000;

        public bool ReskinMiniFridge { get; set; } = true;

        public string FridgeFolderAssets
        {
            get;
            set => field = string.IsNullOrWhiteSpace(value) ? "Vanilla" : value;
        } = "Vanilla";
    }
}
