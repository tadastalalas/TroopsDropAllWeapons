using TaleWorlds.Localization;

namespace TroopsDropAllWeapons.Core;

internal static class Texts
{
    #region Settings
    public static readonly TextObject ModName = new(Ids.ModName);
    #endregion

    internal static class Ids
    {
        #region Settings
        public const string ModName = "{=TDAW_1a2B3c4}Troops Drop All Weapons";
        public const string GroupRangedAmmo = "{=TDAW_5d6E7f8}Ranged Ammo";
        public const string GroupRangedWeapons = "{=TDAW_9g0H1i2}Ranged Weapons";
        public const string GroupMeleeWeapons = "{=TDAW_3j4K5l6}Melee Weapons";
        public const string GroupShields = "{=TDAW_7m8N9o0}Shields";
        public const string DropArrowsBolts = "{=TDAW_p1Q2r3S}Drop Arrows/Bolts";
        public const string DropArrowsBoltsHint = "{=TDAW_t4U5v6W}Drop arrows, bolts, and sling stones on death. [Default: enabled]";
        public const string DropThrowingWeapons = "{=TDAW_x7Y8z9A}Drop Throwing Weapons";
        public const string DropThrowingWeaponsHint = "{=TDAW_b1C2d3E}Drop throwing axes, throwing knives, javelins, and stones on death. [Default: enabled]";
        public const string DoNotDropEmptyAmmo = "{=TDAW_f4G5h6I}Do Not Drop Empty Quivers/Pouches";
        public const string DoNotDropEmptyAmmoHint = "{=TDAW_j7K8l9M}If an arrow/bolt quiver or throwing weapon pouch is empty, skip dropping it. [Default: enabled]";
        public const string ApplyEmptyAmmoCheckToVanilla = "{=TDAW_n1O2p3Q}Also Apply To Vanilla Drops";
        public const string ApplyEmptyAmmoCheckToVanillaHint = "{=TDAW_r4S5t6U}Intercept vanilla empty-quiver drops and remove them from equipment instead of spawning them on the ground. Improves Mission performance. [Default: enabled]";
        public const string DropBowsCrossbows = "{=TDAW_v7W8x9Y}Drop Bows/Crossbows";
        public const string DropBowsCrossbowsHint = "{=TDAW_z1A2b3C}Drop bows, crossbows, and slings on death. [Default: enabled]";
        public const string DropMeleeWeapons = "{=TDAW_d4E5f6G}Drop Melee Weapons";
        public const string DropMeleeWeaponsHint = "{=TDAW_h7I8j9K}Drop swords, axes, maces, polearms, daggers, and other melee weapons on death. [Default: enabled]";
        public const string DropShields = "{=TDAW_l1M2n3O}Drop Shields";
        public const string DropShieldsHint = "{=TDAW_p4Q5r6S}Drop shields on death. [Default: enabled]";
        public const string GroupScope = "{=TDAW_a7R8e9N}Scope";
        public const string ArenaOnly = "{=TDAW_a1R2e3N}Arena Only";
        public const string ArenaOnlyHint = "{=TDAW_a4R5e6N}Drop weapons only in arena practice and tournament missions. [Default: disabled]";
        public const string GroupTechnical = "{=TDAW_Vk3qZ8w}Technical settings";
        public const string LoggingEnabled = "{=TDAW_mT6xR2j}Logging for debugging";
        public const string LoggingEnabledHint = "{=TDAW_H9cLp4u}Logging for debugging (English only). [Default: disabled]";
        #endregion
    }
}
