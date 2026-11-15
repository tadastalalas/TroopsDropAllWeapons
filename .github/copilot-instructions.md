TroopsDropAllWeapons: workspace instructions. They override the global file.

- <Prefix>: TDAW (Tdaw in type names). <Root> namespace and ModuleId: TroopsDropAllWeapons.
- Logger: `Diagnostics\TdawLog.cs` (Debug / Warn / Error).
- Harmony: PatchAll runs in `SubModule.OnGameInitializationFinished` (assembly passed explicitly), UnpatchAll in `OnGameEnd`,
  Harmony id "TroopsDropAllWeapons" (`Core\ModInfo.cs` HarmonyId). Patches are inactive before the first game finishes initializing.
- `ModSettings.Reload()` runs in `SubModule.OnGameStart` for every game (campaign and custom battle), not only campaigns.
- Patches: `Patches\MissionPatch.cs` (OnAgentRemoved, OnTick, runtime id pool), `Patches\AgentPatch.cs` (DropItem).
  Optional mod: `Compat\ArtemCoreCompat.cs` (ArtemCore.ArtemCore.DropAllWeapons), applied after PatchAll.
- `Core\ModInfo.cs` holds the ModuleId, Harmony id, file and folder names and the module root path. `Core\Balance.cs` holds the gameplay numbers.
- The mod saves nothing (no CampaignBehavior, no SyncData).
- Kept exception: the MCM Id stays "TroopsDropAllWeaponsSettings". It is released; changing it resets players' settings.
- Kept exception: existing text ids (TDAW_1a2B3c4 … TDAW_a4R5e6N) look sequential. Never rename them; new ids use 7 random characters.
