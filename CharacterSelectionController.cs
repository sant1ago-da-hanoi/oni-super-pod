using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;

namespace SuperPOD
{
    [HarmonyPatch(typeof(CharacterSelectionController), "InitializeContainers")]
    internal class CharacterSelectionController_sant1ago
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var codes = instructions.ToList();
            for (int i = 0; i < codes.Count; i++)
            {
                // Transpiler replaces hardcoded care package and duplicant counts
                // with calls to ConfigData methods
                if (codes.Count == 146)
                {
                    // Skip instructions 49-55 (original care package count)
                    if (i >= 49 && i <= 55)
                        continue;

                    // Replace instruction 56 with call to GetCarePackageNumber
                    if (i == 56)
                    {
                        codes[i] = new CodeInstruction(OpCodes.Call,
                            AccessTools.Method(typeof(ConfigData), "GetCarePackageNumber"));
                    }

                    // Skip instructions 59-61 (original duplicant count)
                    if (i >= 59 && i <= 61)
                        continue;

                    // Replace instruction 62 with call to GetDuplicantNumber
                    if (i == 62)
                    {
                        codes[i] = new CodeInstruction(OpCodes.Call,
                            AccessTools.Method(typeof(ConfigData), "GetDuplicantNumber"));
                    }
                }

                yield return codes[i];
            }
        }
    }
}
