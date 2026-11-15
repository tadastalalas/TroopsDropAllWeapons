using MCM.Abstractions.Attributes;
using MCM.Abstractions.Attributes.v2;
using MCM.Abstractions.Base.Global;
using TroopsDropAllWeapons.Core;

namespace TroopsDropAllWeapons.Data;

// Used only when MCM is loaded. The only file that may use MCM types.
internal class MCMSettings : AttributeGlobalSettings<MCMSettings>
{
    // Released Id: changing it resets every player's saved settings.
    private const string SettingsId = "TroopsDropAllWeaponsSettings";
    private const string SaveFormat = "json2";

    public override string Id => SettingsId;

    public override string DisplayName => Texts.ModName.ToString();

    public override string FolderName => ModInfo.ModuleId;

    public override string FormatType => SaveFormat;

    [SettingPropertyBool(Texts.Ids.DropArrowsBolts, Order = 0, RequireRestart = false, HintText = Texts.Ids.DropArrowsBoltsHint)]
    [SettingPropertyGroup(Texts.Ids.GroupRangedAmmo, GroupOrder = 0)]
    public bool DropArrowsBolts { get; set; } = ModConfiguration.DropArrowsBoltsDefault;

    [SettingPropertyBool(Texts.Ids.DropThrowingWeapons, Order = 1, RequireRestart = false, HintText = Texts.Ids.DropThrowingWeaponsHint)]
    [SettingPropertyGroup(Texts.Ids.GroupRangedAmmo, GroupOrder = 0)]
    public bool DropThrowingWeapons { get; set; } = ModConfiguration.DropThrowingWeaponsDefault;

    [SettingPropertyBool(Texts.Ids.DoNotDropEmptyAmmo, Order = 2, RequireRestart = false, HintText = Texts.Ids.DoNotDropEmptyAmmoHint)]
    [SettingPropertyGroup(Texts.Ids.GroupRangedAmmo, GroupOrder = 0)]
    public bool DoNotDropEmptyAmmo { get; set; } = ModConfiguration.DoNotDropEmptyAmmoDefault;

    [SettingPropertyBool(Texts.Ids.ApplyEmptyAmmoCheckToVanilla, Order = 3, RequireRestart = false, HintText = Texts.Ids.ApplyEmptyAmmoCheckToVanillaHint)]
    [SettingPropertyGroup(Texts.Ids.GroupRangedAmmo, GroupOrder = 0)]
    public bool ApplyEmptyAmmoCheckToVanilla { get; set; } = ModConfiguration.ApplyEmptyAmmoCheckToVanillaDefault;

    [SettingPropertyBool(Texts.Ids.DropBowsCrossbows, Order = 0, RequireRestart = false, HintText = Texts.Ids.DropBowsCrossbowsHint)]
    [SettingPropertyGroup(Texts.Ids.GroupRangedWeapons, GroupOrder = 1)]
    public bool DropBowsCrossbows { get; set; } = ModConfiguration.DropBowsCrossbowsDefault;

    [SettingPropertyBool(Texts.Ids.DropMeleeWeapons, Order = 0, RequireRestart = false, HintText = Texts.Ids.DropMeleeWeaponsHint)]
    [SettingPropertyGroup(Texts.Ids.GroupMeleeWeapons, GroupOrder = 2)]
    public bool DropMeleeWeapons { get; set; } = ModConfiguration.DropMeleeWeaponsDefault;

    [SettingPropertyBool(Texts.Ids.DropShields, Order = 0, RequireRestart = false, HintText = Texts.Ids.DropShieldsHint)]
    [SettingPropertyGroup(Texts.Ids.GroupShields, GroupOrder = 3)]
    public bool DropShields { get; set; } = ModConfiguration.DropShieldsDefault;

    [SettingPropertyBool(Texts.Ids.ArenaOnly, Order = 0, RequireRestart = false, HintText = Texts.Ids.ArenaOnlyHint)]
    [SettingPropertyGroup(Texts.Ids.GroupScope, GroupOrder = 4)]
    public bool ArenaOnly { get; set; } = ModConfiguration.ArenaOnlyDefault;

    // "Technical settings" is always the last group and "Logging for debugging" its last entry.
    [SettingPropertyBool(Texts.Ids.LoggingEnabled, Order = 0, RequireRestart = false, HintText = Texts.Ids.LoggingEnabledHint)]
    [SettingPropertyGroup(Texts.Ids.GroupTechnical, GroupOrder = 5)]
    public bool LoggingEnabled { get; set; } = ModConfiguration.LoggingEnabledDefault;
}
