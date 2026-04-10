using HarmonyLib;
using TaleWorlds.MountAndBlade;

namespace TroopsDropAllWeapons
{
    public class SubModule : MBSubModuleBase
    {
        private Harmony _harmony;

        protected override void OnSubModuleLoad()
        {
            base.OnSubModuleLoad();
            _harmony = new Harmony("TroopsDropAllWeapons");
            _harmony.PatchAll();
        }

        protected override void OnSubModuleUnloaded()
        {
            base.OnSubModuleUnloaded();
            _harmony?.UnpatchAll("TroopsDropAllWeapons");
        }
    }
}