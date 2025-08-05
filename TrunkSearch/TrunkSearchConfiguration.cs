using Rocket.API;

namespace snowycold.TrunkSearch
{
    public class TrunkSearchConfiguration : IRocketPluginConfiguration
    {
        public static TrunkSearchConfiguration Instance { get; private set; }
        
        public string PolicePermission { get; set; }
        public string DiscordWebhook { get; set; }

        public void LoadDefaults()
        {
            PolicePermission = "Police";
            DiscordWebhook = "<URLHookHere>";
        }
    }
}