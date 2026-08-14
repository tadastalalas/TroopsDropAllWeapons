using System.Collections.Generic;
using HarmonyLib;
using TaleWorlds.MountAndBlade;

namespace TroopsDropAllWeapons
{
    [HarmonyPatch(typeof(Mission))]
    internal static class RuntimeIdAllocatorPatch
    {
        private const int IdCap = 8191;
        private const float ReuseCooldownSeconds = 30f;

        private static readonly Queue<(int Id, float FreedAt)> FreedIds = new Queue<(int, float)>(1024);
        private static Mission _trackedMission;

        private static readonly AccessTools.FieldRef<Mission, int> LastIdRef =
            AccessTools.FieldRefAccess<Mission, int>("_lastRuntimeMissionObjectIdCount");

        internal static int FreedCount => FreedIds.Count;

        internal static float OldestFreedAge =>
            FreedIds.Count > 0 ? MBCommon.GetTotalMissionTime() - FreedIds.Peek().FreedAt : -1f;

        [HarmonyPatch("GetFreeRuntimeMissionObjectId")]
        [HarmonyPrefix]
        private static bool GetFreePrefix(Mission __instance, ref int __result)
        {
            EnsureMission(__instance);
            ref int lastId = ref LastIdRef(__instance);
            float now = MBCommon.GetTotalMissionTime();

            if (FreedIds.Count > 0 && (now - FreedIds.Peek().FreedAt > ReuseCooldownSeconds || lastId >= IdCap))
            {
                __result = FreedIds.Dequeue().Id;
            }
            else if (lastId < IdCap)
            {
                __result = lastId;
                lastId++;
            }
            else
            {
                __result = -1;
            }

            return false;
        }

        [HarmonyPatch("ReturnRuntimeMissionObjectId")]
        [HarmonyPrefix]
        private static bool ReturnPrefix(Mission __instance, int id)
        {
            EnsureMission(__instance);
            FreedIds.Enqueue((id, MBCommon.GetTotalMissionTime()));
            return false;
        }

        [HarmonyPatch("RemoveSpawnedMissionObjects")]
        [HarmonyPostfix]
        private static void ResetPostfix() => FreedIds.Clear();

        private static void EnsureMission(Mission mission)
        {
            if (ReferenceEquals(_trackedMission, mission))
                return;

            _trackedMission = mission;
            FreedIds.Clear();
        }
    }
}