## RiskyMod
A full-game overhaul that aims to bring it closer to RoR1's balance. Rigorously multiplayer playtested by long-time RoR1/Starstorm players!

Unlike other balance mods, this mod aims to prioritize fun over widespread nerfs in the name of balance.

The general trend is that most survivor kits have been buffed to be on-par with the later RoR2 survivors, while item proc chains have been nerfed so that lategame is more about using survivor skills instead of autowiping the map.

### [Full Changelist](https://thunderstore.io/package/Risky_Lives/RiskyMod/wiki/)

### [RiskyStarterPack](https://thunderstore.io/package/Risky_Lives/RiskyStarterPack/)
- Multiplayer modpack with a bunch of new survivors and items.

**Contact me if you're interested in translating the mod!**

## Compatibility

This mod is compatible with nearly all content mods. The only things that will conflict are other mods that modify the Vanilla balance such as FlatItemBuff, but you can make it work by editing your RiskyMod config to disable conflicting features (ex. make sure you dont have both mods trying to modify Leeching Seed).

- Mandatory Dependencies:
	- RiskyTweaks
	- RiskyFixes
	- AssistManager
	- DefenseMatrixManager
	
- Optional Dependencies
	- EliteReworks, included since RiskyMod is designed with it in mind.
	- LinearDamage

## Main Features

- Oneshot Protection now actually works.
	- Oneshot = Going from >90% HP to 0% HP in under 0.5s
	- Once triggered, you have to be out of danger (no damage for 7s) before it can be retriggered.
- Big balance pass over nearly all the items.
	- Bad items like Squid Polyps and Warbanners have gotten substantial buffs.
	- Strong damage items like AtGs and Elemental Bands now have lowered damage stacking.
	- Most forms of range scaling have been removed, and most items aren't able to proc chain anymore.
- Big balance pass over all the Survivors
	- Less useless skill options, default Commando is good now!
	- General power level is meant to be closer to later RoR2 survivors.
- Drones scale better.
- Teleporter radius expands once the boss is dead.
- Void Fields is shorter and only lasts 5 waves.
	
## For Developers

To make custom allies compatible with RiskyMod's ally changes, use these items located in AllyItems.cs:

- AllyMarkerItem makes the holder benefit from Ally damage resistances. No effect when stacked.
- AllyScalingItem changes level scaling from +30% HP +20% damage -> +20% HP +30% damage so that allies perform the same on all stages. **Note that this corresponds to a config option that is disabled by default.**
- AllyRegenItem makes the holder's HP regenerate to full in X seconds, X being the amount of stacks.
- AllyAllowVoidDeathItem makes the holder bypass the "No Void Death" config option.
- AllyAllowOverheatDeathItem makes the holder bypass the "No Overheat" config option.

These are always initialized even if Ally changes are disabled and their effects have config-checking built-in, so you can simply add them to your allies without having to worry about anything.

Make sure these are only given to Player-team allies, since they'll be painful to fight against (in a bad/unintuitive way) when they're given to enemies.

*Feel free to take features of this mod and release them as standalone as long as you link the original GitHub repo in the README.*
	
## Credits

Drone Targeting fix - ZetTweaks

Squid Polyp Distraction - DestroyedClone

Language Support - Anreol

Skill Icons - KoobyKarasu, Thingw, Glad

Acrid Hitbox Tweaks - TheTimesweeper

Buff Icons - SOM, VALE-X

Brazilian Portuguese Translation - Kauzok

/vm/ (formerly /v/)

## Translations

- Brazilian Portuguese: Kauzok
- Chinese: JunJun_W, White-Biggy
- German: tymmey
- Russian: inkyarev
- Korean: bird_wall
- French: DarSitam