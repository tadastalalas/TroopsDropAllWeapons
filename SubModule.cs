using HarmonyLib;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace TroopsDropAllWeapons
{
    public class SubModule : MBSubModuleBase
    {
        private Harmony? _harmony;

        public override void OnGameInitializationFinished(Game game)
        {
            base.OnGameInitializationFinished(game);
            _harmony = new Harmony("TroopsDropAllWeapons");
            _harmony.PatchAll();
            ArtemCoreCompatibilityPatch.TryApply(_harmony);
        }

        public override void OnGameEnd(Game game)
        {
            base.OnGameEnd(game);
            _harmony?.UnpatchAll("TroopsDropAllWeapons");
            ArtemCoreCompatibilityPatch.TryUnpatch(_harmony);
        }
    }
}