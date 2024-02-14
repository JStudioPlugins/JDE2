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
    public class ModelPlacementPreviewDebug : IAssetDebug
    {
        public Type TargetClass => typeof(ItemBarricadeAsset);

        public void InterceptPopulate(Asset asset, Bundle bundle, DatDictionary data, Local localization)
        {
            var barricade = bundle.load<GameObject>("Barricade");

            if (barricade != null)
            {
                void Check(GameObject parent)
                {
                    if (!parent.name.Contains("Model_"))
                        if (parent.transform.TryGetComponent(out MeshRenderer component))
                        {
                            this.LogError($"BARRICADE ASSET WITH AN GUID OF {asset.GUID} AND A NAME OF {asset.name} HAS A MESH GAMEOBJECT THAT ISN'T NAMED CORRECTLY: {parent.name}");
                        }

                    foreach (Transform child in parent.transform)
                    {
                        Check(child.gameObject);
                    }
                }

                Check(barricade);
            }
        }
    }
}
