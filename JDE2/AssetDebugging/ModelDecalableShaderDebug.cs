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
    public class ModelDecalableShaderDebug : IAssetDebug
    {
        public Type TargetClass => typeof(ObjectAsset);

        public void InterceptPopulate(Asset asset, Bundle bundle, DatDictionary data, Local localization)
        {
            var barricade = bundle.load<GameObject>("Object");

            if (barricade != null)
            {
                void Check(GameObject parent)
                {
                    if (parent.transform.TryGetComponent(out MeshRenderer component))
                    {
                        if (!component.material.shader.name.ToLowerInvariant().Contains("decalable"))
                        {
                            this.LogError($"BARRICADE ASSET WITH AN GUID OF {asset.GUID} AND A NAME OF {asset.name} HAS A MESH WITHOUT A DECALABLE SHADER: {parent.name}");
                        }
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