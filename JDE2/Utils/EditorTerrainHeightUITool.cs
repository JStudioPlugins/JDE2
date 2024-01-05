using HarmonyLib;
using JDE2.Managers;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace JDE2.Utils
{
    public static class EditorTerrainHeightUITool
    {
        public static SleekButtonState ModeButton { get; internal set; }

        public static ISleekFloat32Field BrushRadiusField { get; internal set; }

        public static ISleekFloat32Field BrushFalloffField { get; internal set; }

        public static ISleekFloat32Field BrushStrengthField { get; internal set; }

        public static ISleekFloat32Field FlattenTargetField { get; internal set; }

        public static ISleekUInt32Field MaxPreviewSamplesField { get; internal set; }

        public static SleekButtonState SmoothMethodButton { get; internal set; }

        public static SleekButtonState FlattenMethodButton { get; internal set; }

        public static TerrainEditor TerrainEditor
        {
            get
            {
                Type type = AccessTools.TypeByName("SDG.Unturned.EditorInteract");
                object instance = AccessTools.Field(type, "instance").GetValue(null);
                return (TerrainEditor)instance.GetType().GetField("terrainTool").GetValue(instance);
            }
        }

    }

    [HarmonyPatch]
    class EditorTerrainHeightUIPatch
    {
#pragma warning disable IDE0051 // Remove unused private members
        static MethodBase TargetMethod()
        {
            // use reflections (with or without AccessTools) to return the MethodInfo of the original method
            var type = AccessTools.TypeByName("SDG.Unturned.EditorTerrainHeightUI");
            return AccessTools.Constructor(type);
        }

        [HarmonyPostfix]
        static void EditorTerrainHeightUIPostfix(ref SleekButtonState ___modeButton, ref ISleekFloat32Field ___brushRadiusField, ref ISleekFloat32Field ___brushFalloffField, ref ISleekFloat32Field ___brushStrengthField, ref ISleekFloat32Field ___flattenTargetField, ref ISleekUInt32Field ___maxPreviewSamplesField, ref SleekButtonState ___smoothMethodButton, ref SleekButtonState ___flattenMethodButton)
        {
            EditorTerrainHeightUITool.ModeButton = ___modeButton;
            EditorTerrainHeightUITool.BrushRadiusField = ___brushRadiusField;
            EditorTerrainHeightUITool.BrushFalloffField = ___brushFalloffField;
            EditorTerrainHeightUITool.BrushStrengthField = ___brushStrengthField;
            EditorTerrainHeightUITool.FlattenTargetField = ___flattenTargetField;
            EditorTerrainHeightUITool.MaxPreviewSamplesField = ___maxPreviewSamplesField;
            EditorTerrainHeightUITool.SmoothMethodButton = ___smoothMethodButton;
            EditorTerrainHeightUITool.FlattenMethodButton = ___flattenMethodButton;
            LoggingManager.Log("Confirmation of postfix working for editor terrain height", true);
        }
#pragma warning restore IDE0051 // Remove unused private members
    }
}
