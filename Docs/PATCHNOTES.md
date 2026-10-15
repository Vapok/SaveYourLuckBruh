# 1.0.4 - Unified Star Pity & Game Compatibility
* **Valheim 1.0.20 Assembly Migration**:
  * Migrated from `CharacterDrop.s_pseudoCounter` type `Dictionary<string, Tuple<float, int>>` to `Dictionary<int, int>` introduced in Valheim 1.0.20.
  * Updated `DropEvaluationState` and `StorageManager` to handle integer hash keys while maintaining backwards-compatible deserialization for legacy `prefab:chance:counter` strings in `Player.m_customData`.
* **Unified Star-Level Pity Architecture**:
  * Vanilla 1.0.20 hashes `s_pseudoCounter` as `hash(prefabName) + (chance * 1000)`. Because star levels multiply drop chance, 0-star and 1-star enemies create disconnected keys, while 2-star enemies exceed the 30% cutoff and are disqualified from pseudo-drops.
  * In `CharacterDropPatch.Prefix`, the mod maps base countdown progress into vanilla's expected `starHash` for 1-star mobs.
  * In `CharacterDropPatch.Postfix`, temporary star hashes are stripped to avoid save file pollution. Base progress is updated with star-weighted decrements (1-star = 2x weight, 2-star = 4x weight).
  * 2-star enemies that fail standard RNG advance base pity and trigger guaranteed drops when the threshold is reached.
  * Any drop from any star tier resets the unified base counter, preventing unintended back-to-back pity drops.
* **Character Preview Static Leak Prevention**:
  * In `PlayerLoadPatch.Prefix`, `CharacterDrop.s_pseudoCounter.Clear()` is invoked when loading outside active gameplay or during `FejdStartup` character preview to prevent static collection merging across characters.
  * Synchronized `FejdStartupAwakePatch` on `FejdStartup.Awake` to ensure clean dictionary state on return to main menu.
* **Co-op Attacker Attribution**:
  * In `CharacterDropPatch.Postfix`, restored host's bad luck counter if the kill was delivered by an ally or the environment, preventing local player pity consumption during co-op sessions.

# 1.0.3 - Dependency Updates & Stability
* **Dependency & Engine Alignment**:
  * Updated internalized dependency `Vapok.Valheim.Common` to 3.1017.1.
  * Aligned game assembly references to Valheim 1.0.17.

# 1.0.2 - Compatibility Stability
* **Compatibility Stability**: Updated dependencies for third party mod compatibilities.

# 1.0.1 - Compatibility & Patch Sequencing
* **Harmony Patch Ordering Alignment (`FejdStartupPatches`)**:
  * Updated `FejdStartupAwakePatch` on `FejdStartup.Awake` to order `[HarmonyAfter]` `vapok.common.LocalizationManager` and `[HarmonyBefore]` `vapok.common.ItemManager` alongside legacy `org.bepinex.helpers.*` identifiers.

# 1.0.0 - Initial Release
* **Architecture & Persistence**:
  * Serializes `CharacterDrop.s_pseudoCounter` into `Player.m_customData` scoped by `ZNet.instance.GetWorldUID()`.
  * Harmony prefix on `Player.Save` serializes active countdowns to `.fch` binary storage.
  * Harmony postfix on `Player.Load` and `Player.OnSpawned` restores world-specific counters.
  * Harmony postfix on `Player.OnDestroy` clears in-memory counters to prevent cross-character contamination.
  * Real-time cache synchronization on `CharacterDrop.GenerateDropList` postfix updates memory state on creature death.
* **Multiplayer Safety**:
  * Client-side only execution model; dedicated server compatible with no server install required.
  * Non-destructive patches with zero return value modification on `GenerateDropList`.
* **Dependencies & Tooling**:
  * Compiled against Valheim 1.0.16 publicized assemblies and Unity 6 (`6000.0.75f1`).
  * Merged with `Vapok.Valheim.Common` 3.23.1016 via `ILRepack`.
  * Referenced `JotunnLib` 2.30.2.
