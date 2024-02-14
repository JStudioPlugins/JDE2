using JDE2.AssetDebugging;
using JDE2.Managers;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace JDE2.AssetDebugging
{
    public class BarricadeClipPrefabDebug : IAssetDebug
    {
        public Type TargetClass => typeof(ItemBarricadeAsset);

        public void InterceptPopulate(Asset asset, Bundle bundle, DatDictionary data, Local localization)
        {
            if (bundle.load<GameObject>("Clip") != null)
            {
                this.LogError($"ASSET WITH A GUID OF {asset.GUID} AND A NAME OF {asset.name} HAS A CLIP PREFAB, CONSIDER ITS REMOVAL.");
            }
        }
    }
}
