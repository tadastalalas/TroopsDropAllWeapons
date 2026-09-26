using MCM.Abstractions.Attributes;
using MCM.Abstractions.Attributes.v2;
using MCM.Abstractions.Base.Global;
using TaleWorlds.Localization;

namespace TroopsDropAllWeapons
{
    internal class MCMSettings : AttributeGlobalSettings<MCMSettings>
    {
        public override string Id
        { get { return "TroopsDropAllWeaponsSettings"; } }

        public override string DisplayName
        { get { return new TextObject("{=TDAW_1a2B3c4}Troops Drop All Weapons").ToString(); } }

        public override string FolderName
        { get { return "TroopsDropAllWeapons"; } }

        public override string FormatType
        { get { return "json2"; } }

        [SettingPropertyBool("{=TDAW_p1Q2r3S}Drop Arrows/Bolts", Order = 0, RequireRestart = false, HintText = "{=TDAW_t4U5v6W}Drop arrows, bolts, and sling stones on death. [Default: enabled]")]
        [SettingPropertyGroup("{=TDAW_5d6E7f8}Ranged Ammo", GroupOrder = 0)]
        public bool DropArrowsBolts { get; set; } = true;

        [SettingPropertyBool("{=TDAW_x7Y8z9A}Drop Throwing Weapons", Order = 1, RequireRestart = false, HintText = "{=TDAW_b1C2d3E}Drop throwing axes, throwing knives, javelins, and stones on death. [Default: enabled]")]
        [SettingPropertyGroup("{=TDAW_5d6E7f8}Ranged Ammo", GroupOrder = 0)]
        public bool DropThrowingWeapons { get; set; } = true;

        [SettingPropertyBool("{=TDAW_f4G5h6I}Do Not Drop Empty Quivers/Pouches", Order = 2, RequireRestart = false, HintText = "{=TDAW_j7K8l9M}If an arrow/bolt quiver or throwing weapon pouch is empty, skip dropping it. [Default: enabled]")]
        [SettingPropertyGroup("{=TDAW_5d6E7f8}Ranged Ammo", GroupOrder = 0)]
        public bool DoNotDropEmptyAmmo { get; set; } = true;

        [SettingPropertyBool("{=TDAW_n1O2p3Q}Also Apply To Vanilla Drops", Order = 3, RequireRestart = false, HintText = "{=TDAW_r4S5t6U}Intercept vanilla empty-quiver drops and remove them from equipment instead of spawning them on the ground. Improves Mission performance. [Default: enabled]")]
        [SettingPropertyGroup("{=TDAW_5d6E7f8}Ranged Ammo", GroupOrder = 0)]
        public bool ApplyEmptyAmmoCheckToVanilla { get; set; } = true;

        [SettingPropertyBool("{=TDAW_v7W8x9Y}Drop Bows/Crossbows", Order = 0, RequireRestart = false, HintText = "{=TDAW_z1A2b3C}Drop bows, crossbows, and slings on death. [Default: disabled]")]
        [SettingPropertyGroup("{=TDAW_9g0H1i2}Ranged Weapons", GroupOrder = 1)]
        public bool DropBowsCrossbows { get; set; } = false;

        [SettingPropertyBool("{=TDAW_d4E5f6G}Drop Melee Weapons", Order = 0, RequireRestart = false, HintText = "{=TDAW_h7I8j9K}Drop swords, axes, maces, polearms, daggers, and other melee weapons on death. [Default: disabled]")]
        [SettingPropertyGroup("{=TDAW_3j4K5l6}Melee Weapons", GroupOrder = 2)]
        public bool DropMeleeWeapons { get; set; } = false;

        [SettingPropertyBool("{=TDAW_l1M2n3O}Drop Shields", Order = 0, RequireRestart = false, HintText = "{=TDAW_p4Q5r6S}Drop shields on death. [Default: disabled]")]
        [SettingPropertyGroup("{=TDAW_7m8N9o0}Shields", GroupOrder = 3)]
        public bool DropShields { get; set; } = false;
        
        [SettingPropertyBool("{=TDAW_a1R2e3N}Arena Only", Order = 0, RequireRestart = false, HintText = "{=TDAW_a4R5e6N}Drop weapons only in arena practice and tournament missions. [Default: disabled]")]
        [SettingPropertyGroup("{=TDAW_a7R8e9N}Scope", GroupOrder = 4)]
        public bool ArenaOnly { get; set; } = false;
        
        [SettingPropertyBool("{=TDAW_q1W2e3R}Enable Diagnostics Logging", Order = 0, RequireRestart = false, HintText = "{=TDAW_t5Y6u7I}Log mission item/object counters every 10s to Modules/TroopsDropAllWeapons/TDAW_diagnostics.log and on screen. [Default: disabled]")]
        [SettingPropertyGroup("{=TDAW_o8P9a0S}Debug", GroupOrder = 5)]
        public bool EnableDiagnostics { get; set; } = false;
    }
}