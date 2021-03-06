﻿using System;
using System.Collections.Generic;
using System.Linq;
using BattleTech;
using us.frostraptor.modUtils;

namespace SkillBasedInit.Helper {
    public class PilotHelper {

        // The penalty range for injuries, keyed by guts level.
        private static readonly Dictionary<int, int[]> InjuryBounds = new Dictionary<int, int[]> {
            {  1, new[] { 5, 7 } },
            {  2, new[] { 4, 6 } },
            {  3, new[] { 4, 6 } },
            {  4, new[] { 3, 6 } },
            {  5, new[] { 3, 6 } },
            {  6, new[] { 3, 5 } },
            {  7, new[] { 3, 5 } },
            {  8, new[] { 2, 5 } },
            {  9, new[] { 2, 5 } },
            { 10, new[] { 1, 4 } },
            { 11, new[] { 1, 3 } },
            { 12, new[] { 1, 3 } },
            { 13, new[] { 1, 2 } }
        };

        // The randomness each round, keyed by piloting level.
        private static readonly Dictionary<int, int[]> RandomnessBounds = new Dictionary<int, int[]> {
            {  1, new[] { 3, 9 } },
            {  2, new[] { 2, 8 } },
            {  3, new[] { 2, 8 } },
            {  4, new[] { 1, 7 } },
            {  5, new[] { 1, 7 } },
            {  6, new[] { 0, 6 } },
            {  7, new[] { 0, 6 } },
            {  8, new[] { 0, 5 } },
            {  9, new[] { 0, 5 } },
            { 10, new[] { 0, 4 } },
            { 11, new[] { 0, 3 } },
            { 12, new[] { 0, 3 } },
            { 13, new[] { 0, 2 } }
        };

        private static readonly Dictionary<int, int> ModifierBySkill = new Dictionary<int, int> {
            { 1, 0 },
            { 2, 1 },
            { 3, 1 },
            { 4, 2 },
            { 5, 2 },
            { 6, 3 },
            { 7, 3 },
            { 8, 4 },
            { 9, 4 },
            { 10, 5 },
            { 11, 6 },
            { 12, 7 },
            { 13, 8 }
        };

        // Process any tags that provide flat bonuses
        public static int GetTagsModifier(Pilot pilot) {
            int mod = 0;

            foreach (string tag in pilot.pilotDef.PilotTags.Distinct()) {
                if (Mod.Config.PilotTagModifiers.ContainsKey(tag)) {
                    int tagMod = Mod.Config.PilotTagModifiers[tag];
                    //Mod.Log.Debug?.Write($"Pilot {pilot.Name} has tag:{tag}, applying modifier:{tagMod}");
                    mod += tagMod;
                }
            }

            return mod;
        }

        // Generates tooltip details for tags that provide modifiers
        public static List<string> GetTagsModifierDetails(Pilot pilot, int space=2) {
            List<string> details = new List<string>();

            foreach (string tag in pilot.pilotDef.PilotTags.Distinct()) {
                if (Mod.Config.PilotTagModifiers.ContainsKey(tag)) {
                    int tagMod = Mod.Config.PilotTagModifiers[tag];
                    if (tagMod > 0) {
                        details.Add($"<space={space}em><color=#00FF00>{tag}: {tagMod:+0}</color>");
                    } else if (tagMod < 0) {
                        details.Add($"<space={space}em><color=#FF0000>{tag}: {tagMod}</color>");
                    }
                }
            }

            return details;
        }

        public static int GetGunneryModifier(Pilot pilot) {
            return GetModifier(pilot, pilot.Gunnery, "AbilityDefG5", "AbilityDefG8");
        }

        public static int GetGutsModifier(Pilot pilot) {
            return GetModifier(pilot, pilot.Guts, "AbilityDefGu5", "AbilityDefGu8");
        }

        public static int GetPilotingModifier(Pilot pilot) {
            return GetModifier(pilot, pilot.Piloting, "AbilityDefP5", "AbilityDefP8");
        }

        public static int GetTacticsModifier(Pilot pilot) {
            return GetModifier(pilot, pilot.Tactics, "AbilityDefT5A", "AbilityDefT8A");
        }

        public static int GetModifier(Pilot pilot, int skillValue, string abilityDefIdL5, string abilityDefIdL8) {
            int normalizedVal = SkillUtils.NormalizeSkill(skillValue);
            int mod = ModifierBySkill[normalizedVal];

            bool hasL5 = false;
            bool hasL8 = false;
            foreach (Ability ability in pilot.Abilities) {
                //Mod.Log.Trace?.Write($"Pilot {pilot.Name} has ability:{ability.Def.Id}.");
                if (ability.Def.Id.ToLower().Equals(abilityDefIdL5.ToLower())) {
                    //Mod.Log.Debug?.Write($"Pilot {pilot.Name} has L5 ability:{ability.Def.Id}.");
                    hasL5 = true;
                }

                if (ability.Def.Id.ToLower().Equals(abilityDefIdL8.ToLower())) {
                    //Mod.Log.Debug?.Write($"Pilot {pilot.Name} has L8 ability:{ability.Def.Id}.");
                    hasL5 = true;
                }
            }
            if (hasL5) mod++;
            if (hasL8) mod++;

            return mod;
        }

        public static int GetCalledShotModifier(Pilot pilot) {
            int gunneryMod = GetGunneryModifier(pilot);
            int tacticsMod = GetTacticsModifier(pilot);
            int average = (int) Math.Floor((gunneryMod + tacticsMod) / 2.0);
            return average;
        }

        public static int GetVigilanceModifier(Pilot pilot) {
            int gutsMod = GetGutsModifier(pilot);
            int tacticsMod = GetTacticsModifier(pilot);
            int average = (int)Math.Floor((gutsMod + tacticsMod) / 2.0);
            return average;
        }

        public static int[] GetInjuryBounds(Pilot pilot) {
            int normalizedVal = SkillUtils.NormalizeSkill(pilot.Guts);
            int[] bounds = new int[2];
            InjuryBounds[normalizedVal].CopyTo(bounds, 0);
            return bounds;
        }

        public static int[] GetRandomnessBounds(Pilot pilot) {
            int normalizedVal = SkillUtils.NormalizeSkill(pilot.Piloting);
            int[] bounds = new int[2];
            RandomnessBounds[normalizedVal].CopyTo(bounds, 0);
            return bounds;
        }

        public static void LogPilotStats(Pilot pilot) {
            if (Mod.Config.Debug) {
                int normedGuts = SkillUtils.NormalizeSkill(pilot.Guts);
                int gutsMod = GetGutsModifier(pilot);
                int normdPilot = SkillUtils.NormalizeSkill(pilot.Piloting);
                int pilotingMod = GetPilotingModifier(pilot);
                int normedTactics = SkillUtils.NormalizeSkill(pilot.Tactics);
                int tacticsMod = GetTacticsModifier(pilot);

                Mod.Log.Debug?.Write($"{pilot.Name} skill profile is " +
                    $"g:{pilot.Guts}->{normedGuts}={gutsMod}" +
                    $"p:{pilot.Piloting}->{normdPilot}={pilotingMod} " +
                    $"t:{pilot.Tactics}->{normedTactics}={tacticsMod} "
                    );
            }
        }
    }
}
