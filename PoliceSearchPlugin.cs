using Rocket.Core.Logging;
using Rocket.Core.Plugins;

namespace snowycold.PoliceSearch
{
    public class PoliceSearchPlugin : RocketPlugin<PoliceSearchConfiguration>
    {
        public static PoliceSearchPlugin Instance { private set; get; }

        protected override void Load()
        {
            Instance = this;
            Logger.Log("PoliceSearch Plugin loaded");
        }

        protected override void Unload()
        {
            Logger.Log("PoliceSearch Plugin unloaded");
        }
    }
}
