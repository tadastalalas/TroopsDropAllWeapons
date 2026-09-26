using System;
using HarmonyLib;
using SandBox.Missions.MissionLogics.Arena;
using SandBox.Tournaments.MissionLogics;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace TroopsDropAllWeapons
{
    [HarmonyPatch(typeof(Mission), "OnAgentRemoved")]
    internal static class DropWeaponPatch
    {
        private static bool _dropFailureLogged;

        private static void Postfix(Agent affectedAgent, AgentState agentState)
        {
            if (GameNetwork.IsClientOrReplay || agentState == AgentState.Routed)
                return;

            if (affectedAgent == null || !affectedAgent.IsHuman)
                return;

            if (affectedAgent.AgentVisuals == null)
                return;

            if ((affectedAgent.GetAgentFlags() & AgentFlag.CanWieldWeapon) == 0)
                return;

            var settings = MCMSettings.Instance;
            
            if (settings?.ArenaOnly == true && !IsArenaMission(Mission.Current))
                return;

            for (EquipmentIndex slot = EquipmentIndex.WeaponItemBeginSlot;
                 slot < EquipmentIndex.ExtraWeaponSlot;
                 slot++)
            {
                var weapon = affectedAgent.Equipment[slot];
                if (weapon.IsEmpty)
                    continue;

                var primaryWeapon = weapon.Item?.PrimaryWeapon;
                if (primaryWeapon == null)
                    continue;

                if (!ShouldDrop(primaryWeapon.WeaponClass, settings))
                    continue;

                if (IsEmptyConsumableRanged(primaryWeapon, weapon, settings))
                    continue;

                try
                {
                    affectedAgent.DropItem(slot, primaryWeapon.WeaponClass);
                    MissionDiagnosticsPatch.CountModDrop();
                }
                catch (Exception e)
                {
                    LogDropFailure(e);
                }
            }
        }
        
        internal static void LogDropFailure(Exception e)
        {
            if (_dropFailureLogged)
                return;

            _dropFailureLogged = true;
            Debug.Print($"[TroopsDropAllWeapons] DropItem failed: {e}", 0, Debug.DebugColor.Red);
        }

        internal static bool IsEmptyConsumableRanged(WeaponComponentData primaryWeapon, MissionWeapon weapon, MCMSettings? settings)
        {
            if (settings == null || !settings.DoNotDropEmptyAmmo)
                return false;

            if (!primaryWeapon.IsConsumable || !primaryWeapon.IsRangedWeapon)
                return false;

            return weapon.Amount == 0;
        }

        internal static bool ShouldDrop(WeaponClass weaponClass, MCMSettings? settings)
        {
            if (settings == null)
                return true;

            return weaponClass switch
            {
                WeaponClass.Arrow or
                WeaponClass.Bolt or
                WeaponClass.SlingStone or
                WeaponClass.Cartridge => settings.DropArrowsBolts,

                WeaponClass.ThrowingAxe or
                WeaponClass.ThrowingKnife or
                WeaponClass.Javelin or
                WeaponClass.Stone => settings.DropThrowingWeapons,

                WeaponClass.Bow or
                WeaponClass.Crossbow or
                WeaponClass.Sling or
                WeaponClass.Pistol or
                WeaponClass.Musket => settings.DropBowsCrossbows,

                WeaponClass.SmallShield or
                WeaponClass.LargeShield => settings.DropShields,

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
                WeaponClass.LowGripPolearm => settings.DropMeleeWeapons,

                _ => false,
            };
        }
        
        internal static bool IsArenaMission(Mission? mission) =>
            mission != null
            && (mission.HasMissionBehavior<ArenaPracticeFightMissionController>()
                || mission.HasMissionBehavior<TournamentFightMissionController>());
    }
}