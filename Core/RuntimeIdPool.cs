using System.Collections.Generic;
using HarmonyLib;
using TaleWorlds.MountAndBlade;

namespace TroopsDropAllWeapons.Core;

// Runtime mission object ids: freed ids are reused oldest first (vanilla reuses the newest first).
internal static class RuntimeIdPool
{
    private const string LastIdFieldName = "_lastRuntimeMissionObjectIdCount";

    private static readonly Queue<(int Id, float FreedAt)> FreedIds = new(Balance.FreedRuntimeIdQueueCapacity);
    private static readonly AccessTools.FieldRef<Mission, int> LastIdRef = AccessTools.FieldRefAccess<Mission, int>(LastIdFieldName);
    private static Mission? _trackedMission;

    internal static int FreedCount => FreedIds.Count;

    internal static float OldestFreedAge => FreedIds.Count > 0 ? MBCommon.GetTotalMissionTime() - FreedIds.Peek().FreedAt : -1f;

    internal static int HighWater(Mission mission) => LastIdRef(mission);

    internal static int Take(Mission mission)
    {
        EnsureMission(mission);
        ref int lastId = ref LastIdRef(mission);
        float now = MBCommon.GetTotalMissionTime();

        if (FreedIds.Count > 0 && (now - FreedIds.Peek().FreedAt > Balance.RuntimeIdReuseCooldownSeconds || lastId >= Balance.RuntimeIdCap))
            return FreedIds.Dequeue().Id;

        if (lastId >= Balance.RuntimeIdCap)
            return -1;

        int id = lastId;
        lastId++;
        return id;
    }

    internal static void Return(Mission mission, int id)
    {
        EnsureMission(mission);
        FreedIds.Enqueue((id, MBCommon.GetTotalMissionTime()));
    }

    internal static void Clear() => FreedIds.Clear();

    private static void EnsureMission(Mission mission)
    {
        if (ReferenceEquals(_trackedMission, mission)) return;

        _trackedMission = mission;
        FreedIds.Clear();
    }
}
