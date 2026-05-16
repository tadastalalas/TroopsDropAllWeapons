using System;
using System.Reflection;
using HarmonyLib;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace TroopsDropAllWeapons
{
    internal static class ArtemCoreCompatibilityPatch
    {
        private const string ArtemCoreAssemblyQualifiedTypeName = "ArtemCore.ArtemCore, ArtemCore";

        private static MethodInfo _patchedDropAllWeapons;

        internal static void TryApply(Harmony harmony)
        {
            var artemCoreType = Type.GetType(ArtemCoreAssemblyQualifiedTypeName);
            if (artemCoreType == null)
                return;

            var dropAllWeapons = artemCoreType.GetMethod(
                "DropAllWeapons",
                BindingFlags.Public | BindingFlags.Static,
                null,
                new[] { typeof(Agent) },
                null);

            if (dropAllWeapons == null)
                return;

            var prefix = typeof(ArtemCoreCompatibilityPatch).GetMethod(
                nameof(DropAllWeaponsPrefix),
                BindingFlags.Static | BindingFlags.NonPublic);

            harmony.Patch(dropAllWeapons, new HarmonyMethod(prefix));
            _patchedDropAllWeapons = dropAllWeapons;
        }

        internal static void TryUnpatch(Harmony harmony)
        {
            if (_patchedDropAllWeapons == null)
                return;

            var prefix = typeof(ArtemCoreCompatibilityPatch).GetMethod(
                nameof(DropAllWeaponsPrefix),
                BindingFlags.Static | BindingFlags.NonPublic);

            harmony.Unpatch(_patchedDropAllWeapons, prefix);
            _patchedDropAllWeapons = null;
        }

        private static bool DropAllWeaponsPrefix(Agent agent)
        {
            if (agent == null || !agent.IsActive() || !agent.ActionSet.IsValid)
                return true;

            var settings = MCMSettings.Instance;
            if (settings == null || !settings.DoNotDropEmptyAmmo)
                return true;

            for (EquipmentIndex slot = EquipmentIndex.ExtraWeaponSlot;
                 slot >= EquipmentIndex.WeaponItemBeginSlot;
                 slot--)
            {
                var weapon = agent.Equipment[slot];
                if (weapon.IsEmpty)
                    continue;

                if (IsEmptyConsumableRangedSlot(weapon))
                {
                    agent.RemoveEquippedWeapon(slot);
                    continue;
                }

                try
                {
                    agent.DropItem(slot, WeaponClass.Undefined);
                }
                catch { }
            }

            return false;
        }

        private static bool IsEmptyConsumableRangedSlot(MissionWeapon weapon)
        {
            var primary = weapon.Item?.PrimaryWeapon;
            if (primary == null)
                return false;

            return primary.IsConsumable && primary.IsRangedWeapon && weapon.Amount == 0;
        }
    }
}