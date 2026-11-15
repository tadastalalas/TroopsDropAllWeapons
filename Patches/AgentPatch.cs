using HarmonyLib;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TroopsDropAllWeapons.Core;

namespace TroopsDropAllWeapons.Patches;

// TDAW-FEATURE: Empty quivers/pouches are removed instead of dropped on the ground, vanilla drops too | gate: setting:ApplyEmptyAmmoCheckToVanilla | Prefix: skips the drop for an empty quiver/pouch
[HarmonyPatch(typeof(Agent), nameof(Agent.DropItem))]
internal class AgentPatch
{
    // Skips vanilla for an empty quiver/pouch: it is removed from the equipment instead of spawned as a ground item.
    private static bool Prefix(Agent __instance, EquipmentIndex itemIndex)
    {
        if (__instance == null || !ModSettings.ApplyEmptyAmmoCheckToVanilla) return true;
        if (!IsEmptyConsumableRangedSlot(__instance, itemIndex)) return true;

        __instance.RemoveEquippedWeapon(itemIndex);
        return false;
    }

    private static bool IsEmptyConsumableRangedSlot(Agent agent, EquipmentIndex slot)
    {
        if (slot < EquipmentIndex.WeaponItemBeginSlot || slot >= EquipmentIndex.ExtraWeaponSlot) return false;

        MissionWeapon weapon = agent.Equipment[slot];
        if (weapon.IsEmpty) return false;

        WeaponComponentData? primary = weapon.Item?.PrimaryWeapon;
        return primary != null && DropRules.IsEmptyConsumableRanged(primary, weapon);
    }
}
