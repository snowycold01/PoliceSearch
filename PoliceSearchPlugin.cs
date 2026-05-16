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
            Logger.Log("\n-=-=-=-Faction Manager v1.0.1-=-=-=-\n-=-By: snowycold-=-=-\n-=-=-=-Has Been Loaded-=-=-=-");
        }

        protected override void Unload()
        {
            Logger.Log("\n-=-=-=-Faction Manager v1.0.0-=-=-=-\n-=-By: snowycold-=-=-\n-=-=-=-Has Been Unloaded-=-=-=-");
        }
    }
}
