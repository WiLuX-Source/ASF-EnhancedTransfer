using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using ArchiSteamFarm.Core;
using ArchiSteamFarm.Steam;
using ArchiSteamFarm.Steam.Data;

namespace EnhancedTransfer;

internal static class Commands {
	internal static readonly char[] Separator = new[] { ',' };

	internal static async Task<string?> TransferItemsNormalWithBot(Bot bot, EAccess access, string mode, string botNameTo) {
		if (string.IsNullOrEmpty(botNameTo) || string.IsNullOrEmpty(mode)) {
			ASF.ArchiLogger.LogNullError(null, nameof(mode) + " || " + nameof(botNameTo));

			return null;
		}

		if (access < EAccess.Master) {
			return null;
		}

		if (!bot.IsConnectedAndLoggedOn) {
			return bot.Commands.FormatBotResponse(ArchiSteamFarm.Localization.Strings.BotNotConnected);
		}

		Bot? targetBot = Bot.GetBot(botNameTo);

		if (targetBot == null) {
			return access >= EAccess.Owner ? bot.Commands.FormatBotResponse(string.Format(CultureInfo.CurrentCulture, ArchiSteamFarm.Localization.Strings.BotNotFound, botNameTo)) : null;
		}

		if (!targetBot.IsConnectedAndLoggedOn) {
			return bot.Commands.FormatBotResponse(ArchiSteamFarm.Localization.Strings.BotNotConnected);
		}

		if (targetBot.SteamID == bot.SteamID) {
			return bot.Commands.FormatBotResponse(ArchiSteamFarm.Localization.Strings.BotSendingTradeToYourself);
		}

		string[] modes = mode.Split(Separator, StringSplitOptions.RemoveEmptyEntries);

		if (modes.Length == 0) {
			return bot.Commands.FormatBotResponse(string.Format(CultureInfo.CurrentCulture, ArchiSteamFarm.Localization.Strings.ErrorIsEmpty, nameof(modes)));
		}

		HashSet<Asset.EType> transferTypes = new();

		foreach (string singleMode in modes) {
			switch (singleMode.ToUpperInvariant()) {
				case "A":
				case "ALL":
					foreach (Asset.EType type in (Asset.EType[]) Enum.GetValues(typeof(Asset.EType))) {
						transferTypes.Add(type);
					}

					break;
				case "BG":
				case "BACKGROUND":
					transferTypes.Add(Asset.EType.ProfileBackground);

					break;
				case "BO":
				case "BOOSTER":
					transferTypes.Add(Asset.EType.BoosterPack);

					break;
				case "C":
				case "CARD":
					transferTypes.Add(Asset.EType.TradingCard);

					break;
				case "E":
				case "EMOTICON":
					transferTypes.Add(Asset.EType.Emoticon);

					break;
				case "F":
				case "FOIL":
					transferTypes.Add(Asset.EType.FoilTradingCard);

					break;
				case "G":
				case "GEMS":
					transferTypes.Add(Asset.EType.SteamGems);

					break;
				case "U":
				case "UNKNOWN":
					transferTypes.Add(Asset.EType.Unknown);

					break;
				default:
					return bot.Commands.FormatBotResponse(string.Format(CultureInfo.CurrentCulture, ArchiSteamFarm.Localization.Strings.ErrorIsInvalid, mode));
			}
		}

		(bool success, string message) = await bot.Actions.SendInventory(targetSteamID: targetBot.SteamID, filterFunction: item => transferTypes.Contains(item.Type)).ConfigureAwait(false);

		return bot.Commands.FormatBotResponse(success ? message : string.Format(CultureInfo.CurrentCulture, ArchiSteamFarm.Localization.Strings.WarningFailedWithError, message));
	}

	internal static async Task<string?> TransferItemsNormal(EAccess access, string botNames, string mode, string botNameTo) {
		if (string.IsNullOrEmpty(botNames) || string.IsNullOrEmpty(mode) || string.IsNullOrEmpty(botNameTo)) {
			ASF.ArchiLogger.LogNullError(null, nameof(botNames) + " || " + nameof(mode) + " || " + nameof(botNameTo));

			return null;
		}

		if (access < EAccess.Master) {
			return null;
		}

		HashSet<Bot>? bots = Bot.GetBots(botNames);

		if ((bots == null) || (bots.Count == 0)) {
			return access >= EAccess.Owner ? FormatStaticResponse(string.Format(CultureInfo.CurrentCulture, ArchiSteamFarm.Localization.Strings.BotNotFound, botNames)) : null;
		}

		IEnumerable<Task<string?>> tasks = bots.Select(bot => TransferItemsNormalWithBot(bot, access, mode, botNameTo));
		ICollection<string?> results = await Task.WhenAll(tasks).ConfigureAwait(false);

		List<string?> responses = new(results.Where(static result => !string.IsNullOrEmpty(result)));

		return responses.Count > 0 ? string.Join("", responses) : null;
	}

