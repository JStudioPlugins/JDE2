using JDE2.SituationDependent;
using JDE2.Utils;
using JetBrains.Annotations;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace JDE2.Managers
{
    public class PluginManager
    {
        public static PluginManager Instance { get; private set; }

        public static bool Enabled
        {
            get => Config.Instance.Data.PluginConfig.Enabled;
        }

        public static string PluginsDirectory
        {
            get
            { 
                string path = Path.Combine(UnturnedPaths.RootDirectory.FullName, "Modules", "JDE2", "Plugins");
                if (!Directory.Exists(path)) Directory.CreateDirectory(path);
                return path;
            }

        }

        public List<Assembly> Plugins { get; private set; }

        public delegate void PostPluginsLoaded();

        public event PostPluginsLoaded OnPostPluginsLoaded;

        public PluginManager()
        {
            if (!Enabled) return;

            Instance = this;
            Plugins = new();

            foreach (string pluginLibrary in Directory.GetFiles(PluginsDirectory, "*.dll", SearchOption.AllDirectories))
            {
                Plugins.Add(Assembly.LoadFile(pluginLibrary));
                LoggingManager.Log($"Mounted {{{{cyan}}}}{pluginLibrary}{{{{_}}}}!");
            }

            foreach (Assembly plugin in Plugins)
            {
                DependentManager.ActivateDependentsByType<IJDEDependent>(plugin);
            }

            OnPostPluginsLoaded?.Invoke();
        }
    }

    public class PluginIEditorDependentLoader : IEditorDependent
    {
        public PluginIEditorDependentLoader()
        {
            if (!PluginManager.Enabled) return;

            foreach (Assembly plugin in PluginManager.Instance.Plugins)
            {
                DependentManager.ActivateDependentsByType<IEditorDependent>(plugin);
            }
        }
    }

    public class PluginISingleplayerDependentLoader : ISingleplayerDependent
    {
        public PluginISingleplayerDependentLoader()
        {
            if (!PluginManager.Enabled) return;

            foreach (Assembly plugin in PluginManager.Instance.Plugins)
            {
                DependentManager.ActivateDependentsByType<ISingleplayerDependent>(plugin);
            }
        }
    }

    public class PluginIMenuDependentLoader : IMenuDependent
    {
        public PluginIMenuDependentLoader()
        {
            if (!PluginManager.Enabled) return;

            foreach (Assembly plugin in PluginManager.Instance.Plugins)
            {
                DependentManager.ActivateDependentsByType<IMenuDependent>(plugin);
            }
        }
    }

    public class PluginIConsoleDependentLoader : IConsoleDependent
    {
        public PluginIConsoleDependentLoader()
        {
            if (!PluginManager.Enabled) return;

            foreach (Assembly plugin in PluginManager.Instance.Plugins)
            {
                DependentManager.ActivateDependentsByType<IConsoleDependent>(plugin);
            }
        }
    }

    public class PluginIMultiplayerDependentLoader : IMultiplayerDependent
    {
        public PluginIMultiplayerDependentLoader()
        {
            if (!PluginManager.Enabled) return;

            foreach (Assembly plugin in PluginManager.Instance.Plugins)
            {
                DependentManager.ActivateDependentsByType<IMultiplayerDependent>(plugin);
            }
        }
    }
}
