using Rocket.Core.Logging;
using Rocket.Core.Plugins;

namespace snowycold.TrunkSearch
{
    public class TrunkSearchPlugin : RocketPlugin<TrunkSearchConfiguration>
    {
        public static TrunkSearchPlugin Instance { private set; get; }

        protected override void Load()
        {
            Instance = this;
            Logger.Log("TrunkSearch Plugin loaded");
        }

        protected override void Unload()
        {
            Logger.Log("TrunkSearch Plugin unloaded");
        }
    }
}