using HarmonyLib;
using KMod;
using PeterHan.PLib.Core;
using PeterHan.PLib.Options;
using UnityEngine;

namespace SuperPOD
{
    public class Patches : UserMod2
    {
        public override void OnLoad(Harmony harmony)
        {
            base.OnLoad(harmony);
            PUtil.InitLibrary(true);

            // Register English strings first (safe default — locale not yet available)
            SuperPODStrings.Register(false);

            new POptions().RegisterOptions(this, typeof(SuperPODOptions));

            // Widen PLib Options dialog
            try
            {
                var dialogType = AccessTools.TypeByName("PeterHan.PLib.Options.OptionsDialog");
                if (dialogType != null)
                {
                    var maxSizeField = AccessTools.Field(dialogType, "SETTINGS_DIALOG_MAX_SIZE");
                    if (maxSizeField != null)
                        maxSizeField.SetValue(null, new Vector2(1600f, 900f));

                    var sizeField = AccessTools.Field(dialogType, "SETTINGS_DIALOG_SIZE");
                    if (sizeField != null)
                        sizeField.SetValue(null, new Vector2(800f, 400f));
                }
            }
            catch (System.Exception e)
            {
                Debug.LogWarning("[SuperPOD] Could not widen options dialog: " + e.Message);
            }

            Debug.Log("[SuperPOD] Loaded with PLib Options");
        }

        private static bool IsVietnameseLocale()
        {
            try
            {
                // Try to get current locale from Localization
                var locale = Localization.GetLocale();
                if (locale != null && !string.IsNullOrEmpty(locale.Code))
                {
                    return locale.Code == "vi";
                }

                // Fallback: check language code
                var code = Localization.GetCurrentLanguageCode();
                if (!string.IsNullOrEmpty(code))
                {
                    return code == "vi";
                }
            }
            catch
            {
                // Localization may not be initialized yet during OnLoad
            }

            // Default to Vietnamese since this mod targets Vietnamese players
            return true;
        }

        [HarmonyPatch(typeof(Localization), "Initialize")]
        public class Localization_Initialize_Patch
        {
            public static void Postfix()
            {
                // Re-register strings with correct locale now that Localization is ready
                bool isVietnamese = IsVietnameseLocale();
                SuperPODStrings.Register(isVietnamese);
                Debug.Log($"[SuperPOD] Localization.Initialize — locale: {(isVietnamese ? "vi" : "en")}");
            }
        }

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
