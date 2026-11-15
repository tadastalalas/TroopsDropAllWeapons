using HarmonyLib;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TroopsDropAllWeapons.Compat;
using TroopsDropAllWeapons.Core;

namespace TroopsDropAllWeapons;

public class SubModule : MBSubModuleBase
{
    private Harmony? _harmony;

    // Every game (campaign and custom battle): the mod works in any mission.
    protected override void OnGameStart(Game game, IGameStarter gameStarterObject)
    {
        base.OnGameStart(game, gameStarterObject);
        ModSettings.Reload();
    }

    public override void OnGameInitializationFinished(Game game)
    {
        base.OnGameInitializationFinished(game);
        _harmony = new Harmony(ModInfo.HarmonyId);
        // Assembly passed explicitly: the no-argument PatchAll() takes it from the call stack, which under the
        // module loader can be the loader's assembly, so no patch would be applied.
        _harmony.PatchAll(typeof(SubModule).Assembly);
        ArtemCoreCompat.TryApply(_harmony);
    }

    public override void OnGameEnd(Game game)
    {
        base.OnGameEnd(game);
        _harmony?.UnpatchAll(ModInfo.HarmonyId);
        ArtemCoreCompat.TryUnpatch(_harmony);
    }
}
