using HarmonyLib;

namespace SuperPOD
{
    internal class Immigration_sant1ago
    {
        [HarmonyPatch(typeof(Immigration), "OnPrefabInit")]
        private class Immigration_OnPrefabInit
        {
            private static void Prefix(Immigration __instance)
            {
                var config = ConfigHelper<ConfigData>.GetConfig("config.ini");
                if (config.TimeBeforeSpawn < 0f)
                    config.TimeBeforeSpawn = 0f;

                __instance.spawnInterval[0] = config.TimeBeforeSpawn;
                __instance.spawnInterval[1] = config.TimeBeforeSpawn;
            }
        }

        [HarmonyPatch(typeof(Immigration), "Sim200ms")]
        private class Immigration_Sim200ms
        {
            private static void Prefix(Immigration __instance)
            {
                var config = ConfigHelper<ConfigData>.GetConfig("config.ini");
                if (config.TimeBeforeSpawn < 0f)
                    config.TimeBeforeSpawn = 0f;

                if (__instance.spawnInterval[1] != config.TimeBeforeSpawn)
                {
                    __instance.spawnInterval[1] = config.TimeBeforeSpawn;
                    __instance.timeBeforeSpawn = config.TimeBeforeSpawn;
                }
            }
        }
    }
}
