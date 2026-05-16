using Rocket.API;

namespace snowycold.PoliceSearch
{
    public class PoliceSearchConfiguration : IRocketPluginConfiguration
    {
        public static PoliceSearchConfiguration Instance { get; private set; }
        
        public float VehicleSearchDistance { get; set; }
        public float BarricadeSearchDistance { get; set; }
        public bool AlertPlayerAboutSearches { get; set; }
        public string PolicePermission { get; set; }
        public string VehicleSearchWebhook { get; set; }
        public string BarricadeSearchWebhook { get; set; }

        public void LoadDefaults()
        {
            VehicleSearchDistance = 10f;
            BarricadeSearchDistance = 10f;
            AlertPlayerAboutSearches =  true;
            PolicePermission = "Police";
            VehicleSearchWebhook = "URLHookHere";
            BarricadeSearchWebhook = "URLHookHere";
        }
    }
}
