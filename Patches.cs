using HarmonyLib;
using UnityEngine;

namespace SuperPOD
{
    public class Patches
    {
        [HarmonyPatch(typeof(Db), "Initialize")]
        public class Db_Initialize_Patch
        {
            public static void Prefix()
            {
                Debug.Log("[SuperPOD] Db.Initialize starting");
            }

            public static void Postfix()
            {
                Debug.Log("[SuperPOD] Db.Initialize complete");
            }
        }
    }
}