	internal static async Task<string?> TransferItemsAdvancedWithBot(Bot bot, EAccess access, string mode, string targetName) {
		if (string.IsNullOrEmpty(targetName) || string.IsNullOrEmpty(mode)) {
			ASF.ArchiLogger.LogNullError(null, nameof(mode) + " || " + nameof(targetName));

			return null;
		}

		if (access < EAccess.Master) {
			return null;
		}

		if (!bot.IsConnectedAndLoggedOn) {
			return bot.Commands.FormatBotResponse(ArchiSteamFarm.Localization.Strings.BotNotConnected);
		}

		Bot? targetBot = Bot.GetBot(targetName);

		if (targetBot == null) {
			return access >= EAccess.Owner ? bot.Commands.FormatBotResponse(string.Format(CultureInfo.CurrentCulture, ArchiSteamFarm.Localization.Strings.BotNotFound, targetBot)) : null;
		}

		if (!targetBot.IsConnectedAndLoggedOn) {
			return bot.Commands.FormatBotResponse(ArchiSteamFarm.Localization.Strings.BotNotConnected);
		}

		if (targetBot.SteamID == bot.SteamID) {
			return bot.Commands.FormatBotResponse(ArchiSteamFarm.Localization.Strings.BotSendingTradeToYourself);
		}

		if (string.IsNullOrEmpty(mode)) {
			return bot.Commands.FormatBotResponse(string.Format(CultureInfo.CurrentCulture, ArchiSteamFarm.Localization.Strings.ErrorIsEmpty, nameof(mode)));
		}

		string[] modes = mode.Split(Separator, StringSplitOptions.RemoveEmptyEntries);

		if (modes.Length == 0) {
			return bot.Commands.FormatBotResponse(string.Format(CultureInfo.CurrentCulture, ArchiSteamFarm.Localization.Strings.ErrorIsEmpty, nameof(modes)));
		}

		HashSet<Asset> inventory = new();

		foreach (string singleMode in modes) {
			switch (singleMode.ToUpperInvariant()) {
				case "CS":
				case "CASE":

					await foreach (Asset asset in bot.ArchiWebHandler.GetInventoryAsync(appID: 730, contextID: 2).ConfigureAwait(false)) {
						if (asset.Tradable && asset.Tags != null && asset.Tags.Any(static tag => tag is { Identifier: "Type", Value: "CSGO_Type_WeaponCase" })) {
							inventory.Add(asset);
						}
					}

					break;
				case "TF2":
				case "KEY":

					await foreach (Asset asset in bot.ArchiWebHandler.GetInventoryAsync(appID: 440, contextID: 2).ConfigureAwait(false)) {
						if (asset is { Tradable: true, ClassID: 101785959 }) {
							inventory.Add(asset);
						}
					}

					break;
				default:
					return bot.Commands.FormatBotResponse(string.Format(CultureInfo.CurrentCulture, ArchiSteamFarm.Localization.Strings.ErrorIsInvalid, mode));
			}
		}

		(bool success, string message) = await bot.Actions.SendInventory(inventory, targetBot.SteamID).ConfigureAwait(false);

		return bot.Commands.FormatBotResponse(success ? message : string.Format(CultureInfo.CurrentCulture, ArchiSteamFarm.Localization.Strings.WarningFailedWithError, message));
	}

	internal static async Task<string?> TransferItemsAdvanced(EAccess access, string botNames, string mode, string targetName) {
		if (string.IsNullOrEmpty(botNames) || string.IsNullOrEmpty(mode) || string.IsNullOrEmpty(targetName)) {
			ASF.ArchiLogger.LogNullError(null, nameof(botNames) + " || " + nameof(mode) + " || " + nameof(targetName));

			return null;
		}

		if (access < EAccess.Master) {
			return null;
		}

		HashSet<Bot>? bots = Bot.GetBots(botNames);

		if ((bots == null) || (bots.Count == 0)) {
			return access >= EAccess.Owner ? FormatStaticResponse(string.Format(CultureInfo.CurrentCulture, ArchiSteamFarm.Localization.Strings.BotNotFound, botNames)) : null;
		}

		IEnumerable<Task<string?>> tasks = bots.Select(bot => TransferItemsAdvancedWithBot(bot, access, mode, targetName));
		ICollection<string?> results = await Task.WhenAll(tasks).ConfigureAwait(false);

		List<string?> responses = new(results.Where(static result => !string.IsNullOrEmpty(result)));

		return responses.Count > 0 ? string.Join("", responses) : null;
	}

	private static string FormatStaticResponse(string response) => ArchiSteamFarm.Steam.Interaction.Commands.FormatStaticResponse(response);
}
