using System;
using System.Collections;
using System.IO;
using System.Reflection;
using HarmonyLib;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace TroopsDropAllWeapons
{
    [HarmonyPatch(typeof(Mission), nameof(Mission.OnTick))]
    internal static class MissionDiagnosticsPatch
    {
        private const float LogIntervalSeconds = 10f;
        private const int RuntimeIdCap = 8191;

        private static readonly FieldInfo SpawnedItemsField = AccessTools.Field(typeof(Mission), "_spawnedItemEntitiesCreatedAtRuntime");
        private static readonly FieldInfo LastRuntimeIdField = AccessTools.Field(typeof(Mission), "_lastRuntimeMissionObjectIdCount");
        private static readonly FieldInfo EmptyRuntimeIdsField = AccessTools.Field(typeof(Mission), "_emptyRuntimeMissionObjectIds");

        private static readonly string LogPath = Path.Combine(BasePath.Name, "Modules", "TroopsDropAllWeapons", "TDAW_diagnostics.log");

        private static Mission _trackedMission;
        private static float _timeSinceLastLog;
        private static int _modDropCount;

        internal static void CountModDrop() => _modDropCount++;

        private static void Postfix(Mission __instance, float dt)
        {
            var settings = MCMSettings.Instance;
            if (settings == null || !settings.EnableDiagnostics)
                return;

            if (!ReferenceEquals(_trackedMission, __instance))
            {
                _trackedMission = __instance;
                _timeSinceLastLog = 0f;
                _modDropCount = 0;
                WriteLine("=== New mission ===");
            }

            _timeSinceLastLog += dt;
            if (_timeSinceLastLog < LogIntervalSeconds)
                return;

            _timeSinceLastLog = 0f;

            int spawnedItems = (SpawnedItemsField?.GetValue(__instance) as ICollection)?.Count ?? -1;
            int runtimeIdHighWater = LastRuntimeIdField?.GetValue(__instance) as int? ?? -1;
            int freedIds = RuntimeIdAllocatorPatch.FreedCount;

            string line = $"t={__instance.CurrentTime:F0}s modDrops={_modDropCount} spawnedItems={spawnedItems} " +
                          $"runtimeIdHighWater={runtimeIdHighWater}/{RuntimeIdCap} freedIds={freedIds} " +
                          $"oldestFreedAge={RuntimeIdAllocatorPatch.OldestFreedAge:F0}s " +
                          $"missionObjects={__instance.MissionObjects?.Count ?? -1} " +
                          $"missiles={__instance.MissilesList?.Count ?? -1} " +
                          $"agents={__instance.AllAgents?.Count ?? -1}";

            WriteLine(line);
            InformationManager.DisplayMessage(new InformationMessage($"[TDAW] {line}", Colors.Yellow));

            if (runtimeIdHighWater >= RuntimeIdCap && freedIds < 64)
            {
                WriteLine("!!! RUNTIME MISSION OBJECT ID POOL EXHAUSTED !!!");
                InformationManager.DisplayMessage(new InformationMessage("[TDAW] ID POOL EXHAUSTED — this causes the stuck battle!", Colors.Red));
            }
        }

        private static void WriteLine(string line)
        {
            try
            {
                File.AppendAllText(LogPath, $"[{DateTime.Now:HH:mm:ss}] {line}{Environment.NewLine}");
            }
            catch (Exception e) when (e is IOException || e is UnauthorizedAccessException)
            {
            }
        }
    }
}