﻿using JDE2.SituationDependent;
using JDE2.Utils;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace JDE2.Managers
{
    /// <summary>
    /// Used for JDE2's SituationDependent interfaces. Modders originally had to create their own static instances of classes that inherited the interface.
    /// </summary>
    public class DependentManager
    {
        public static DependentManager Instance { get; private set; }

        public static List<IDependent> ActiveDependents { get; private set; }

        public static IReadOnlyList<T> ActiveDependentsByType<T>() where T : IDependent
        {
            return (IReadOnlyList<T>)ActiveDependents.Where(x => x is T);
        }

        /// <summary>
        /// This code showed up quite a bit throughout the old code, so it is nice to just combine it into one method.
        /// </summary>
        /// <typeparam name="T">Should be an IDependent type, starts up all ones of that type.</typeparam>
        /// <param name="assem">Assembly that should be searched to activate IDependents.</param>
        /// <returns>False if there was an exception while activating an IDependent.</returns>
        public static bool ActivateDependentsByType<T>(Assembly assem) where T : IDependent
        {
            var types = ReflectionTool.GetTypesFromInterface(assem, typeof(T).FullName);
            bool problematicFlag = true;
            foreach (Type type in types)
            {
                try
                {
                    if (IsTypeDisabled(type)) continue;
                    ActiveDependents.Add((IDependent)Activator.CreateInstance(type));
                    LoggingManager.Log($"STARTED {{cyan}}{type.FullName}{{_}}", true);
                }
                catch (Exception ex)
                {
                    LoggingManager.LogException(ex, $"Encountered an error when creating an instance of ~cyan~{type.FullName}~0~");
                    problematicFlag = false;
                }
            }
            return problematicFlag;
        }

        public static bool IsTypeDisabled(Type type)
        {
            return Config.Instance.Data.DeveloperConfig.DisabledTypes.Contains(type.FullName);
        }

        public DependentManager()
        {
            Instance = this;

            ActiveDependents = new();

            LoggingManager.Log("LOADING IJDEDependent TYPES!", true);
            ActivateDependentsByType<IJDEDependent>(Main.Instance.Assembly);

            Level.onPostLevelLoaded += LoadLevelDependents;
            Main.Instance.OnGameMenuLoaded += LoadMenuDependents;
            ConsoleManager.OnConsoleEnabled += LoadConsoleDependents;
        }

        private void LoadLevelDependents(int level)
        {
            LoggingManager.Log("REMOVING MENU DEPENDENTS.", true);
            ActiveDependents.RemoveAll(x => x is IMenuDependent);

            LoggingManager.Log("LEVEL IS LOADED. NOW STARTING IEditorDependent TYPES!", true);
            if (Level.isEditor)
            {
                /*UnturnedLog.info("ADDING OnUpdateManager TO PROVIDE HOTKEY FUNCTIONALITY.");
                Editor.editor.gameObject.AddComponent<OnUpdateManager>();*/
                //This was replaced by the BackingObj.

                ActivateDependentsByType<IEditorDependent>(Main.Instance.Assembly);
            }
            else
            {
                LoggingManager.Log("LEVEL IS NOT EDITOR. SWITCHING TO ISingleplayerDependent TYPES!", true);
                ActivateDependentsByType<ISingleplayerDependent>(Main.Instance.Assembly);
            }
        }

        private void LoadConsoleDependents()
        {
            LoggingManager.Log("LOADING IConsoleDependent TYPES!");
            ActiveDependentsByType<IConsoleDependent>();
        }

        private void LoadMenuDependents()
        {
            LoggingManager.Log("REMOVING ALL LEVEL RELATED DEPENDENTS.", true);
            ActiveDependents.RemoveAll(x => x is IEditorDependent || x is ISingleplayerDependent);

            LoggingManager.Log("MENU IS LOADED. NOW STARTING IMenuDependent TYPES!", true);
            ActivateDependentsByType<IMenuDependent>(Main.Instance.Assembly);
        }

    }
}
