using Rocket.API;

namespace snowycold.PoliceSearch
{
    public class PoliceSearchConfiguration : IRocketPluginConfiguration
    {
        public static PoliceSearchConfiguration Instance { get; private set; }
        
        public int BatteringRamChance { get; set; }
        public ushort BatteringRamID { get; set; }
        public float VehicleSearchDistance { get; set; }
        public float BarricadeSearchDistance { get; set; }
        public bool AlertPlayerAboutSearches { get; set; }
        public string SearchPermission { get; set; }
        public string BatteringRamPermission { get; set; }
        public string VehicleSearchWebhook { get; set; }
        public string BarricadeSearchWebhook { get; set; }
        public string BatteringRamWebhook { get; set; }

        public void LoadDefaults()
        {
            BatteringRamChance = 25;
            BatteringRamID = 15090;
            VehicleSearchDistance = 10f;
            BarricadeSearchDistance = 10f;
            AlertPlayerAboutSearches =  true;
            SearchPermission = "Police";
            BatteringRamPermission = "BatteringRam";
            VehicleSearchWebhook = "URLHookHere";
            BarricadeSearchWebhook = "URLHookHere";
            BatteringRamWebhook = "URLHookHere";
        }
    }
}
