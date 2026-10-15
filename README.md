<div align="center">

# 🍀 SaveYourLuckBruh

### *Unifies and fixes vanilla Valheim pseudo-random drop counters so bad luck equity is never fragmented or lost across game sessions, world travels, or star tiers.*

[![GitHub Release](https://img.shields.io/github/v/release/Vapok/SaveYourLuckBruh?include_prereleases&logo=github&style=for-the-badge)](https://github.com/Vapok/SaveYourLuckBruh/releases)
[![Thunderstore Version](https://img.shields.io/thunderstore/v/Vapok/SaveYourLuckBruh?logo=thunderstore&style=for-the-badge)](https://thunderstore.io/c/valheim/p/Vapok/SaveYourLuckBruh/)
[![Nexus Mods](https://img.shields.io/badge/Nexus_Mods-Available-da8e35?logo=nexusmods&style=for-the-badge)](https://www.nexusmods.com/valheim/mods/4084)
<br>
[![Discord](https://img.shields.io/badge/Discord-Join%20Community-7289da?logo=discord&logoColor=white&style=for-the-badge)](https://discord.gg/5YAJkRFBXt)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg?style=for-the-badge)](https://opensource.org/licenses/MIT)

---

</div>

In Valheim, rare item drops (such as trophies and rare materials) utilize an internal pseudo-random countdown (`s_pseudoCounter`) to guarantee drops after a set number of kills. 

In Patch 1.0.20, Iron Gate added basic character persistence for this dictionary and attempted to scale trophy drop rates with monster stars. However, their implementation introduced severe mechanical flaws: pity counters are split across star tiers, 2-star enemies are excluded from pity entirely, and the static dictionary leaks across characters on the character select screen.

**SaveYourLuckBruh** fixes these issues by unifying pity progression across all star tiers, isolating character saves from menu preview leaks, preserving per-world progression, and protecting your pity counters in co-op.

---

<div align="center">

<br>

[![Survival Servers](https://raw.githubusercontent.com/Vapok/SaveYourLuckBruh/main/images/survivalservers_banner.png)](https://www.survivalservers.com/services/game_servers/valheim/?ref=vapok)

</div>

## 🎯 The Problem with Vanilla 1.0.20

In Patch 1.0.20, Iron Gate made two major changes to `CharacterDrop`:
1. They serialized `CharacterDrop.s_pseudoCounter` into the `.fch` character save file.
2. They allowed monster star levels to multiply trophy drop chances.

While this addressed the basic issue of counters resetting on logout, their implementation created three critical problems:

### 1. Star-Level Pity Fragmentation
Iron Gate changed the dictionary key to a compound hash: `hash(prefabName) + (chance * 1000)`. Because chance scales with star level (0-star = 10%, 1-star = 20%, 2-star = 40%):
* A 0-star Cultist and a 1-star Cultist write to completely different dictionary entries.
* Kills on 0-star and 1-star enemies do not pool together. If you kill 8 0-stars and 2 1-stars, neither counter advances the other.
* Because 2-star Cultists have a 40% drop chance, they exceed vanilla's 30% cutoff (`chance <= 0.3f`). **2-star enemies are completely disqualified from pseudo drops.** They do not advance pity, and if a 2-star drops a trophy, your 0-star counter does not reset—giving you an unintended duplicate drop shortly after.

### 2. Static Memory Leak / Cross-Character Pollution
`CharacterDrop.s_pseudoCounter` is a `static` dictionary. In `Player.Load`, vanilla inserts saved entries without ever calling `.Clear()`. Because `Player.Load` is executed during character preview in the main menu, cycling through characters on the start screen merges all their counters together into memory. Whichever character you launch into the world inherits the combined counters of every previewed character.

### 3. Multiplayer ZDO Attribution & World Bleed
Only the network owner of a creature's `ZNetView` executes drop calculations. In co-op, an ally landing a kill can consume your personal countdown. Furthermore, vanilla counters are tied globally to the character file with zero world scoping, allowing players to farm pity in local worlds and cash it in on multiplayer servers.

---

## 💡 The Solution

| Feature | Vanilla 1.0.20 Behavior | SaveYourLuckBruh Behavior |
| :--- | :--- | :--- |
| **Unified Star Pity** | Star tiers have separate counters; 2-star mobs excluded. | All star tiers advance a single unified pity counter per item. |
| **Star Difficulty Weighting** | 1-star kills advance 1-star counter by 1. | 1-star kills count as 2 progress; 2-star kills count as 4 progress. |
| **2-Star Guarantee** | 2-star mobs cannot trigger guaranteed bad luck drops. | 2-star mobs advance pity and trigger guaranteed drops if threshold is hit. |
| **Counter Reset on Drop** | Dropping a trophy on a 1-star mob leaves 0-star counter intact. | Any drop of the target item from any star tier resets the unified counter. |
| **Character Select Isolation** | Previews in the main menu merge counters across characters. | Clears static dictionary in menu; keeps characters fully isolated. |
| **World Isolation** | Counters are global across all solo and server worlds. | Stores per-world snapshots in `Player.m_customData` scoped by World UID. |
| **Co-op Attribution** | Host/ZDO owner loses pity counter regardless of killer. | Preserves local player's pity counter when allies make the kill. |

---

## ⚙️ Configuration

**SaveYourLuckBruh** is an install-and-forget mod. It requires no functional configuration to work.

Standard logging settings are available in `BepInEx/config/vapok.mods.SaveYourLuckBruh.cfg`:
* **Enable SaveYourLuckBruh**: Master toggle to enable or disable mod functionality.
* **Enable Debug Mode**: Logs real-time counter changes, star weight calculations, resets, and attribution in `LogOutput.log`.

---

## 🤝 Compatibility & Requirements

* **Valheim**: 1.0.20 or later.
* **BepInEx**: 5.4.2351 or later.
* **Jotunn**: 2.30.2 or later.
* **Client-Side Only**: Does not need to be installed on dedicated servers. Works in singleplayer and multiplayer.

---

## 🔌 For Mod Developers & AI Coding Agents

* 🤖 **[AI Agent Rules & Invariants](AGENTS.md)**: Technical guidelines and repository invariants.
* ⚠️ **Integration Policy**: SaveYourLuckBruh is a self-contained mod without a public API. Do not use reflection or Harmony patches on internal classes.
* 💡 **Feature Requests**: Report bugs or suggest features on [GitHub Issues](https://github.com/Vapok/SaveYourLuckBruh/issues) or in [Discord](https://discord.gg/5YAJkRFBXt).
