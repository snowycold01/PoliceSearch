using System;
using System.Collections.Generic;
using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using SDG.Unturned;
using ShimmyMySherbet.DiscordWebhooks;
using ShimmyMySherbet.DiscordWebhooks.Models;
using snowycold.TrunkSearch.Helpers;
using Steamworks;
using UnityEngine;

namespace snowycold.TrunkSearch;

public class SearchCommand : IRocketCommand
{
    public void Execute(IRocketPlayer caller, string[] command)
    {
        UnturnedPlayer player = (UnturnedPlayer)caller;
        Ray ray = new Ray(player.Player.look.aim.position, player.Player.look.aim.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 10f, RayMasks.VEHICLE))
        {
            InteractableVehicle vehicle = hit.transform.GetComponent<InteractableVehicle>();
            if (vehicle != null)
            {
                if (vehicle.isLocked)
                {
                    UnturnedPlayer vehicleOwner = UnturnedPlayer.FromCSteamID(vehicle.lockedOwner);
                    UnturnedChat.Say($"You are searching {vehicleOwner.DisplayName}'s {vehicle.asset.name}.");

                    ThreadHelper.RunAsynchronously(async () =>
                    {
                        var message = new WebhookMessage()
                            .PassEmbed()
                            .WithTitle("Vehicle Searched!")
                            .WithColor(EmbedColor.White)
                            .WithField("", $"{player.DisplayName} ({player.CSteamID.m_SteamID.ToString()}) searched {vehicleOwner.DisplayName} ({vehicleOwner.CSteamID.m_SteamID})'s {vehicle.asset.name}!")
                            .WithTimestamp(DateTime.UtcNow);

                        var send = message.Finalize();
                        await DiscordWebhookService.PostMessageAsync(TrunkSearchPlugin.Instance.Configuration.Instance.DiscordWebhook, send);
                    });
                }

                else
                {
                    UnturnedChat.Say($"You are searching an unlocked {vehicle.asset.name}.");
                    
                    ThreadHelper.RunAsynchronously(async () =>
                    {
                        var message = new WebhookMessage()
                            .PassEmbed()
                            .WithTitle("Vehicle Searched!")
                            .WithColor(EmbedColor.White)
                            .WithField("", $"{player.DisplayName} ({player.CSteamID.m_SteamID.ToString()}) searched an unlocked {vehicle.asset.name}!")
                            .WithTimestamp(DateTime.UtcNow);

                        var send = message.Finalize();
                        await DiscordWebhookService.PostMessageAsync(TrunkSearchPlugin.Instance.Configuration.Instance.DiscordWebhook, send);
                    });
                }
                List<ItemJar> trunkItems = vehicle.trunkItems.items;
                
                player.Player.inventory.updateItems(7, vehicle.trunkItems);
                player.Player.inventory.sendStorage();
                
            }
            else
            {
                UnturnedChat.Say("You are looking at something but not a vehicle");
            }
        }
        else
        {
            UnturnedChat.Say("You're not looking at any vehicle.");
        }
    }

    public AllowedCaller AllowedCaller => AllowedCaller.Player;
    public string Name => "search";
    public string Help => "Use this command to search a player or vehicle";
    public string Syntax => "";
    public List<string> Aliases => new List<string>();
    public List<string> Permissions => [TrunkSearchPlugin.Instance.Configuration.Instance.PolicePermission];
}