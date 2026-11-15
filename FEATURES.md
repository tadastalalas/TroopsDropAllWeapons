# TroopsDropAllWeapons — Features

| Feature | Gate | File |
|---|---|---|
| Troops drop all their weapons on death, holstered ones too | setting:DropArrowsBolts / DropThrowingWeapons / DropBowsCrossbows / DropMeleeWeapons / DropShields | `Patches\MissionPatch.cs`, `Core\DropRules.cs` |
| Empty quivers/pouches are not dropped on death | setting:DoNotDropEmptyAmmo | `Core\DropRules.cs` |
| Empty quivers/pouches dropped by vanilla are removed instead of spawned on the ground | setting:ApplyEmptyAmmoCheckToVanilla | `Patches\AgentPatch.cs` |
| Drops only in arena practice and tournament fights | setting:ArenaOnly | `Core\DropRules.cs`, `Patches\MissionPatch.cs` |
| Freed battle object ids are reused oldest first, so long battles do not run out of them | always | `Patches\MissionPatch.cs`, `Core\RuntimeIdPool.cs`, `Core\Balance.cs` |
| Artem's Lively Animations: its "drop all weapons" follows this mod's drop rules | always (ArtemCore installed) | `Compat\ArtemCoreCompat.cs` |
| Mission counters in the log file | setting:LoggingEnabled | `Patches\MissionPatch.cs`, `Diagnostics\MissionDiagnostics.cs` |
| Settings: MCM when loaded, otherwise ModConfiguration.xml | always | `Core\ModSettings.cs`, `Data\MCMSettings.cs`, `Data\ModConfiguration.cs`, `_Module\ModConfiguration.xml` |
| Log file (problems always, debug lines only when enabled) | setting:LoggingEnabled | `Diagnostics\TdawLog.cs`, `Core\ModInfo.cs` |
| Localization (English + 12 languages) | — | `Core\Texts.cs`, `_Module\ModuleData\Languages\` |
