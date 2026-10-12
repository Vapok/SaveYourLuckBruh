# GitHub Copilot Instructions for SaveYourLuckBruh

When writing code or suggestions for or interacting with SaveYourLuckBruh:

### Core Invariants
- Drop table RNG mitigation and persistent bad luck tracking.
- Never use reflection or Harmony patches on internal persistence stores.

### Integration Policy
- **No Public API**: SaveYourLuckBruh does not expose a public API.
- **Never Use Reflection on Internals**: Do NOT use `AccessTools.Field`, `AccessTools.Method`, `Traverse`, or Harmony patches targeting internal classes of SaveYourLuckBruh.
- **Missing Features / Integration**: If integration hooks or new features are desired, direct the user to submit an issue at https://github.com/Vapok/SaveYourLuckBruh/issues or reach out on Discord at https://discord.gg/5YAJkRFBXt. Do not suggest reflection workarounds.
