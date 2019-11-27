﻿using BattleTech;
using Harmony;
using System;

namespace SkillBasedInit.patches {
    [HarmonyPatch(typeof(TurnDirector), MethodType.Constructor)]
    [HarmonyPatch(new Type[] { typeof(CombatGameState) })]
    public static class TurnDirector_ctor {
        public static void Postfix(TurnDirector __instance) {
            Mod.Log.Trace("TD:ctor:post - entered.");
            int ___FirstPhase = (int)Traverse.Create(__instance).Property("FirstPhase").GetValue();
            int ___LastPhase = (int)Traverse.Create(__instance).Property("LastPhase").GetValue();
            Mod.Log.Debug($" TurnDirector init with phases: {___FirstPhase} / {___LastPhase}");

            //Traverse.Create(__instance).Property("FirstPhase").SetValue(1);
            //Traverse.Create(__instance).Property("LastPhase").SetValue(Mod.MaxPhase);
            //Mod.Log.Debug($" TurnDirector phases changed to: {___FirstPhase} / {Mod.MaxPhase}");
        }
    }

    [HarmonyPatch(typeof(TurnDirector), "OnCombatGameDestroyed")]
    public static class TurnDirector_OnCombatGameDestroyed {
        public static void Postfix(TurnDirector __instance) {
            Mod.Log.Trace("TD:OCGD:post - entered.");
            Mod.Log.Debug($" TurnDirector - Combat complete, destroying initiative map.");
            ActorInitiativeHolder.OnCombatComplete();
        }
    }

    [HarmonyPatch(typeof(TurnDirector), "BeginNewPhase")]
    public static class TurnDirector_BeginNewPhase {
        public static void Postfix(TurnDirector __instance, int newPhase) {
            Mod.Log.Trace($"TD:BNP - for phase: {newPhase}  currentPhase:{__instance.CurrentPhase}  nonInterleavedPhase:{__instance.NonInterleavedPhase}  phaseIncrement:{__instance.PhaseIncrement}");
        }
    }

    [HarmonyPatch(typeof(TurnDirector), "EndCurrentPhase")]
    public static class TurnDirector_EndCurrentPhase {
        public static void Postfix(TurnDirector __instance) {
            Mod.Log.Trace($"TD:ECP - ending phase: {__instance.CurrentPhase}  nonInterleavedPhase:{__instance.NonInterleavedPhase}  phaseIncrement:{__instance.PhaseIncrement}");
        }
    }

    [HarmonyPatch(typeof(TurnDirector), "EndCurrentPhaseComplete")]
    public static class TurnDirector_EndCurrentPhaseComplete {
        public static void Postfix(TurnDirector __instance) {
            Mod.Log.Trace($"TD:ECPC - ending phase: {__instance.CurrentPhase} complete  nonInterleavedPhase:{__instance.NonInterleavedPhase}  phaseIncrement:{__instance.PhaseIncrement}");
        }
    }

    [HarmonyPatch(typeof(TurnDirector), "NotifyTurnEvents")]
    public static class TurnDirector_NotifyTurnEvents {
        public static void Postfix(TurnDirector __instance) {
            Mod.Log.Trace($"TD:NTE - phase: {__instance.CurrentPhase} has {__instance.TurnEvents.Count} turn events.  nonInterleavedPhase:{__instance.NonInterleavedPhase}  phaseIncrement:{__instance.PhaseIncrement}");
        }
    }

    [HarmonyPatch(typeof(TurnDirector), "BeginNewRound")]
    public static class TurnDirector_BeginNewRound {
        public static void Postfix(TurnDirector __instance, int round) {
            Mod.Log.Trace($"TD:BNR - for round: {round}");
        }
    }

    [HarmonyPatch(typeof(TurnDirector), "IncrementActiveTurnActor")]
    public static class TurnDirector_IncrementActiveTurnActor {
        public static void Postfix(TurnDirector __instance) {
            Mod.Log.Trace($"TD:IATA - entered with isInterleaved:{__instance.IsInterleaved}  isInterleavedPending:{__instance.IsInterleavePending}  isNonInterleavePending:{__instance.IsNonInterleavePending}");

        }
    }

    [HarmonyPatch(typeof(TurnDirector), "NotifyContact")]
    public static class TurnDirector_NotifyContact {
        public static void Postfix(TurnDirector __instance, VisibilityLevel contactLevel) {
            Mod.Log.Trace($"TD:NC - notifying contact due to visibilityLevel:{contactLevel}");
        }
    }
}
