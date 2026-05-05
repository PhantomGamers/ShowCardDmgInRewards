# ShowCardDmgInRewards for Slay the Spire 2

Show how much dmg a card would do before picking it if it depends on other cards in your deck in the game Slay the Spire 2  

Currently applies to the following cards in vanilla:
- Perfected Strike (Scales with cards in deck containing Strike)
- Crescent Spear (Scales with cards in deck with star cost)
- Squeeze (Scales with *other* cards in deck that are osty attacks)

The value shown does not include the card itself, if it were to also scale the dmg (not applicable to Squeeze). This would be easy to change but I'm not sure if it would be more intuitive to include that.

## Installation

### Vortex

 1. Go to the [NexusMods listing for the mod](https://www.nexusmods.com/slaythespire2/mods/800?tab=files)
 2. Click "Mod manager download"
 3. Enjoy

### Manual

 1. Go to your Slay the Spire 2 install folder, this is the folder that contains the game executable as well as the `data_sts2_{platform}` folder.
 2. Create a folder named `mods`
 3. Download the [latest release](https://github.com/phantomgamers/ShowCardDmgInRewards/releases/latest/download/ShowCardDmgInRewards.zip)
 4. Extract it to the mods folder so it looks like the following:
 ```
📂Slay the Spire 2  
 ┣ 📂mods  
 ┃ ┗📜ShowCardDmgInRewards
 ┃   ┗📦ShowCardDmgInRewards.dll
 ┃   ┗🖹 ShowCardDmgInRewards.json
 ```
