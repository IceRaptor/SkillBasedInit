﻿using BattleTech;
using Harmony;
using System;

namespace SkillBasedInit {

    // Injure the pilot as soon as it happens
    [HarmonyPatch(typeof(Pilot), "InjurePilot")]
    [HarmonyPatch(new Type[] { typeof(string), typeof(int), typeof(int), typeof(DamageType), typeof(Weapon), typeof(AbstractActor) })]
    public static class Pilot_InjurePilot {

        public static void Prefix(Pilot __instance, ref int __state) {
            __state = 0;
            if (__instance.StatCollection.GetStatistic("BonusHeath") != null) {
                __state = __instance.StatCollection.GetStatistic("BonusHeath").Value<int>();
            }
            SkillBasedInit.Logger.Log($"Pilot:InjurePilot:pre - Actor:({__instance.ParentActor.DisplayName}_{__instance.Name}) injured with bonusHealth:{__state}");
        }

        public static void Postfix(Pilot __instance, int __state, string sourceID, int stackItemUID, int dmg, DamageType damageType, Weapon sourceWeapon, AbstractActor sourceActor) {
            SkillBasedInit.Logger.Log($"Pilot:InjurePilot:post - Actor:({__instance.ParentActor.DisplayName}_{__instance.Name}) injured with initial bonusHealth:{__state}");

            int currentBonus = 0;
            if (__instance.StatCollection.GetStatistic("BonusHeath") != null) {
                currentBonus = __instance.StatCollection.GetStatistic("BonusHeath").Value<int>();
            }
            int bonusDelta = __state - currentBonus;
            int damageTaken = dmg - bonusDelta;
            SkillBasedInit.LogDebug($"  Actor:({__instance.ParentActor.DisplayName}_{__instance.Name}) lost bonusHealth:{bonusDelta}, while results in damage:{damageTaken} from the attack.");

            // If the attacker took any damage, apply it
            if (damageTaken > 0) {
                AbstractActor parent = __instance.ParentActor;
                ActorInitiative target = ActorInitiativeHolder.GetOrCreate(parent);
                int injuryPenalty = target.CalculateInjuryPenalty(damageTaken, __instance.Injuries);

                if (!parent.HasActivatedThisRound) {
                    // Apply penalty in current round. Remember high init -> higher phase
                    parent.Initiative += injuryPenalty;
                    if (parent.Initiative > SkillBasedInit.MaxPhase) {
                        parent.Initiative = SkillBasedInit.MaxPhase;
                    }
                    parent.Combat.MessageCenter.PublishMessage(new ActorPhaseInfoChanged(parent.GUID));
                    parent.Combat.MessageCenter.PublishMessage(new FloatieMessage(parent.GUID, parent.GUID, $"OUCH! -{injuryPenalty} INITIATIVE", FloatieMessage.MessageNature.Debuff));
                } else {
                    // Injuries are cumulative
                    target.deferredInjuryMod += injuryPenalty;
                    parent.Combat.MessageCenter.PublishMessage(new FloatieMessage(parent.GUID, parent.GUID, $"OUCH! -{injuryPenalty} INITIATIVE NEXT ROUND", FloatieMessage.MessageNature.Debuff));
                }

            }
        }       
    }
}
