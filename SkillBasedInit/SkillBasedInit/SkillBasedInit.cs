﻿using Harmony;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Reflection;
using us.frostraptor.modUtils.logging;

namespace SkillBasedInit {
    public class Mod {

        public const string HarmonyPackage = "us.frostraptor.SkillBasedInit";
        public const string LogName = "skill_based_init";

        public static IntraModLogger Log;
        public static string ModDir;
        public static ModConfig Config;

        public const int MaxPhase = 30;
        public const int MinPhase = 1;
        public static readonly Random Random = new Random();

        public static void Init(string modDirectory, string settingsJSON) {
            ModDir = modDirectory;

            Exception configE;
            try {
                Config = JsonConvert.DeserializeObject<ModConfig>(settingsJSON);
            } catch (Exception e) {
                configE = e;
                Config = new ModConfig();
            } finally {
                Config.InitializeColors();
            }

            Log = new IntraModLogger(modDirectory, "skill_based_init");

            Assembly asm = Assembly.GetExecutingAssembly();
            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(asm.Location);
            Log.Info($"Assembly version: {fvi.ProductVersion}");

            Log.Debug($"ModDir is:{modDirectory}");
            Log.Debug($"mod.json settings are:({settingsJSON})");
            Mod.Config.LogConfig();

            var harmony = HarmonyInstance.Create(HarmonyPackage);
            harmony.PatchAll(Assembly.GetExecutingAssembly());
            
        }

    }
}
