using HarmonyLib;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TroopsDropAllWeapons.Core;
using TroopsDropAllWeapons.Diagnostics;

namespace TroopsDropAllWeapons.Patches;

// TDAW-FEATURE: Troops drop all their weapons on death, holstered ones too | gate: setting:DropArrowsBolts/DropThrowingWeapons/DropBowsCrossbows/DropMeleeWeapons/DropShields (+ DoNotDropEmptyAmmo, ArenaOnly) | Postfix: drops what is left after vanilla removed the agent
[HarmonyPatch(typeof(Mission), "OnAgentRemoved")]
internal class MissionOnAgentRemovedPatch
{
    private static void Postfix(Agent affectedAgent, AgentState agentState)
    {
        if (GameNetwork.IsClientOrReplay || agentState == AgentState.Routed) return;
        if (affectedAgent == null || !affectedAgent.IsHuman || affectedAgent.AgentVisuals == null) return;
        if ((affectedAgent.GetAgentFlags() & AgentFlag.CanWieldWeapon) == 0) return;
        if (DropRules.IsBlockedByArenaOnly(Mission.Current)) return;

        for (EquipmentIndex slot = EquipmentIndex.WeaponItemBeginSlot; slot < EquipmentIndex.ExtraWeaponSlot; slot++)
        {
            MissionWeapon weapon = affectedAgent.Equipment[slot];
            if (weapon.IsEmpty) continue;

            WeaponComponentData? primaryWeapon = weapon.Item?.PrimaryWeapon;
            if (primaryWeapon == null) continue;
            if (!DropRules.ShouldDrop(primaryWeapon.WeaponClass)) continue;
            if (DropRules.IsSkippedEmptyAmmo(primaryWeapon, weapon)) continue;

            DropRules.DropItem(affectedAgent, slot, primaryWeapon.WeaponClass);
        }
    }
}

// TDAW-FEATURE: Mission counters in the log file | gate: setting:LoggingEnabled | Postfix: reads the counters after vanilla ticked
[HarmonyPatch(typeof(Mission), nameof(Mission.OnTick))]
internal class MissionOnTickPatch
{
    private static void Postfix(Mission __instance, float dt)
    {
        if (__instance == null) return;
        MissionDiagnostics.Tick(__instance, dt);
    }
}

// TDAW-FEATURE: Freed battle object ids are reused oldest first, so long battles do not run out of them | gate: always | Prefix: replaces vanilla's id pick
[HarmonyPatch(typeof(Mission), nameof(Mission.GetFreeRuntimeMissionObjectId))]
internal class MissionGetFreeRuntimeMissionObjectIdPatch
{
    // Skips vanilla: vanilla reuses the newest freed id, RuntimeIdPool the oldest.
    private static bool Prefix(Mission __instance, ref int __result)
    {
        if (__instance == null) return true;
        __result = RuntimeIdPool.Take(__instance);
        return false;
    }
}

// TDAW-FEATURE: Freed battle object ids are reused oldest first, so long battles do not run out of them | gate: always | Prefix: freed ids go to RuntimeIdPool
[HarmonyPatch(typeof(Mission), "ReturnRuntimeMissionObjectId")]
internal class MissionReturnRuntimeMissionObjectIdPatch
{
    // Skips vanilla: the freed id goes to RuntimeIdPool instead of vanilla's stack.
    private static bool Prefix(Mission __instance, int id)
    {
        if (__instance == null) return true;
        RuntimeIdPool.Return(__instance, id);
        return false;
    }
}

// TDAW-FEATURE: Freed battle object ids are reused oldest first, so long battles do not run out of them | gate: always | Postfix: empties RuntimeIdPool when vanilla resets the runtime ids
[HarmonyPatch(typeof(Mission), "RemoveSpawnedMissionObjects")]
internal class MissionRemoveSpawnedMissionObjectsPatch
{
    private static void Postfix() => RuntimeIdPool.Clear();
}
