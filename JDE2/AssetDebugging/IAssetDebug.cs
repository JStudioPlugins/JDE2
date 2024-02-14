
using JDE2.Managers;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JDE2.AssetDebugging
{
    public interface IAssetDebug
    {
        public Type TargetClass { get; }

        public void InterceptPopulate(Asset asset, Bundle bundle, DatDictionary data, Local localization);

    }

    public static class AssetDebugExtensions
    {
        public static void LogError(this IAssetDebug debug, string message)
        {
            LoggingManager.LogError(message);
            LoggingManager.Log($"To disable this asset debug check, specify the type {debug.GetType().FullName} in the DisabledTypes.");
        }
    }
}
