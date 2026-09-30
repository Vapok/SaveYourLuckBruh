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
