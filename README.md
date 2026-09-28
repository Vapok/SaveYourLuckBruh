<div align="center">

# 🍀 SaveYourLuckBruh

### *Persists vanilla Valheim pseudo-random drop counters so your bad luck equity is never lost across game sessions or world travels.*

[![GitHub Release](https://img.shields.io/github/v/release/Vapok/SaveYourLuckBruh?include_prereleases&logo=github&style=for-the-badge)](https://github.com/Vapok/SaveYourLuckBruh/releases)
[![Thunderstore Version](https://img.shields.io/thunderstore/v/Vapok/SaveYourLuckBruh?logo=thunderstore&style=for-the-badge)](https://thunderstore.io/c/valheim/p/Vapok/SaveYourLuckBruh/)
[![Nexus Mods](https://img.shields.io/badge/Nexus_Mods-Available-da8e35?logo=nexusmods&style=for-the-badge)](https://www.nexusmods.com/valheim/mods/4084)
<br>
[![Discord](https://img.shields.io/badge/Discord-Join%20Community-7289da?logo=discord&logoColor=white&style=for-the-badge)](https://discord.gg/5YAJkRFBXt)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg?style=for-the-badge)](https://opensource.org/licenses/MIT)

---

</div>

In Valheim, rare item drops (such as trophies and rare materials) feature an internal "bad luck protection" system designed to prevent prolonged dry streaks. However, in the vanilla game, this counter is stored purely in temporary memory and resets to zero every time you close the game.

**SaveYourLuckBruh** fixes this by saving your bad luck progress directly into your character save file, preserving your hard-earned drop progress across restarts and across different worlds.

---

<div align="center">

<br>

[![Survival Servers](https://raw.githubusercontent.com/Vapok/SaveYourLuckBruh/main/images/survivalservers_banner.png)](https://www.survivalservers.com/services/game_servers/valheim/?ref=vapok)

</div>

## 🎯 The Problem

Vanilla Valheim uses a hidden countdown (`s_pseudoCounter`) for any item with a drop rate of 30% or lower. Every time you kill an eligible creature without getting the rare item, the countdown ticks closer to a **guaranteed drop**.

### Example: Hunting Cultist Trophies
1. **The Math**: A Cultist Trophy has a base 10% drop chance. The game rolls an internal countdown between 1 and 20 kills.
2. **Session 1**: You clear Frost Caves and defeat 18 Cultists without seeing a trophy drop. You are now 1 or 2 kills away from a guaranteed trophy.
3. **The Flaw**: You log off for the night and close Valheim.
4. **Session 2 (Vanilla)**: You boot up Valheim tomorrow. **Your 18 kills are gone.** The game re-rolls a brand-new countdown from scratch. If you repeatedly play in short 1–2 hour sessions, you might suffer dozens of dry kills because your bad luck equity never survives a game restart.

---

## 💡 The Solution

**SaveYourLuckBruh** automatically saves your active bad luck counters into your character's native custom save data (`Player.m_customData`):

* 💾 **Persistent Across Restarts**: When you exit Valheim and launch it again the next day, your exact remaining countdown is restored.
* 🌍 **World-Scoped Progress**: Counters are tracked per unique World ID. Hunting Cultists in your solo world will not bleed into or reset your progress on a multiplayer server.
* ⚔️ **Attacker Attribution**: In co-op play, your bad luck countdown is only consumed when you (or your tamed pets) deliver the lethal blow—an ally landing a kill won't drain your personal bad luck equity.
* 🛡️ **Zero Corrupted Saves**: Stored entirely within vanilla character fields. If you ever uninstall the mod, your character file (`.fch`) remains 100% intact and uncorrupted.
* 🔌 **Client-Side Only**: Does not need to be installed on dedicated servers. Works seamlessly in singleplayer, co-op, or multiplayer servers without affecting unmodded players.

---

## ⚙️ Configuration

**SaveYourLuckBruh** is an install-and-forget mod. It requires no functional configuration or gameplay tweaking to work. 

Standard logging settings (including debug logging) are available via `BepInEx/config/vapok.mods.SaveYourLuckBruh.cfg`:

* **Enable Debug Mode**: Outputs detailed log statements to `LogOutput.log` showing when counters load, decrement, trigger drops, and save.

---

## 🤝 Compatibility & Requirements

* **BepInEx**: 5.4.2350 or later.
* **Jotunn**: 2.30.2 or later.
