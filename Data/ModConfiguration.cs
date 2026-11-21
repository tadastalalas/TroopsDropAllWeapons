namespace TroopsDropAllWeapons.Data;

// Shape of ModConfiguration.xml and the only place where defaults live (MCMSettings uses them too).
// One section per MCM group, same names, same order.
public class ModConfiguration
{
    public const bool DropArrowsBoltsDefault = true;
    public const bool DropThrowingWeaponsDefault = true;
    public const bool DoNotDropEmptyAmmoDefault = true;
    public const bool ApplyEmptyAmmoCheckToVanillaDefault = true;
    public const bool DropBowsCrossbowsDefault = true;
    public const bool DropMeleeWeaponsDefault = true;
    public const bool DropShieldsDefault = true;
    public const bool ArenaOnlyDefault = false;
    public const bool LoggingEnabledDefault = false;

    public RangedAmmoSection RangedAmmo { get; set; } = new();

    public RangedWeaponsSection RangedWeapons { get; set; } = new();

    public MeleeWeaponsSection MeleeWeapons { get; set; } = new();

    public ShieldsSection Shields { get; set; } = new();

    public ScopeSection Scope { get; set; } = new();

    // Always the last section.
    public TechnicalSettingsSection TechnicalSettings { get; set; } = new();

    public class RangedAmmoSection
    {
        public bool DropArrowsBolts { get; set; } = DropArrowsBoltsDefault;
        public bool DropThrowingWeapons { get; set; } = DropThrowingWeaponsDefault;
        public bool DoNotDropEmptyAmmo { get; set; } = DoNotDropEmptyAmmoDefault;
        public bool ApplyEmptyAmmoCheckToVanilla { get; set; } = ApplyEmptyAmmoCheckToVanillaDefault;
    }

    public class RangedWeaponsSection
    {
        public bool DropBowsCrossbows { get; set; } = DropBowsCrossbowsDefault;
    }

    public class MeleeWeaponsSection
    {
        public bool DropMeleeWeapons { get; set; } = DropMeleeWeaponsDefault;
    }

    public class ShieldsSection
    {
        public bool DropShields { get; set; } = DropShieldsDefault;
    }

    public class ScopeSection
    {
        public bool ArenaOnly { get; set; } = ArenaOnlyDefault;
    }

    public class TechnicalSettingsSection
    {
        // Always the last entry.
        public bool LoggingEnabled { get; set; } = LoggingEnabledDefault;
    }
}
