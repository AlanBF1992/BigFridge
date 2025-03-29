namespace BigFridge
{
    public sealed class ModConfig
    {
        public bool HouseFridgeProgressive { get; set; } = false;
        public bool ItemFridgeWithHearths { get; set; } = true;
        public int HearthsWithRobin { get; set; } = 5;
        public int Price { get; set; } = 10000;
    }
}