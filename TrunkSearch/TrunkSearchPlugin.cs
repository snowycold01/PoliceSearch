using Rocket.Core.Logging;
using Rocket.Core.Plugins;

namespace snowycold.TrunkSearch
{
    public class TrunkSearchPlugin : RocketPlugin<TrunkSearchConfiguration>
    {
        protected override void Load()
        {
            Logger.Log($"{Name} {Assembly.GetName().Version.ToString(3)} has been loaded!");
        }

        protected override void Unload()
        {
            Logger.Log($"{Name} has been unloaded!");
        }
    }
}