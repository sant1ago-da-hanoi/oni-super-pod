using System.Collections.Generic;
using HarmonyLib;
using STRINGS;
using UnityEngine;

namespace SuperPOD
{
    internal class ImmigrantScreen_sant1ago
    {
        [HarmonyPatch(typeof(ImmigrantScreen), "OnRejectAll")]
        private class ImmigrantScreen_OnRejectAll
        {
            private static float markTime;

            private static bool Prefix(ImmigrantScreen __instance)
            {

                // Debounce — prevent rapid re-rolls
                if (Time.realtimeSinceStartup - markTime < 0.666f)
                    return false;

                var traverse = Traverse.Create(__instance);
                var containers = traverse.Field("containers").GetValue<List<ITelepadDeliverableContainer>>();

                containers.ForEach(c => Object.Destroy(c.GetGameObject()));
                containers.Clear();

                traverse.Method("InitializeContainers").GetValue();
                markTime = Time.realtimeSinceStartup;
                return false;
            }
        }

        [HarmonyPatch(typeof(Localization), "Initialize")]
        private static class Localization_Initialize
        {
            private static void Postfix()
            {
                // Replace "Reject All" button text with "Shuffle"
                UI.IMMIGRANTSCREEN.REJECTALL = UI.IMMIGRANTSCREEN.SHUFFLE;
            }
        }

        [HarmonyPatch(typeof(ImmigrantScreen), "Initialize")]
        private class ImmigrantScreen_Initialize
        {
            private static bool Prefix(ImmigrantScreen __instance, Telepad telepad)
            {
                var traverse = Traverse.Create(__instance);
                traverse.Method("InitializeContainers").GetValue();
                traverse.Field("telepad").SetValue(telepad);
                return false;
            }
        }
    }
}
