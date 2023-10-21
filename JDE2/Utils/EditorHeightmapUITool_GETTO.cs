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
    public static class EditorHeightmapUITool
    {
        public static SleekButtonState ModeButton;

        public static ISleekFloat32Field BrushRadiusField;

        public static ISleekFloat32Field BrushFalloffField;

        public static ISleekFloat32Field BrushStrengthField;

        public static ISleekFloat32Field FlattenTargetField;

        public static ISleekUInt32Field MaxPreviewSamplesField;

        public static SleekButtonState SmoothMethodButton;

        public static SleekButtonState FlattenMethodButton;
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
            EditorHeightmapUITool.ModeButton = ___modeButton;
            EditorHeightmapUITool.BrushRadiusField = ___brushRadiusField;
            EditorHeightmapUITool.BrushFalloffField = ___brushFalloffField;
            EditorHeightmapUITool.BrushStrengthField = ___brushStrengthField;
            EditorHeightmapUITool.FlattenTargetField = ___flattenTargetField;
            EditorHeightmapUITool.MaxPreviewSamplesField = ___maxPreviewSamplesField;
            EditorHeightmapUITool.SmoothMethodButton = ___smoothMethodButton;
            EditorHeightmapUITool.FlattenMethodButton = ___flattenMethodButton;
            LoggingManager.Log("Confirmation of postfix working for editor terrain height", true);
        }
#pragma warning restore IDE0051 // Remove unused private members
    }
}
