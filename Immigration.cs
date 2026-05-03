using HarmonyLib;
using UnityEngine;

namespace SuperPOD
{
    internal class Immigration_sant1ago
    {
        [HarmonyPatch(typeof(Immigration), "OnPrefabInit")]
        private class Immigration_OnPrefabInit
        {
            private static void Prefix(Immigration __instance)
            {
                var opts = SuperPODOptions.Instance;
                float spawn = Mathf.Max((float)opts.TimeBeforeSpawn, 0f);

                __instance.spawnInterval[0] = spawn;
                __instance.spawnInterval[1] = spawn;
            }
        }

        [HarmonyPatch(typeof(Immigration), "Sim200ms")]
        private class Immigration_Sim200ms
        {
            private static void Prefix(Immigration __instance)
            {
                var opts = SuperPODOptions.Instance;
                float spawn = Mathf.Max((float)opts.TimeBeforeSpawn, 0f);

                if (__instance.spawnInterval[1] != spawn)
                {
                    __instance.spawnInterval[1] = spawn;
                    __instance.timeBeforeSpawn = spawn;
                }
            }
        }
    }
}
