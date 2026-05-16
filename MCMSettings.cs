using MCM.Abstractions.Attributes;
using MCM.Abstractions.Attributes.v2;
using MCM.Abstractions.Base.Global;

namespace TroopsDropAllWeapons
{
    internal class MCMSettings : AttributeGlobalSettings<MCMSettings>
    {
        public override string Id
        { get { return "TroopsDropAllWeaponsSettings"; } }

        public override string DisplayName
        { get { return "Troops Drop All Weapons"; } }

        public override string FolderName
        { get { return "TroopsDropAllWeapons"; } }

        public override string FormatType
        { get { return "json2"; } }

        [SettingPropertyBool("Drop Arrows/Bolts", Order = 0, RequireRestart = false, HintText = "Drop arrows, bolts, and sling stones on death. [Default: enabled]")]
        [SettingPropertyGroup("Ranged Ammo", GroupOrder = 0)]
        public bool DropArrowsBolts { get; set; } = true;

        [SettingPropertyBool("Drop Throwing Weapons", Order = 1, RequireRestart = false, HintText = "Drop throwing axes, throwing knives, javelins, and stones on death. [Default: enabled]")]
        [SettingPropertyGroup("Ranged Ammo", GroupOrder = 0)]
        public bool DropThrowingWeapons { get; set; } = true;

        [SettingPropertyBool("Do Not Drop Empty Quivers/Pouches", Order = 2, RequireRestart = false, HintText = "If an arrow/bolt quiver or throwing weapon pouch is empty, skip dropping it. [Default: enabled]")]
        [SettingPropertyGroup("Ranged Ammo", GroupOrder = 0)]
        public bool DoNotDropEmptyAmmo { get; set; } = true;

        [SettingPropertyBool("Also Apply To Vanilla Drops", Order = 3, RequireRestart = false, HintText = "Intercept vanilla empty-quiver drops and remove them from equipment instead of spawning them on the ground. Improves Mission performance. [Default: enabled]")]
        [SettingPropertyGroup("Ranged Ammo", GroupOrder = 0)]
        public bool ApplyEmptyAmmoCheckToVanilla { get; set; } = true;

        [SettingPropertyBool("Drop Bows/Crossbows", Order = 0, RequireRestart = false, HintText = "Drop bows, crossbows, and slings on death. [Default: disabled]")]
        [SettingPropertyGroup("Ranged Weapons", GroupOrder = 1)]
        public bool DropBowsCrossbows { get; set; } = false;

        [SettingPropertyBool("Drop Melee Weapons", Order = 0, RequireRestart = false, HintText = "Drop swords, axes, maces, polearms, daggers, and other melee weapons on death. [Default: disabled]")]
        [SettingPropertyGroup("Melee Weapons", GroupOrder = 2)]
        public bool DropMeleeWeapons { get; set; } = false;

        [SettingPropertyBool("Drop Shields", Order = 0, RequireRestart = false, HintText = "Drop shields on death. [Default: disabled]")]
        [SettingPropertyGroup("Shields", GroupOrder = 3)]
        public bool DropShields { get; set; } = false;
    }
}