using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ArchiSteamFarm.Core;
using ArchiSteamFarm.Plugins.Interfaces;
using ArchiSteamFarm.Steam;
using ArchiSteamFarm.Steam.Data;
using JetBrains.Annotations;

namespace EnhancedTransfer;

#pragma warning disable CA1812 // ASF uses this class during runtime
[UsedImplicitly]
internal sealed class EnhancedTransfer : IBotCommand2, IBotTradeOffer {
	public string Name => nameof(EnhancedTransfer);
	public Version Version => typeof(EnhancedTransfer).Assembly.GetName().Version ?? throw new InvalidOperationException(nameof(Version));

	public Task OnLoaded() {
		ASF.ArchiLogger.LogGenericInfo($"{Name} by Wilux Loaded!");

		return Task.CompletedTask;
	}

	public async Task<string?> OnBotCommand(Bot bot, EAccess access, string message, string[] args, ulong steamID = 0) {
		if (string.IsNullOrEmpty(message)) {
			return null;
		}

		return args[0].ToUpperInvariant() switch {
			"ETRANSFER" when args.Length > 3 => await Commands.TransferItemsNormal(access, args[1], args[2], args[3]).ConfigureAwait(false),
			"ETRANSFER" when args.Length > 2 => await Commands.TransferItemsNormalWithBot(bot, access, args[1], args[2]).ConfigureAwait(false),
			"ETRANSFER^" when args.Length > 3 => await Commands.TransferItemsAdvanced(access, args[1], args[2], args[3]).ConfigureAwait(false),
			"ETRANSFER^" when args.Length > 2 => await Commands.TransferItemsAdvancedWithBot(bot, access, args[1], args[2]).ConfigureAwait(false),
			_ => null
		};
	}

	public async Task<bool> OnBotTradeOffer(Bot bot, TradeOffer? tradeOffer) {
		if (tradeOffer == null) {
			ASF.ArchiLogger.LogNullError(tradeOffer);

			return false;
		}

		byte? holdDuration = await bot.GetTradeHoldDuration(tradeOffer.OtherSteamID64, tradeOffer.TradeOfferID).ConfigureAwait(false);

		if (!holdDuration.HasValue) {
			return false;
		}

		List<ulong>? bots = Bot.GetBots("ASF")?.Select(static b => b.SteamID).ToList();

		if (bots == null) {
			return false;
		}

		ASF.ArchiLogger.LogGenericInfo($"Accepted trade request from {tradeOffer.OtherSteamID64}");

		return bots.Contains(tradeOffer.OtherSteamID64);
	}
}
#pragma warning restore CA1812 // ASF uses this class during runtime
