using System;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using TroopsDropAllWeapons.Data;
using TroopsDropAllWeapons.Diagnostics;

namespace TroopsDropAllWeapons.Core;

// The only place mod code reads settings from. MCM loaded -> MCM. MCM not loaded -> ModConfiguration.xml.
internal static class ModSettings
{
    private const string McmAssemblyName = "MCMv5";

    private static ModConfiguration _xml = new();
    private static XmlSerializer? _serializer;
    private static bool _useMcm;

    internal static bool DropArrowsBolts => _useMcm ? McmDropArrowsBolts() : _xml.RangedAmmo.DropArrowsBolts;

    internal static bool DropThrowingWeapons => _useMcm ? McmDropThrowingWeapons() : _xml.RangedAmmo.DropThrowingWeapons;

    internal static bool DoNotDropEmptyAmmo => _useMcm ? McmDoNotDropEmptyAmmo() : _xml.RangedAmmo.DoNotDropEmptyAmmo;

    internal static bool ApplyEmptyAmmoCheckToVanilla => _useMcm ? McmApplyEmptyAmmoCheckToVanilla() : _xml.RangedAmmo.ApplyEmptyAmmoCheckToVanilla;

    internal static bool DropBowsCrossbows => _useMcm ? McmDropBowsCrossbows() : _xml.RangedWeapons.DropBowsCrossbows;

    internal static bool DropMeleeWeapons => _useMcm ? McmDropMeleeWeapons() : _xml.MeleeWeapons.DropMeleeWeapons;

    internal static bool DropShields => _useMcm ? McmDropShields() : _xml.Shields.DropShields;

    internal static bool ArenaOnly => _useMcm ? McmArenaOnly() : _xml.Scope.ArenaOnly;

    internal static bool LoggingEnabled => _useMcm ? McmLoggingEnabled() : _xml.TechnicalSettings.LoggingEnabled;

    private static XmlSerializer Serializer => _serializer ??= new XmlSerializer(typeof(ModConfiguration));

    // Called on every game start (campaign and custom battle).
    internal static void Reload()
    {
        _useMcm = false;
        _xml = new ModConfiguration();

        if (IsMcmLoaded())
        {
            if (McmHasSettings())
            {
                _useMcm = true;
                return;
            }

            TdawLog.Warn("MCM is loaded but returned no settings for this mod. Using " + ModInfo.ConfigurationFileName + " instead.");
        }

        _xml = LoadFromFile();
    }

    private static bool IsMcmLoaded()
    {
        Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
        for (int i = 0; i < assemblies.Length; i++)
        {
            if (assemblies[i].GetName().Name == McmAssemblyName) return true;
        }

        return false;
    }

    // Every method that touches MCMSettings must be NoInlining and may only be called when MCM is loaded.
    // Otherwise the game tries to load MCM's classes and crashes when MCM is not installed.
    [MethodImpl(MethodImplOptions.NoInlining)]
    private static bool McmHasSettings() => MCMSettings.Instance != null;

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static bool McmDropArrowsBolts() => MCMSettings.Instance?.DropArrowsBolts ?? ModConfiguration.DropArrowsBoltsDefault;

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static bool McmDropThrowingWeapons() => MCMSettings.Instance?.DropThrowingWeapons ?? ModConfiguration.DropThrowingWeaponsDefault;

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static bool McmDoNotDropEmptyAmmo() => MCMSettings.Instance?.DoNotDropEmptyAmmo ?? ModConfiguration.DoNotDropEmptyAmmoDefault;

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static bool McmApplyEmptyAmmoCheckToVanilla() => MCMSettings.Instance?.ApplyEmptyAmmoCheckToVanilla ?? ModConfiguration.ApplyEmptyAmmoCheckToVanillaDefault;

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static bool McmDropBowsCrossbows() => MCMSettings.Instance?.DropBowsCrossbows ?? ModConfiguration.DropBowsCrossbowsDefault;

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static bool McmDropMeleeWeapons() => MCMSettings.Instance?.DropMeleeWeapons ?? ModConfiguration.DropMeleeWeaponsDefault;

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static bool McmDropShields() => MCMSettings.Instance?.DropShields ?? ModConfiguration.DropShieldsDefault;

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static bool McmArenaOnly() => MCMSettings.Instance?.ArenaOnly ?? ModConfiguration.ArenaOnlyDefault;

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static bool McmLoggingEnabled() => MCMSettings.Instance?.LoggingEnabled ?? ModConfiguration.LoggingEnabledDefault;

    // Every setting is a bool: nothing to clamp.
    private static ModConfiguration LoadFromFile()
    {
        string path = Path.Combine(ModInfo.ModuleRoot, ModInfo.ConfigurationFileName);
        ModConfiguration configuration = new();

        if (!File.Exists(path))
        {
            TdawLog.Warn(ModInfo.ConfigurationFileName + " was missing. Created it with the default settings.");
            TryWrite(path, configuration);
            return configuration;
        }

        try
        {
            configuration = Read(path);
            if (!HasEverySetting(path, configuration))
            {
                TdawLog.Warn(ModInfo.ConfigurationFileName + " had missing settings. Added them with their default values.");
                TryWrite(path, configuration);
            }
        }
        catch (Exception ex) when (ex is InvalidOperationException or XmlException)
        {
            TdawLog.Warn(ModInfo.ConfigurationFileName + " was broken and was replaced with the default settings. Reason: " + (ex.InnerException?.Message ?? ex.Message));
            configuration = new ModConfiguration();
            TryWrite(path, configuration);
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            TdawLog.Warn(ModInfo.ConfigurationFileName + " could not be opened. Using the default settings. Reason: " + ex.Message);
            configuration = new ModConfiguration();
        }

        return configuration;
    }

    private static ModConfiguration Read(string path)
    {
        using FileStream stream = File.OpenRead(path);
        return Serializer.Deserialize(stream) as ModConfiguration ?? new ModConfiguration();
    }

    private static void TryWrite(string path, ModConfiguration configuration)
    {
        try
        {
            XmlWriterSettings writerSettings = new() { Indent = true, Encoding = Encoding.UTF8 };
            using XmlWriter writer = XmlWriter.Create(path, writerSettings);
            Serializer.Serialize(writer, configuration, NoNamespaces());
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            TdawLog.Warn(ModInfo.ConfigurationFileName + " could not be written. The settings are kept in memory for this session. Reason: " + ex.Message);
        }
    }

    private static bool HasEverySetting(string path, ModConfiguration configuration)
    {
        XDocument complete = new();
        using (XmlWriter writer = complete.CreateWriter())
        {
            Serializer.Serialize(writer, configuration, NoNamespaces());
        }

        XElement? fileRoot = XDocument.Load(path).Root;
        return fileRoot != null && complete.Root != null && ContainsEveryElement(fileRoot, complete.Root);
    }

    private static bool ContainsEveryElement(XElement actual, XElement expected)
    {
        foreach (XElement expectedChild in expected.Elements())
        {
            XElement? actualChild = actual.Element(expectedChild.Name);
            if (actualChild == null || !ContainsEveryElement(actualChild, expectedChild)) return false;
        }

        return true;
    }

    private static XmlSerializerNamespaces NoNamespaces()
    {
        XmlSerializerNamespaces namespaces = new();
        namespaces.Add(string.Empty, string.Empty);
        return namespaces;
    }
}
