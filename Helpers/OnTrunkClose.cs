using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using SDG.Unturned;
using ShimmyMySherbet.DiscordWebhooks.Embeded;
using UnityEngine;

namespace snowycold.PoliceSearch.Helpers;

public class OnTrunkClose
{
    public static IEnumerator WaitForTrunkClose(UnturnedPlayer player, InteractableVehicle vehicle, GameObject tempObject, List<ItemJar> itemsBefore)
    {
        yield return new WaitUntil(() => player.Player.inventory.isStoring);
        yield return new WaitUntil(() => !player.Player.inventory.isStoring);

        GameObject.Destroy(tempObject);

        List<ItemJar> itemsAfter = vehicle.trunkItems.items;

        // Find items that were taken (in before but not in after)
        List<ItemJar> takenItems = itemsBefore
            .Where(before => !itemsAfter.Any(after => after.item.id == before.item.id && after.x == before.x && after.y == before.y))
            .ToList();

        // Find items that were added (in after but not in before)
        List<ItemJar> addedItems = itemsAfter
            .Where(after => !itemsBefore.Any(before => before.item.id == after.item.id && before.x == after.x && before.y == after.y))
            .ToList();

        if (takenItems.Count == 0 && addedItems.Count == 0)
        {
            yield break;
        }

        List<String> itemAddedNames = new List<String>();
        List<String> itemTakenNames = new List<String>();
        
        foreach (ItemJar taken in takenItems)
        {
            ItemAsset asset = (ItemAsset)Assets.find(EAssetType.ITEM, taken.item.id);
            string itemName = asset is not null ? asset.itemName : $"Unknown ({taken.item.id})";
            itemTakenNames.Add(itemName);
            if (PoliceSearchPlugin.Instance.Configuration.Instance.AlertPlayerAboutSearches) UnturnedChat.Say(vehicle.lockedOwner, $"{player.DisplayName} took {itemName} from {vehicle.asset.name}");
        }
        
        foreach (ItemJar added in addedItems)
        {
            ItemAsset asset = (ItemAsset)Assets.find(EAssetType.ITEM, added.item.id);
            string itemName = asset is not null ? asset.itemName : $"Unknown ({added.item.id})";
            itemAddedNames.Add(itemName);
            if (PoliceSearchPlugin.Instance.Configuration.Instance.AlertPlayerAboutSearches) UnturnedChat.Say(vehicle.lockedOwner, $"{player.DisplayName} added {itemName} from {vehicle.asset.name}");
        }

        List<String> itemTakenNamesSorted = new List<string>();
        List<String> itemGivenNamesSorted = new List<string>();

        foreach (string itemName in itemTakenNames)
        {
            if (itemTakenNamesSorted.Any(name => name == itemName || name.StartsWith(itemName + " ")))
                continue;
            int total = itemTakenNames.Count(name => name == itemName);
            itemTakenNamesSorted.Add($"{itemName} {total}x");
        }
        
        foreach (string itemName in itemAddedNames)
        {
            if (itemGivenNamesSorted.Any(name => name == itemName || name.StartsWith(itemName + " ")))
                continue;
            int total = itemAddedNames.Count(name => name == itemName);
            itemGivenNamesSorted.Add($"{itemName} {total}x");
        }

        UnturnedPlayer vehicleOwner = UnturnedPlayer.FromCSteamID(vehicle.lockedOwner);
        
        if (itemAddedNames.Count > 0)
        {
            string result = string.Join("\n", itemGivenNamesSorted);
            ThreadHelper.RunAsynchronously(async () =>
            {
                var message = new WebhookMessage()
                    .PassEmbed()
                    .WithTitle("Item Added")
                    .WithDescription($"An item(s) have been added to {vehicleOwner.DisplayName}'s ({vehicleOwner.CSteamID}) {vehicle.name}!")
                    .WithColor(EmbedColor.White)
                    .WithField("Items:", result)
                    .WithField("Officer:", $"{player.DisplayName} ({player.CSteamID})")
                    .WithTimestamp(DateTime.Now);

                var send = message.Finalize();
                await DiscordWebhookService.PostMessageAsync(PoliceSearchPlugin.Instance.Configuration.Instance.VehicleSearchWebhook, send);
            });
        }

        if (itemTakenNamesSorted.Count > 0)
        {
            string result = string.Join("\n", itemTakenNamesSorted);
            ThreadHelper.RunAsynchronously(async () =>
            {
                var message = new WebhookMessage()
                    .PassEmbed()
                    .WithTitle("Item Taken")
                    .WithDescription($"An item(s) have been taken from {vehicleOwner.DisplayName}'s ({vehicleOwner.CSteamID}) {vehicle.name}!")
                    .WithColor(EmbedColor.White)
                    .WithField("Items:", result)
                    .WithField("Officer:", $"{player.DisplayName} ({player.CSteamID})")
                    .WithTimestamp(DateTime.Now);

                var send = message.Finalize();
                await DiscordWebhookService.PostMessageAsync(PoliceSearchPlugin.Instance.Configuration.Instance.VehicleSearchWebhook, send);
            });   
        }
    }
}
