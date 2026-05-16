using Rocket.Core.Plugins;
using Rocket.Unturned.Player;
using SDG.Unturned;
using Steamworks;
using UnityEngine;
using Logger = Rocket.Core.Logging.Logger;

namespace snowycold.PoliceSearch
{
    public class PoliceSearchPlugin : RocketPlugin<PoliceSearchConfiguration>
    {
        public static PoliceSearchPlugin Instance { private set; get; }

        protected override void Load()
        {
            Instance = this;
            Logger.Log("\n-=-=-=-Faction Manager v1.0.1-=-=-=-\n-=-By: snowycold-=-=-\n-=-=-=-Has Been Loaded-=-=-=-");
            BarricadeManager.onDamageBarricadeRequested += new DamageBarricadeRequestHandler(this.onDamageBarricadeRequested);
        }

        protected override void Unload()
        {
            Logger.Log("\n-=-=-=-Faction Manager v1.0.0-=-=-=-\n-=-By: snowycold-=-=-\n-=-=-=-Has Been Unloaded-=-=-=-");
            BarricadeManager.onDamageBarricadeRequested -= new DamageBarricadeRequestHandler(this.onDamageBarricadeRequested);
        }
        
        public void onDamageBarricadeRequested(CSteamID instigatorSteamID, Transform structureTransform, ref ushort pendingTotalDamage, ref bool shouldAllow, EDamageOrigin damageOrigin)
        {
            shouldAllow = false;
            UnturnedPlayer player = UnturnedPlayer.FromCSteamID(instigatorSteamID);
            ushort itemHeld = player.Player.equipment.itemID;
            Interactable component = structureTransform.GetComponent<Interactable>();
            if (component == null || structureTransform.GetComponent<InteractableDoor>() == null || itemHeld != Instance.Configuration.Instance.BatteringRamID || !(component is InteractableDoor))
            {
                return;
            }

            int result = Random.Range(1, 101);
            print(result);
            if (result <= Instance.Configuration.Instance.BatteringRamChance)
            {
                ToggleDoor(player, structureTransform);
            }
        }

        public void ToggleDoor(UnturnedPlayer player, Transform structureTransform)
        {
            InteractableDoor component = structureTransform.GetComponent<InteractableDoor>();
            if (component == null)
            {
                Logger.Log("anotha one");
                return;
            }

            BarricadeManager.ServerSetDoorOpen(component, !component.isOpen);
        }
    }
}
