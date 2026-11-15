using System;
using SandBox.Missions.MissionLogics.Arena;
using SandBox.Tournaments.MissionLogics;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TroopsDropAllWeapons.Diagnostics;

namespace TroopsDropAllWeapons.Core;

// Drop rules shared by the death-drop patch, the vanilla empty-ammo patch and the ArtemCore compat.
internal static class DropRules
{
    private static bool _dropFailureLogged;

    // "Arena Only" is on and the mission is neither an arena practice fight nor a tournament fight.
    internal static bool IsBlockedByArenaOnly(Mission? mission) => ModSettings.ArenaOnly && !IsArenaMission(mission);

    internal static bool IsArenaMission(Mission? mission) =>
        mission != null
        && (mission.HasMissionBehavior<ArenaPracticeFightMissionController>()
            || mission.HasMissionBehavior<TournamentFightMissionController>());

    internal static bool ShouldDrop(WeaponClass weaponClass) => weaponClass switch
    {
        WeaponClass.Arrow or
        WeaponClass.Bolt or
        WeaponClass.SlingStone or
        WeaponClass.Cartridge => ModSettings.DropArrowsBolts,

        WeaponClass.ThrowingAxe or
        WeaponClass.ThrowingKnife or
        WeaponClass.Javelin or
        WeaponClass.Stone => ModSettings.DropThrowingWeapons,

        WeaponClass.Bow or
        WeaponClass.Crossbow or
        WeaponClass.Sling or
        WeaponClass.Pistol or
        WeaponClass.Musket => ModSettings.DropBowsCrossbows,

        WeaponClass.SmallShield or
        WeaponClass.LargeShield => ModSettings.DropShields,

        WeaponClass.Dagger or
        WeaponClass.OneHandedSword or
        WeaponClass.TwoHandedSword or
        WeaponClass.OneHandedAxe or
        WeaponClass.TwoHandedAxe or
        WeaponClass.Mace or
        WeaponClass.Pick or
        WeaponClass.TwoHandedMace or
        WeaponClass.OneHandedPolearm or
        WeaponClass.TwoHandedPolearm or
        WeaponClass.LowGripPolearm => ModSettings.DropMeleeWeapons,

        _ => false,
    };

    // Empty quiver/pouch that "Do Not Drop Empty Quivers/Pouches" keeps on the agent.
    internal static bool IsSkippedEmptyAmmo(WeaponComponentData primaryWeapon, MissionWeapon weapon) =>
        ModSettings.DoNotDropEmptyAmmo && IsEmptyConsumableRanged(primaryWeapon, weapon);

    internal static bool IsEmptyConsumableRanged(WeaponComponentData primaryWeapon, MissionWeapon weapon) =>
        primaryWeapon.IsConsumable && primaryWeapon.IsRangedWeapon && weapon.Amount == 0;

    internal static void DropItem(Agent agent, EquipmentIndex slot, WeaponClass weaponClass)
    {
        try
        {
            agent.DropItem(slot, weaponClass);
            MissionDiagnostics.CountModDrop();
        }
        catch (Exception e)
        {
            LogDropFailure(e);
        }
    }

    // Logged once per session so a failing drop cannot flood the log.
    private static void LogDropFailure(Exception e)
    {
        if (_dropFailureLogged) return;

        _dropFailureLogged = true;
        TdawLog.Error("DropItem failed.", e);
    }
}
