namespace TroopsDropAllWeapons.Core;

// Gameplay numbers (not player settings). Values = the mod's released behavior.
internal static class Balance
{
    // Highest runtime mission object id the engine hands out (vanilla Mission.GetFreeRuntimeMissionObjectId). Never change.
    internal const int RuntimeIdCap = 8191;

    // A freed runtime id is reused only after this many seconds, unless no new id is left below RuntimeIdCap.
    internal const float RuntimeIdReuseCooldownSeconds = 30f;

    // Starting capacity of the freed runtime id queue.
    internal const int FreedRuntimeIdQueueCapacity = 1024;

    // While "Logging for debugging" is on, the mission counters are written to the log this often.
    internal const float DiagnosticsIntervalSeconds = 10f;

    // Runtime ids at RuntimeIdCap and fewer freed ids than this: the log reports the id pool as exhausted.
    internal const int ExhaustedFreedIdThreshold = 64;
}
