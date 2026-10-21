# 1.1.2 - Bug Fixes & Co-op Drop Protection
* **New Character Isolation**: Fixed an issue where creating a new character could inherit bad luck drop progress from the previously previewed character.
* **Starred Enemy Pity Progress**: Fixed an issue where the first starred kill for an unencountered item could prematurely trigger a guaranteed or near-guaranteed drop.
* **Co-op Ally Drop Reset**: Fixed an issue where rare drops could drop repeatedly on consecutive ally kills in multiplayer.

<details>
<summary><b>1.0 Changelog History (Valheim Release)</b> (<i>click to expand</i>)</summary>

### 1.1.1 - Dependency Updates & Valheim 1.0.20 Alignment
* **Compatibility**: Updated internal dependencies for stability and Valheim 1.0.20 compatibility.

### 1.1.0 - Unified Star Pity & Game Compatibility
* **Valheim 1.0.20 Support**: Updated for compatibility with the latest game patch.
* **Unified Star Progress**: Enemies with 0, 1, or 2 stars now share the same bad luck counter for rare trophy drops instead of splitting your progress.
* **Star Enemy Bonus**: Defeating stronger star enemies advances your bad luck counter faster.
* **Character Isolation**: Fixed a vanilla game issue where browsing characters on the main menu mixed up their bad luck progress.

### 1.0.3 - Dependency Updates & Stability
* Updated internal dependencies for stability.

### 1.0.2 - Compatibility Stability
* **Compatibility Stability**: Updated dependencies for third party mod compatibilities.

### 1.0.1 - Compatibility & Patch Sequencing
* Updated startup initialization to ensure patch sequencing aligns properly with internal helper libraries.

### 1.0.0 - Initial Release
* **Persistent Bad Luck Protection**: Rare drop countdowns now save directly to your character, preventing progress from wiping when you close the game.
* **World-Scoped Tracking**: Drop countdowns are saved separately for each world, keeping your solo and multiplayer progress isolated.
* **Zero Configuration**: Functions out of the box with no extra settings or setup required.
* **Dependencies**: Built with Jotunn 2.30.2 for Valheim 1.0.16.

</details>
