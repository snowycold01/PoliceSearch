using System;
using System.Linq;
using Rocket.API;
using Rocket.Core.Plugins;
using Rocket.Unturned.Player;
using SDG.Unturned;
using ShimmyMySherbet.DiscordWebhooks.Embeded;
using snowycold.PoliceSearch.Helpers;
using Steamworks;
using UnityEngine;
using Logger = Rocket.Core.Logging.Logger;
using Random = UnityEngine.Random;

namespace snowycold.PoliceSearch
{
    public class PoliceSearchPlugin : RocketPlugin<PoliceSearchConfiguration>
    {
        public static PoliceSearchPlugin Instance { private set; get; }

        protected override void Load()
        {
            Instance = this;
            Logger.Log("\n-=-=-=-Police Search v1.0.2-=-=-=-\n-=-By: snowycold-=-=-\n-=-=-=-Has Been Loaded-=-=-=-");
            BarricadeManager.onDamageBarricadeRequested += new DamageBarricadeRequestHandler(this.onDamageBarricadeRequested);
        }

        protected override void Unload()
        {
            Logger.Log("\n-=-=-=-Police Search v1.0.2-=-=-=-\n-=-By: snowycold-=-=-\n-=-=-=-Has Been Unloaded-=-=-=-");
            BarricadeManager.onDamageBarricadeRequested -= new DamageBarricadeRequestHandler(this.onDamageBarricadeRequested);
        }
        
        public void onDamageBarricadeRequested(CSteamID instigatorSteamID, Transform structureTransform, ref ushort pendingTotalDamage, ref bool shouldAllow, EDamageOrigin damageOrigin)
        {
            UnturnedPlayer player = UnturnedPlayer.FromCSteamID(instigatorSteamID);

            if (!player.HasPermission(Instance.Configuration.Instance.BatteringRamPermission)) return;
            ushort itemHeld = player.Player.equipment.itemID;
            Interactable component = structureTransform.GetComponent<Interactable>();
            if (component == null || structureTransform.GetComponent<InteractableDoor>() == null || itemHeld != Instance.Configuration.Instance.BatteringRamID || !(component is InteractableDoor))
            {
                return;
            }

            int result = Random.Range(1, 101);
            if (result <= Instance.Configuration.Instance.BatteringRamChance)
            {
                string locationName = GetNearestLocationName(structureTransform);
                ThreadHelper.RunAsynchronously(async () =>
                {
                    var message = new WebhookMessage()
                        .PassEmbed()
                        .WithTitle("Door Raided")
                        .WithColor(EmbedColor.White)
                        .WithField("", $"{player.DisplayName} ({player.CSteamID.m_SteamID.ToString()}) raided {structureTransform.name} near {locationName}!")
                        .WithTimestamp(DateTime.Now);

                    var send = message.Finalize();
                    await DiscordWebhookService.PostMessageAsync(Instance.Configuration.Instance.VehicleSearchWebhook, send);
                });
                ToggleDoor(player, structureTransform);
            }
        }

        public void ToggleDoor(UnturnedPlayer player, Transform structureTransform)
        {
            InteractableDoor component = structureTransform.GetComponent<InteractableDoor>();
            if (component == null)
            {
                return;
            }

            BarricadeManager.ServerSetDoorOpen(component, !component.isOpen);
        }
        
        public static string GetNearestLocationName(Transform transform)
        {
            LocationNode nearest = null;
            float nearestDistance = float.MaxValue;

            foreach (LocationNode node in LevelNodes.nodes.OfType<LocationNode>())
            {
                float distance = Vector3.Distance(transform.position, node.point);
                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearest = node;
                }
            }

            return nearest != null ? nearest.name : "Unknown";
        }
    }
}
