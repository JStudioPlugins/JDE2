using HarmonyLib;
using JDE2.Managers;
using JDE2.Tools;
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
using UnityEngine.SceneManagement;
using Action = System.Action;

namespace JDE2
{
    public class Main : IModuleNexus
    {
        public Assembly Assembly { get => typeof(Main).Assembly; }

        public static string Directory { get => UnturnedPaths.RootDirectory + "Modules/JDE2"; }

        public static Main Instance { get; private set; }

        public event Action OnLoad;

        public event Action OnUnload;

        public event Action OnGameMenuLoaded;

        public void initialize()
        {
            Instance = this;

            UnturnedLog.info($"ATTEMPTING TO LOAD {Assembly.GetName().Name} {Assembly.GetName().Version}.");
            UnturnedLog.info($"GENERATING AND LOADING CONFIGURATION FOR {Assembly.GetName().Name}.");
            new Config();
            UnturnedLog.info("LOADING IJDEDependent TYPES!");
            var types = ReflectionTool.GetTypesFromInterface(Assembly, "IJDEDependent");
            foreach (Type type in types)
            {
                object obj = Activator.CreateInstance(type);
                UnturnedLog.info($"STARTED {type.FullName}");
            }

            //Events
            Level.onPostLevelLoaded += HandlePostLevelLoad;
            

            OnLoad?.Invoke();
            UnturnedLog.info($"Loaded!");
        }

        public void shutdown()
        {
            UnturnedLog.info($"UNLOADING {Assembly.GetName().Name}.");

            //Events
            Level.onPostLevelLoaded -= HandlePostLevelLoad;

            OnLoad?.Invoke();

            UnturnedLog.info($"Unloaded!");
        }

        private void HandlePostLevelLoad(int level)
        {
            UnturnedLog.info("LEVEL IS LOADED. NOW STARTING IEditorDependent TYPES!");
            if (Level.isEditor)
            {
                UnturnedLog.info("ADDING OnUpdateManager TO PROVIDE HOTKEY FUNCTIONALITY.");
                Editor.editor.gameObject.AddComponent<OnUpdateManager>();

                var types = ReflectionTool.GetTypesFromInterface(Assembly, "IEditorDependent");
                foreach (Type type in types)
                {
                    Activator.CreateInstance(type);
                    UnturnedLog.info($"STARTED {type.FullName}");
                }

                
            }
            else
            {
                UnturnedLog.info("LEVEL IS NOT EDITOR. SWITCHING TO ISingleplayerDependent TYPES!");
                var types = ReflectionTool.GetTypesFromInterface(Assembly, "ISingleplayerDependent");
                foreach (Type type in types)
                {
                    Activator.CreateInstance(type);
                    UnturnedLog.info($"STARTED {type.FullName}");
                }
            }
        }

        
        public void HandleGameMenuLoaded()
        {
            OnGameMenuLoaded?.Invoke();
            MenuUI.alert("YOU ARE CURRENTLY RUNNING JDE. PLAYING MULTIPLAYER IS NOT ADVISED.");
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
        }

    }
}
