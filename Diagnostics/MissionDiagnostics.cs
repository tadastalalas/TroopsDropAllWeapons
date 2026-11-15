using System.Collections;
using System.Reflection;
using HarmonyLib;
using TaleWorlds.MountAndBlade;
using TroopsDropAllWeapons.Core;

namespace TroopsDropAllWeapons.Diagnostics;

// Mission counters (drops, spawned items, runtime ids, objects, missiles, agents), written to the log
// every Balance.DiagnosticsIntervalSeconds while "Logging for debugging" is on.
internal static class MissionDiagnostics
{
    private const string SpawnedItemsFieldName = "_spawnedItemEntitiesCreatedAtRuntime";

    private static readonly FieldInfo? SpawnedItemsField = AccessTools.Field(typeof(Mission), SpawnedItemsFieldName);

    private static Mission? _trackedMission;
    private static float _timeSinceLastLog;
    private static int _modDropCount;

    internal static void CountModDrop() => _modDropCount++;

    // Runs every mission tick: nothing is allocated until a line is due.
    internal static void Tick(Mission mission, float dt)
    {
        if (!ModSettings.LoggingEnabled) return;

        if (!ReferenceEquals(_trackedMission, mission))
        {
            _trackedMission = mission;
            _timeSinceLastLog = 0f;
            _modDropCount = 0;
            TdawLog.Debug("=== New mission ===");
        }

        _timeSinceLastLog += dt;
        if (_timeSinceLastLog < Balance.DiagnosticsIntervalSeconds) return;

        _timeSinceLastLog = 0f;

        int spawnedItems = (SpawnedItemsField?.GetValue(mission) as ICollection)?.Count ?? -1;
        int runtimeIdHighWater = RuntimeIdPool.HighWater(mission);
        int freedIds = RuntimeIdPool.FreedCount;

        TdawLog.Debug($"t={mission.CurrentTime:F0}s modDrops={_modDropCount} spawnedItems={spawnedItems} " +
                      $"runtimeIdHighWater={runtimeIdHighWater}/{Balance.RuntimeIdCap} freedIds={freedIds} " +
                      $"oldestFreedAge={RuntimeIdPool.OldestFreedAge:F0}s " +
                      $"missionObjects={mission.MissionObjects?.Count ?? -1} " +
                      $"missiles={mission.MissilesList?.Count ?? -1} " +
                      $"agents={mission.AllAgents?.Count ?? -1}");

        if (runtimeIdHighWater >= Balance.RuntimeIdCap && freedIds < Balance.ExhaustedFreedIdThreshold)
            TdawLog.Debug("!!! RUNTIME MISSION OBJECT ID POOL EXHAUSTED: this causes the stuck battle !!!");
    }
}
