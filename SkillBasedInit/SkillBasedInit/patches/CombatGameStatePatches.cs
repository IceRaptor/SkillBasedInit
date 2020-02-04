﻿using BattleTech;
using BattleTech.Data;
using Harmony;
using SVGImporter;

namespace SkillBasedInit.patches {

    [HarmonyPatch(typeof(CombatGameState), "_Init")]
    public static class CombatGameState__Init {
        public static void Postfix(CombatGameState __instance) {
            Mod.Log.Trace("CGS:_I entered.");
            Mod.Log.Debug("Caching CombatGameState");
            ModState.Combat = __instance;

            // Pre-load our required icons, otherwise DM will unload them as they aren't necessary
            DataManager dm = UnityGameInstance.BattleTechGame.DataManager;
            LoadRequest loadRequest = dm.CreateLoadRequest();

            // Need to load each unique icon
            Mod.Log.Info("LOADING EFFECT ICONS...");
            loadRequest.AddLoadRequest<SVGAsset>(BattleTechResourceType.SVGAsset, Mod.Config.Icons.Stopwatch, null);
            loadRequest.ProcessRequests();
            Mod.Log.Info("  ICON LOADING COMPLETE!");
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