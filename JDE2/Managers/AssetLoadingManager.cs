using HarmonyLib;
using JDE2.AssetDebugging;
using JDE2.Assets;
using JDE2.SituationDependent;
using JDE2.UI;
using JDE2.Utils;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Assertions;

namespace JDE2.Managers
{
    public class AssetLoadingManager : IJDEDependent
    {
        internal static List<IAssetDebug> _assetDebugs = new();

        public static IReadOnlyList<IAssetDebug> AssetDebugs
        {
            get { return _assetDebugs.AsReadOnly(); }
        }

        public AssetLoadingManager()
        {
            RegisterCustomAssetTypesForAssembly(Main.Instance.Assembly);

            if (PluginManager.Enabled)
                PluginManager.Instance.OnPostPluginsLoaded += HandlePostPluginsLoaded;

            foreach (Type type in ReflectionTool.GetTypesFromBaseClass<Asset>(typeof(Level).Assembly))
                PatchAssetType(type);
        }

        private void HandlePostPluginsLoaded()
        {

        }

        public static bool PatchAssetType(Type type)
        {
            bool problematicFlag = true;


            try
            {
                PatchManager.harmony.Patch(type.GetMethod("PopulateAsset", BindingFlags.Public | BindingFlags.Instance | BindingFlags.InvokeMethod), prefix: new HarmonyMethod(typeof(PopulateAssetPatch).GetMethod("PopulateAssetPrefix", BindingFlags.Static | BindingFlags.Instance | BindingFlags.NonPublic)));

                LoggingManager.Log($"PATCHED ASSET TYPE {{{{cyan}}}}{type.FullName}{{{{_}}}}", true);
            }
            catch (Exception ex)
            {
                LoggingManager.LogException(ex, $"Encountered an error when patching asset type {{{{cyan}}}}{type.FullName}{{{{0}}}}!");
                problematicFlag = false;
            }


            return problematicFlag;
        }

        /// <summary>
        /// Registers ICustomAsset types, this allows for easier implementation of custom assets.
        /// </summary>
        /// <param name="assem">Assembly to check.</param>
        /// <returns>True if the registration worked out fine.</returns>
        public static bool RegisterCustomAssetTypesForAssembly(Assembly assem)
        {
            var types = ReflectionTool.GetTypesFromInterface<ICustomAsset>(assem);
            bool problematicFlag = true;

            foreach (Type type in types)
            {
                try
                {
                    if (Config.Instance.Data.DeveloperConfig.DisabledTypes.Contains(type.FullName)) continue;
                    var newAsset = (ICustomAsset)Activator.CreateInstance(type);
                    if (!type.IsAssignableFrom(typeof(Asset))) continue;
                    PatchAssetType(type);
                    LoggingManager.Log($"REGISTERED ASSET TYPE {{{{cyan}}}}{type.FullName}{{{{_}}}}", true);
                }
                catch (Exception ex)
                {
                    LoggingManager.LogException(ex, $"Encountered an error when creating an instance/registering asset type {{{{cyan}}{type.FullName}{{{{0}}}}!");
                    problematicFlag = false;
                }
            }

            return problematicFlag;
        }

        /// <summary>
        /// Registers IAssetDebug types, this allows for easier implementation of debugs.
        /// </summary>
        /// <param name="assem">Assembly to check.</param>
        /// <returns>True if the registration worked out fine.</returns>
        public static bool RegisterAssetDebugs(Assembly assem)
        {

            var types = ReflectionTool.GetTypesFromInterface<IAssetDebug>(assem);
            bool problematicFlag = true;

            foreach (Type type in types)
            {
                try
                {
                    if (Config.Instance.Data.DeveloperConfig.DisabledTypes.Contains(type.FullName)) continue;
                    var debug = (IAssetDebug)Activator.CreateInstance(type);
                    _assetDebugs.Add(debug);
                    LoggingManager.Log($"REGISTERED ASSET DEBUG {{{{cyan}}}}{type.FullName}{{{{_}}}}", true);
                }
                catch (Exception ex)
                {
                    LoggingManager.LogException(ex, $"Encountered an error when creating an instance/registering asset debug {{{{cyan}}{type.FullName}{{{{0}}}}!");
                    problematicFlag = false;
                }
            }

            return problematicFlag;
        }
    }

    public class AssetLoadingPluginManager : IJDEDependent
    {
        public AssetLoadingPluginManager()
        {
            if (!PluginManager.Enabled)
            {
                LoggingManager.Log("SKIPPED REGISTERING PLUGIN ASSET TYPES AND DEBUGS.");
            }

            foreach (Assembly assembly in PluginManager.Instance.Plugins)
            {
                AssetLoadingManager.RegisterCustomAssetTypesForAssembly(assembly);
                AssetLoadingManager.RegisterAssetDebugs(assembly);
            }
        }
    }

    [HarmonyPatch(typeof(SDG.Unturned.Assets))]
    [HarmonyPatch("AddSandboxSearchLocations")]
    class AddSandboxSearchLocationsPatch
    {
        [HarmonyPrefix]
        internal static bool AddSandboxSearchLocationsPrefix()
        {
            if (Config.Instance.Data.AssetsConfig.LoadSandbox) return true;
            else return false;
        }

    }

    class PopulateAssetPatch
    {
        [HarmonyPrefix]
        internal static bool PopulateAssetPrefix(Asset __instance, Bundle bundle, DatDictionary data, Local localization)
        {
            var debug = AssetLoadingManager._assetDebugs.FirstOrDefault(x => __instance.GetType().IsAssignableFrom(x.TargetClass));
            if (debug != null)
                debug.InterceptPopulate(__instance, bundle, data, localization);
            return true;
        }

    }

    [HarmonyPatch(typeof(SDG.Unturned.Assets))]
    [HarmonyPatch("AddMapSearchLocations")]
    class AddMapSearchLocationsPatch
    {
        [HarmonyPrefix]
        internal static bool AddMapSearchLocationsPrefix()
        {
            if (Config.Instance.Data.AssetsConfig.LoadMaps) return true;
            else return false;
        }
    }

    [HarmonyPatch(typeof(SDG.Unturned.Assets))]
    [HarmonyPatch("AddClientUgcSearchLocations")]
    class AddClientUgcSearchLocationsPatch
    {
        [HarmonyPrefix]
        internal static bool AddClientUgcSearchLocationsPrefix()
        {
            if (Config.Instance.Data.AssetsConfig.LoadWorkshop) return true;
            else return false;

        }
    }
}
