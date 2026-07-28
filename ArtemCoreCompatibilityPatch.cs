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

            for (EquipmentIndex slot = EquipmentIndex.WeaponItemBeginSlot;
                 slot < EquipmentIndex.ExtraWeaponSlot;
                 slot++)
            {
                var weapon = agent.Equipment[slot];
                if (weapon.IsEmpty)
                    continue;

                var primaryWeapon = weapon.Item?.PrimaryWeapon;
                if (primaryWeapon == null)
                    continue;

                if (!DropWeaponPatch.ShouldDrop(primaryWeapon.WeaponClass, settings))
                    continue;

                if (DropWeaponPatch.IsEmptyConsumableRanged(primaryWeapon, weapon, settings))
                {
                    agent.RemoveEquippedWeapon(slot);
                    continue;
                }

                try
                {
                    agent.DropItem(slot, primaryWeapon.WeaponClass);
                }
                catch
                {
                }
            }

            return false;
        }
    }
}