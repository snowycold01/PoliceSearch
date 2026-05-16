using System;
using System.Collections.Generic;
using System.Linq;
using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using SDG.Unturned;
using ShimmyMySherbet.DiscordWebhooks;
using ShimmyMySherbet.DiscordWebhooks.Models;
using snowycold.PoliceSearch.Helpers;
using Steamworks;
using UnityEngine;
using Rocket.Core.Logging;
using Logger = Rocket.Core.Logging.Logger;

namespace snowycold.PoliceSearch;

public class SearchCommand : IRocketCommand
{
    public void Execute(IRocketPlayer caller, string[] command)
    {
        UnturnedPlayer player = (UnturnedPlayer)caller;
        Ray ray = new Ray(player.Player.look.aim.position, player.Player.look.aim.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, PoliceSearchPlugin.Instance.Configuration.Instance.VehicleSearchDistance, RayMasks.VEHICLE))
        {
            InteractableVehicle vehicle = hit.transform.GetComponent<InteractableVehicle>();
            if (vehicle != null)
            {
                if (vehicle.isLocked)
                {
                    UnturnedPlayer vehicleOwner = UnturnedPlayer.FromCSteamID(vehicle.lockedOwner);

                    ThreadHelper.RunAsynchronously(async () =>
                    {
                        var message = new WebhookMessage()
                            .PassEmbed()
                            .WithTitle("Vehicle Searched!")
                            .WithColor(EmbedColor.White)
                            .WithField("", $"{player.DisplayName} ({player.CSteamID.m_SteamID.ToString()}) searched {vehicleOwner.DisplayName} ({vehicleOwner.CSteamID.m_SteamID})'s {vehicle.asset.name}!")
                            .WithTimestamp(DateTime.Now);

                        var send = message.Finalize();
                        await DiscordWebhookService.PostMessageAsync(PoliceSearchPlugin.Instance.Configuration.Instance.VehicleSearchWebhook, send);
                    });
                }

                else
                {
                    
                    ThreadHelper.RunAsynchronously(async () =>
                    {
                        var message = new WebhookMessage()
                            .PassEmbed()
                            .WithTitle("Vehicle Searched!")
                            .WithColor(EmbedColor.White)
                            .WithField("", $"{player.DisplayName} ({player.CSteamID.m_SteamID.ToString()}) searched an unlocked {vehicle.asset.name}!")
                            .WithTimestamp(DateTime.Now);

                        var send = message.Finalize();
                        await DiscordWebhookService.PostMessageAsync(PoliceSearchPlugin.Instance.Configuration.Instance.VehicleSearchWebhook, send);
                    });
                }
                
                GameObject tempObject = new GameObject("TrunkStorage");
                InteractableStorage trunkStorage = tempObject.AddComponent<InteractableStorage>();

                var type = typeof(InteractableStorage);

                var itemsField = type.GetField("_items", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                var isOpenField = type.GetField("isOpen", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                var canPlayersOpenField = type.GetField("canPlayersOpen", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

                itemsField?.SetValue(trunkStorage, vehicle.trunkItems);
                isOpenField?.SetValue(trunkStorage, true);
                canPlayersOpenField?.SetValue(trunkStorage, true);

                player.Player.inventory.openStorage(trunkStorage);
                
                List<ItemJar> itemsBefore = vehicle.trunkItems.items
                    .Select(jar => new ItemJar(jar.x, jar.y, jar.rot, new Item(jar.item.id, jar.item.amount, jar.item.quality, jar.item.state)))
                    .ToList();

                PoliceSearchPlugin.Instance.StartCoroutine(OnTrunkClose.WaitForTrunkClose(player, vehicle, tempObject, itemsBefore));
            }
        }
        
        else if (Physics.Raycast(ray, out hit, PoliceSearchPlugin.Instance.Configuration.Instance.BarricadeSearchDistance,
                     RayMasks.BARRICADE_INTERACT))
        {
            InteractableStorage interactableStorage = hit.transform.GetComponent<InteractableStorage>();
            if (interactableStorage == null) interactableStorage = hit.transform.GetComponent<InteractableStorage>();
            if (interactableStorage != null)
            {
                BarricadeDrop drop = BarricadeManager.FindBarricadeByRootTransform(hit.transform.root);
                if (drop == null) return;

                if (drop.asset is ItemStorageAsset)
                {
                    BarricadeData data = drop.GetServersideData();
                    UnturnedPlayer owner = UnturnedPlayer.FromCSteamID(new CSteamID(data.owner));

                    if (data.owner != 0)
                    {
                        ThreadHelper.RunAsynchronously(async () =>
                        {
                            var message = new WebhookMessage()
                                .PassEmbed()
                                .WithTitle("Storage Searched!")
                                .WithColor(EmbedColor.White)
                                .WithField("", $"{player.DisplayName} ({player.CSteamID.m_SteamID}) searched {owner.DisplayName} ({data.owner})'s {drop.asset.itemName}!")
                                .WithTimestamp(DateTime.Now);

                            var send = message.Finalize();
                            await DiscordWebhookService.PostMessageAsync(PoliceSearchPlugin.Instance.Configuration.Instance.BarricadeSearchWebhook, send);
                        });
                    }

                    else
                    {
                        ThreadHelper.RunAsynchronously(async () =>
                        {
                            var message = new WebhookMessage()
                                .PassEmbed()
                                .WithTitle("Storage Searched!")
                                .WithColor(EmbedColor.White)
                                .WithField("", $"{player.DisplayName} ({player.CSteamID.m_SteamID}) searched an unowned {drop.asset.itemName}!")
                                .WithTimestamp(DateTime.Now);

                            var send = message.Finalize();
                            await DiscordWebhookService.PostMessageAsync(PoliceSearchPlugin.Instance.Configuration.Instance.BarricadeSearchWebhook, send);
                        });
                    }
                    
                    player.Player.inventory.openStorage(interactableStorage);
                    List<ItemJar> beforeItems = interactableStorage.items.items
                        .Select(jar => new ItemJar(jar.x, jar.y, jar.rot, new Item(jar.item.id, jar.item.amount, jar.item.quality, jar.item.state)))
                        .ToList();
                    PoliceSearchPlugin.Instance.StartCoroutine(OnBarricadeClose.WaitForBarricadeClose(player, interactableStorage, beforeItems, drop));
                }
            }
        }
        else
        {
            UnturnedChat.Say("Nothing has been searched");
        }
            
    }

    public AllowedCaller AllowedCaller => AllowedCaller.Player;
    public string Name => "search";
    public string Help => "Use this command to search a player or vehicle";
    public string Syntax => "";
    public List<string> Aliases => new List<string>();
    public List<string> Permissions => [PoliceSearchPlugin.Instance.Configuration.Instance.SearchPermission];
}
