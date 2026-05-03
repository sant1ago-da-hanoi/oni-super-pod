using System;
using System.Collections.Generic;
using Database;
using HarmonyLib;
using Klei.AI;
using TUNING;
using UnityEngine;

namespace SuperPOD
{
    internal class MinionStartingStats_sant1ago
    {
        [HarmonyPatch(typeof(MinionStartingStats), "GenerateAptitudes")]
        private class GenerateAptitudes_Patch
        {
            private static bool Prefix(MinionStartingStats __instance, string guaranteedAptitudeID = null)
            {
                // Skip Bionic duplicants
                if (__instance.personality.model == GameTags.Minions.Models.Bionic)
                    return true;

                int num = ConfigData.GetInterestNumber();
                var list = new List<SkillGroup>(Db.Get().SkillGroups.resources);
                list.RemoveAll((SkillGroup match) => !match.allowAsAptitude);
                Util.Shuffle(list);

                if (guaranteedAptitudeID != null)
                {
                    __instance.skillAptitudes.Add(Db.Get().SkillGroups.Get(guaranteedAptitudeID), DUPLICANTSTATS.APTITUDE_BONUS);
                    list.Remove(Db.Get().SkillGroups.Get(guaranteedAptitudeID));
                    num--;
                }

                num = Mathf.Min(num, list.Count);
                for (int i = 0; i < num; i++)
                {
                    __instance.skillAptitudes.Add(list[i], DUPLICANTSTATS.APTITUDE_BONUS);
                }

                return false;
            }
        }

