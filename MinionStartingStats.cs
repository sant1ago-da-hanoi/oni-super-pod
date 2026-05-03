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
                int num = ConfigData.GetInterestNumber();
                var list = new List<SkillGroup>(Db.Get().SkillGroups.resources);
                Util.Shuffle(list);

                if (guaranteedAptitudeID != null)
                {
                    __instance.skillAptitudes.Add(Db.Get().SkillGroups.Get(guaranteedAptitudeID), DUPLICANTSTATS.APTITUDE_BONUS);
                    list.Remove(Db.Get().SkillGroups.Get(guaranteedAptitudeID));
                    num--;
                }

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
                DUPLICANTSTATS.MAX_TRAITS = ConfigData.GetMaxTraits();
                __instance.personality.stresstrait = ConfigData.GetStress();
                __instance.personality.joyTrait = ConfigData.GetOverjoyed();

                int statDelta = 0;
                var selectedTraits = new List<string>();
                var randSeed = new KRandom();

                Trait stressTrait = Db.Get().traits.Get(__instance.personality.stresstrait);
                __instance.stressTrait = stressTrait;

                Trait joyTrait = Db.Get().traits.Get(__instance.personality.joyTrait);
                __instance.joyTrait = joyTrait;

                __instance.stickerType = __instance.personality.stickerType;

                Trait congenitalTrait = Db.Get().traits.Get(__instance.personality.congenitaltrait);
                __instance.congenitaltrait = congenitalTrait.Name == "None" ? null : congenitalTrait;

                Func<List<DUPLICANTSTATS.TraitVal>, bool, bool> addTrait = (traitPossibilities, positiveTrait) =>
                {
                    if (__instance.Traits.Count > DUPLICANTSTATS.MAX_TRAITS)
                        return false;

                    var shuffled = new List<DUPLICANTSTATS.TraitVal>(traitPossibilities);
                    shuffled.ShuffleSeeded(randSeed);

                    foreach (var item in shuffled)
                    {
                        if (Game.IsCorrectDlcActiveForCurrentSave(item) && !selectedTraits.Contains(item.id))
                        {
                            Trait trait = Db.Get().traits.TryGet(item.id);
                            if (trait != null)
                            {
                                selectedTraits.Add(item.id);
                                statDelta += item.statBonus;
                                __instance.rarityBalance += positiveTrait ? -item.rarity : item.rarity;
                                __instance.Traits.Add(trait);

                                if (trait.disabledChoreGroups != null)
                                {
                                    for (int i = 0; i < trait.disabledChoreGroups.Length; i++)
                                        disabled_chore_groups.Add(trait.disabledChoreGroups[i]);
                                }
                                return true;
                            }
                            Debug.LogWarning("Trying to add nonexistent trait: " + item.id);
                        }
                    }
                    return false;
                };

                int positiveTarget = ConfigData.GetPositiveTraitsNumber();
                int negativeTarget = ConfigData.GetNegativeTraitsNumber();
                int positiveCount = 0;
                int negativeCount = 0;

                while (negativeCount < negativeTarget || positiveCount < positiveTarget)
                {
                    if (negativeCount < negativeTarget && addTrait(DUPLICANTSTATS.BADTRAITS, false))
                        negativeCount++;
                    if (positiveCount < positiveTarget && addTrait(DUPLICANTSTATS.GOODTRAITS, true))
                        positiveCount++;
                }

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
