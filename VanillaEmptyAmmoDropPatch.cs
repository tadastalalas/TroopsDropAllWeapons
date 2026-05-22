using HarmonyLib;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace TroopsDropAllWeapons
{
    [HarmonyPatch(typeof(Agent), nameof(Agent.DropItem))]
    internal static class VanillaEmptyAmmoDropPatch
    {
        private static bool Prefix(Agent __instance, EquipmentIndex itemIndex, WeaponClass pickedUpItemType)
        {
            var settings = MCMSettings.Instance;
            if (settings == null || !settings.ApplyEmptyAmmoCheckToVanilla)
                return true;

            if (!IsEmptyConsumableRangedSlot(__instance, itemIndex))
                return true;

            __instance.RemoveEquippedWeapon(itemIndex);
            return false;
        }

        private static bool IsEmptyConsumableRangedSlot(Agent agent, EquipmentIndex slot)
        {
            if (slot < EquipmentIndex.WeaponItemBeginSlot || slot >= EquipmentIndex.ExtraWeaponSlot)
                return false;

            var weapon = agent.Equipment[slot];
            if (weapon.IsEmpty)
                return false;

            var primary = weapon.Item?.PrimaryWeapon;
            if (primary == null)
                return false;

            if (!primary.IsConsumable || !primary.IsRangedWeapon)
                return false;

            return weapon.Amount == 0;
        }
    }
}