        [HarmonyPatch(typeof(MinionStartingStats), "GenerateTraits")]
        private class GenerateTraits_Patch
        {
            private static bool Prefix(MinionStartingStats __instance, ref int __result,
                bool is_starter_minion, List<ChoreGroup> disabled_chore_groups,
                string guaranteedAptitudeID = null)
            {
                // Bionic duplicants have their own trait logic — let the game handle them
                if (__instance.personality.model == GameTags.Minions.Models.Bionic)
                    return true;

                DUPLICANTSTATS.MAX_TRAITS = ConfigData.GetMaxTraits();

                int statDelta = 0;
                var selectedTraits = new List<string>();
                var randSeed = new KRandom();

                // --- Stress trait (from config) ---
                string stressId = ConfigData.GetStress();
                if (!string.IsNullOrEmpty(stressId))
                {
                    __instance.personality.stresstrait = stressId;
                    Trait stressTrait = Db.Get().traits.TryGet(stressId);
                    if (stressTrait != null)
                        __instance.stressTrait = stressTrait;
                }
                else
                {
                    // Fallback: use personality default
                    Trait stressTrait = Db.Get().traits.TryGet(__instance.personality.stresstrait);
                    if (stressTrait != null)
                        __instance.stressTrait = stressTrait;
                }

                // --- Joy trait (from config) ---
                string joyId = ConfigData.GetOverjoyed();
                if (!string.IsNullOrEmpty(joyId))
                {
                    __instance.personality.joyTrait = joyId;
                    Trait joyTrait = Db.Get().traits.TryGet(joyId);
                    if (joyTrait != null)
                        __instance.joyTrait = joyTrait;
                }
                else
                {
                    Trait joyTrait = Db.Get().traits.TryGet(__instance.personality.joyTrait);
                    if (joyTrait != null)
                        __instance.joyTrait = joyTrait;
                }

                __instance.stickerType = __instance.personality.stickerType;

                // --- Congenital trait (from personality, not config) ---
                string congenitalId = __instance.personality.congenitaltrait;
                if (!string.IsNullOrEmpty(congenitalId))
                {
                    Trait congenitalTrait = Db.Get().traits.TryGet(congenitalId);
                    if (congenitalTrait != null && congenitalTrait.Name != "None")
                    {
                        __instance.congenitaltrait = congenitalTrait;
                        // Add congenital trait to Traits list (game does this via SelectTrait)
                        __instance.Traits.Add(congenitalTrait);
                        var traitVal = congenitalTrait.PositiveTrait
                            ? DUPLICANTSTATS.GOODTRAITS.Find(m => m.id == congenitalTrait.Id)
                            : DUPLICANTSTATS.BADTRAITS.Find(m => m.id == congenitalTrait.Id);
                        if (!string.IsNullOrEmpty(traitVal.id))
                        {
                            selectedTraits.Add(traitVal.id);
                            statDelta += traitVal.statBonus;
                            __instance.rarityBalance += congenitalTrait.PositiveTrait ? -traitVal.rarity : traitVal.rarity;
                        }
                        if (congenitalTrait.disabledChoreGroups != null)
                        {
                            for (int i = 0; i < congenitalTrait.disabledChoreGroups.Length; i++)
                            {
                                if (congenitalTrait.disabledChoreGroups[i] != null)
                                    disabled_chore_groups.Add(congenitalTrait.disabledChoreGroups[i]);
                            }
                        }
                    }
                    else
                    {
                        __instance.congenitaltrait = null;
                    }
                }
                else
                {
                    __instance.congenitaltrait = null;
                }

                // --- Add good/bad traits ---
                Func<List<DUPLICANTSTATS.TraitVal>, bool, bool> addTrait = (traitPossibilities, positiveTrait) =>
                {
                    if (__instance.Traits.Count > DUPLICANTSTATS.MAX_TRAITS)
                        return false;

                    var shuffled = new List<DUPLICANTSTATS.TraitVal>(traitPossibilities);
                    shuffled.ShuffleSeeded(randSeed);

                    foreach (var item in shuffled)
                    {
                        if (!Game.IsCorrectDlcActiveForCurrentSave(item))
                            continue;
                        if (selectedTraits.Contains(item.id))
                            continue;
                        if (item.doNotGenerateTrait)
                            continue;

                        Trait trait = Db.Get().traits.TryGet(item.id);
                        if (trait == null)
                            continue;

                        // Skip traits that disable chores for starter minions in debug mode
                        if (is_starter_minion && !trait.ValidStarterTrait)
                            continue;

                        selectedTraits.Add(item.id);
                        statDelta += item.statBonus;
                        __instance.rarityBalance += positiveTrait ? -item.rarity : item.rarity;
                        __instance.Traits.Add(trait);

                        if (trait.disabledChoreGroups != null)
                        {
                            for (int i = 0; i < trait.disabledChoreGroups.Length; i++)
                            {
                                if (trait.disabledChoreGroups[i] != null)
                                    disabled_chore_groups.Add(trait.disabledChoreGroups[i]);
                            }
                        }
                        return true;
                    }
                    return false;
                };

                int positiveTarget = ConfigData.GetPositiveTraitsNumber();
                int negativeTarget = ConfigData.GetNegativeTraitsNumber();
                int positiveCount = 0;
                int negativeCount = 0;
                int maxAttempts = (positiveTarget + negativeTarget) * 4;

                while (maxAttempts > 0 && (negativeCount < negativeTarget || positiveCount < positiveTarget))
                {
                    if (negativeCount < negativeTarget && addTrait(DUPLICANTSTATS.BADTRAITS, false))
                        negativeCount++;
                    if (positiveCount < positiveTarget && addTrait(DUPLICANTSTATS.GOODTRAITS, true))
                        positiveCount++;
                    maxAttempts--;
                }

                __instance.IsValid = true;
                __result = statDelta;
                return false;
            }
        }

        [HarmonyPatch(typeof(MinionStartingStats), "GenerateAttributes")]
        private class GenerateAttributes_Patch
        {
            private static bool Prefix(MinionStartingStats __instance, int pointsDelta,
                List<ChoreGroup> disabled_chore_groups)
            {
                var allAttributes = new List<string>(DUPLICANTSTATS.ALL_ATTRIBUTES);
                for (int i = 0; i < allAttributes.Count; i++)
                {
                    if (!__instance.StartingLevels.ContainsKey(allAttributes[i]))
                        __instance.StartingLevels[allAttributes[i]] = 0;
                }

                foreach (var aptitude in __instance.skillAptitudes)
                {
                    if (aptitude.Key.relevantAttributes.Count <= 0)
                        continue;

                    for (int j = 0; j < aptitude.Key.relevantAttributes.Count; j++)
                    {
                        string attrId = aptitude.Key.relevantAttributes[j].Id;
                        if (!__instance.StartingLevels.ContainsKey(attrId))
                        {
                            Debug.LogError("Need to add " + attrId + " to TUNING.DUPLICANTSTATS.ALL_ATTRIBUTES");
                        }
                        __instance.StartingLevels[attrId] = ConfigData.GetInterestValue();
                    }
                }

                return false;
            }
        }
    }
}
