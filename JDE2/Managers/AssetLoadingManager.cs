using HarmonyLib;
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

namespace JDE2.Managers
{
    public class AssetLoadingManager : IJDEDependent
    {
        public AssetLoadingManager() 
        {
            RegisterCustomAssetsForAssembly(Main.Instance.Assembly);   
        }

        public static bool RegisterCustomAssetsForAssembly(Assembly assem)
        {
            var types = ReflectionTool.GetTypesFromInterface<ICustomAsset>(assem);
            bool problematicFlag = true;

            foreach (Type type in types)
            {
                try
                {
                    if (Config.Instance.Data.DeveloperConfig.DisabledTypes.Contains(type.FullName)) continue;
                    var newAsset = (ICustomAsset)Activator.CreateInstance(type);
                    if (newAsset is not Asset) continue;
                    SDG.Unturned.Assets.assetTypes.addType(newAsset.TypeRegistryName, type);
                    LoggingManager.Log($"REGISTERED ASSET {{{{cyan}}}}{type.FullName}{{{{_}}}}", true);
                }
                catch (Exception ex)
                {
                    LoggingManager.LogException(ex, $"Encountered an error when creating an instance/registering asset {{{{cyan}}{type.FullName}{{{{0}}}}!");
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
                LoggingManager.Log("SKIPPED REGISTERING PLUGIN ASSETS.");
            }

            foreach (Assembly assembly in PluginManager.Instance.Plugins)
            {
                AssetLoadingManager.RegisterCustomAssetsForAssembly(assembly);
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
