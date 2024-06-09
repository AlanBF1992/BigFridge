namespace BigFridge
{
    public sealed class ModConfig
    {
        public bool HouseFridgeProgressive { get; set; }
        public bool ItemFridgeWithHearths { get; set; }
        public int HearthsWithRobin { get; set; }
        public int Price { get; set; }

        public ModConfig()
        {
            this.HouseFridgeProgressive = false;
            this.ItemFridgeWithHearths = true;
            this.HearthsWithRobin = 5;
            this.Price = 10000;
        }
    }
}