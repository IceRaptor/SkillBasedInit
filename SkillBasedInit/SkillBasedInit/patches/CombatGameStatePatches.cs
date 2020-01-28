﻿using BattleTech;
using Harmony;

namespace SkillBasedInit.patches {

    [HarmonyPatch(typeof(CombatGameState), "_Init")]
    public static class CombatGameState__Init {
        public static void Postfix(CombatGameState __instance) {
            Mod.Log.Debug("Caching CombatGameState");
            ModState.Combat = __instance;
        }
    }

    [HarmonyPatch(typeof(CombatGameState), "OnCombatGameDestroyed")]
    public static class CombatGameState_OnCombatGameDestroyed {
        public static void Postfix(CombatGameState __instance) {
            Mod.Log.Debug("Clearing cached copy of CombatGameState");
            ModState.Combat = null;
        }
    }

}
