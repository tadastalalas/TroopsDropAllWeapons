using System;
using System.Reflection;
using HarmonyLib;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TroopsDropAllWeapons.Core;
using TroopsDropAllWeapons.Diagnostics;

namespace TroopsDropAllWeapons.Compat;

// TDAW-FEATURE: Artem's Lively Animations compat: its "drop all weapons" follows this mod's drop rules, empty quivers/pouches are removed instead of dropped | gate: always (ArtemCore installed) | Prefix: replaces ArtemCore.DropAllWeapons
internal static class ArtemCoreCompat
{
    private const string ArtemCoreTypeName = "ArtemCore.ArtemCore";
    private const string DropAllWeaponsMethodName = "DropAllWeapons";

    private static MethodInfo? _patchedDropAllWeapons;

    private static MethodInfo PrefixMethod => AccessTools.Method(typeof(ArtemCoreCompat), nameof(Prefix));

    internal static void TryApply(Harmony harmony)
    {
        // ArtemCore is optional and not referenced at build time.
#pragma warning disable BHA0003
        Type? artemCoreType = AccessTools.TypeByName(ArtemCoreTypeName);
#pragma warning restore BHA0003
        if (artemCoreType == null)
        {
            TdawLog.Debug("ArtemCore not installed: ArtemCore compat off.");
            return;
        }

        MethodInfo? dropAllWeapons = AccessTools.Method(artemCoreType, DropAllWeaponsMethodName, [typeof(Agent)]);
        if (dropAllWeapons is not { IsPublic: true, IsStatic: true })
        {
            TdawLog.Warn($"{ArtemCoreTypeName}.{DropAllWeaponsMethodName}(Agent) not found (other ArtemCore version?): ArtemCore compat off.");
            return;
        }

        harmony.Patch(dropAllWeapons, new HarmonyMethod(PrefixMethod));
        _patchedDropAllWeapons = dropAllWeapons;
        TdawLog.Debug($"Patched {ArtemCoreTypeName}.{DropAllWeaponsMethodName}: ArtemCore drops follow this mod's rules.");
    }

    internal static void TryUnpatch(Harmony? harmony)
    {
        if (harmony == null || _patchedDropAllWeapons == null) return;

        harmony.Unpatch(_patchedDropAllWeapons, PrefixMethod);
        _patchedDropAllWeapons = null;
    }

    // Skips ArtemCore's own drop: the weapons are dropped by this mod's rules instead ("Arena Only" is not applied here).
    private static bool Prefix(Agent agent)
    {
        if (agent == null || !agent.IsActive() || !agent.ActionSet.IsValid) return true;

        for (EquipmentIndex slot = EquipmentIndex.WeaponItemBeginSlot; slot < EquipmentIndex.ExtraWeaponSlot; slot++)
        {
            MissionWeapon weapon = agent.Equipment[slot];
            if (weapon.IsEmpty) continue;

            WeaponComponentData? primaryWeapon = weapon.Item?.PrimaryWeapon;
            if (primaryWeapon == null) continue;
            if (!DropRules.ShouldDrop(primaryWeapon.WeaponClass)) continue;

            if (DropRules.IsSkippedEmptyAmmo(primaryWeapon, weapon))
            {
                agent.RemoveEquippedWeapon(slot);
                continue;
            }

            DropRules.DropItem(agent, slot, primaryWeapon.WeaponClass);
        }

        return false;
    }
}
