using HarmonyLib;
using JDE2.UI;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JDE2.Managers
{
    public class AssetLoadingManager
    {
    }

    [HarmonyPatch(typeof(Assets))]
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

    [HarmonyPatch(typeof(Assets))]
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
}
