using HarmonyLib;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace TroopsDropAllWeapons
{
    [HarmonyPatch(typeof(Mission), "OnAgentRemoved")]
    internal static class DropWeaponPatch
    {
        private static void Postfix(Agent affectedAgent)
        {
            if (!affectedAgent.IsHuman)
                return;

            if (affectedAgent.AgentVisuals == null)
                return;

            var settings = MCMSettings.Instance;

            for (EquipmentIndex slot = EquipmentIndex.WeaponItemBeginSlot;
                 slot < EquipmentIndex.ExtraWeaponSlot;
                 slot++)
            {
                if (affectedAgent.Equipment[slot].IsEmpty)
                    continue;

                var primaryWeapon = affectedAgent.Equipment[slot].Item?.PrimaryWeapon;
                if (primaryWeapon == null)
                    continue;

                if (!ShouldDrop(primaryWeapon.WeaponClass, settings))
                    continue;

                try
                {
                    affectedAgent.DropItem(slot, primaryWeapon.WeaponClass);
                }
                catch
                {
                    
                }
            }
        }

        private static bool ShouldDrop(WeaponClass weaponClass, MCMSettings? settings)
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
    }
}