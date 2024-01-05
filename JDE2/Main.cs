using HarmonyLib;
using JDE2.Managers;
using JDE2.Utils;
using JDE2.UI;
using JetBrains.Annotations;
using SDG.Framework.Modules;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using Action = System.Action;
using System.Threading;
using JDE2.SituationDependent;
using Module = SDG.Framework.Modules.Module;
using Steamworks;
using System.IO;

namespace JDE2
{
    public class Main : IModuleNexus
    {
        public Assembly Assembly { get => typeof(Main).Assembly; }

        public static string Directory { get => Path.Combine(UnturnedPaths.RootDirectory.FullName, "Modules", "JDE2"); }

        public static GameObject BackingObj { get; private set; }

        public static Main Instance { get; private set; }

        public static Module Module { get; private set; }

        public event Action OnLoad;

        public event Action OnUnload;

        public event Action OnGameMenuLoaded;

        public void initialize()
        {
            Instance = this;
            Module = ModuleHook.modules.FirstOrDefault(x => x.config.Name == "JDE2" && x.config.Version == Assembly.GetName().Version.ToString());

            UnturnedLog.info($"ATTEMPTING TO LOAD {Assembly.GetName().Name} {Assembly.GetName().Version}.");
            UnturnedLog.info($"GENERATING AND LOADING CONFIGURATION FOR {Assembly.GetName().Name}.");
            new Config();


            //Turn on plugin stuff.
            new PluginManager();

            //Console stuff.
            new ConsoleManager();
            LoggingManager.Log($"{{{{blue}}}}Now running JDE2 {Assembly.GetName().Version} - For support and additonal documentation, visit the JStudio Discord at https://discord.gg/XBSGmGTNWP {{{{_}}}}");

            BackingObj = new GameObject();
            GameObject.DontDestroyOnLoad(BackingObj);

            LoggingManager.Log("LOADING DispatcherTool.", true);
            BackingObj.AddComponent<DispatcherTool>();
            BackingObj.AddComponent<OnUpdateManager>();

            LoggingManager.Log($"LOADING DependentManager", true);
            new DependentManager();

            //Loading IJDEDependent types moved to DependentManager, trying to keep Main clean.

            //Events
            Level.onPostLevelLoaded += HandlePostLevelLoad;

            
            OnLoad?.Invoke();
            LoggingManager.Log($"JDE2 WENT THROUGH ITS NORMAL SETUP PROCESS.");
        }

        public void shutdown()
        {
            LoggingManager.Log($"UNLOADING {Assembly.GetName().Name}.");

            //Events
            Level.onPostLevelLoaded -= HandlePostLevelLoad;

            OnUnload?.Invoke();

            LoggingManager.Log($"Unloaded!");
        }

        private void HandlePostLevelLoad(int level)
        {
            //IDependent loading moved to DependentManager, trying to keep Main clean.
        }


        public void HandleGameMenuLoaded()
        {
            LoggingManager.Log("MAIN MENU WAS LOADED.", true);
            OnGameMenuLoaded?.Invoke();
            MenuUI.alert("YOU ARE CURRENTLY RUNNING JDE2. PLAYING MULTIPLAYER IS NOT ADVISED.");
        }
    }

    [HarmonyPatch(typeof(MenuUI))]
    [HarmonyPatch("customStart")]
    class OnUpdateObjectUIPatch
    {
        [HarmonyPostfix]
        internal static void OnUpdatePostfix()
        {
            Main.Instance.HandleGameMenuLoaded();

            //IMenuDependent loading moved to DependentManager, trying to keep Main clean.
        }

    }
}
