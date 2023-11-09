# ASF-EnhancedTransfer

ArchiSteamFarm plugin that enhances the functionality of the transfer command.

## How To Install
1. Go to [**Releases.**](https://github.com/WiLuX-Source/ASF-EnhancedTransfer/releases)
2. Find the **latest version**
3. **Download** the zip file.
4. Extract it to **root folder of ASF.**
5. Check plugins tab to see if it worked or not.

Enjoy!

## How to use
When you install this mod, it enables a passive behavior that allows you to **acquire items from your bots by requesting them through regular trades**. Normally, such requests would be **ignored/rejected** in standard ASF.

There are also a few commands you can use.

## Commands
Plugin implements following commands:

Command | Access | Description
--- | --- | ---
`etransfer <Bots> <Modes> <Bot>` | `Master` | Sends from given `<Bots>` instances to given `Bot` instance, all Steam inventory items that are matching given `modes`, explained **[below](#modes-parameter)**.
`etransfer^ <Bots> <ModeAdvanced> <Bot>` | `Master` | Sends from given `<Bots>` instances to given `Bot` instance, all special inventory items that are matching given `AdvancedMode`, explained **[below](#advancedmode-parameter)**.
## `Modes` parameter

`<Modes>` argument accepts multiple mode values, separated as usual by a comma. Available mode values are specified below:

Value | Alias | Description
--- | --- | ---
All | A | Same as enabling all item types below
Background | BG | Profile background to use on your Steam profile
Booster | BO | Booster pack
Card | C | Steam trading card, being used for crafting badges (non-foil)
Emoticon | E | Emoticon to use in Steam Chat
Foil | F | Foil variant of `Card`
Gems | G | Steam gems being used for crafting boosters, sacks included
Unknown | U | Every type that doesn't fit in any of the above

## `AdvancedMode` parameter

`<AdvancedMode>` argument accepts one mode value, available mode values are specified below:

Value | Alias | Description
--- | --- | ---
Case | CS | Sends CS2 cases.
Key | TF2 | Sends TF2 Keys.

## Planned
These are things I plan to add for now...
- [ ] IPC Api
- [ ] Setting for disabling passive behavior.
- [ ] `<AdvancedMode>` take multiple arguments.

## Inspirations & Sources
- [ASF-PluginTemplate](https://github.com/JustArchiNET/ASF-PluginTemplate)
- [ASF-Wiki](https://github.com/JustArchiNET/ArchiSteamFarm/wiki/Plugins)
- [Selective-Loot-Transfer](https://github.com/Rudokhvist/Selective-Loot-and-Transfer-Plugin)
- [ItemDispenser](https://github.com/Rudokhvist/ItemDispenser)

## Special Thanks
- [Rudokhvist](https://github.com/Rudokhvist)