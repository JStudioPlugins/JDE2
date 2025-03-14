﻿using DanielWillett.UITools.Util;
using HarmonyLib;
using JDE2.Managers;
using JDE2.SituationDependent;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace JDE2.UI
{
    public class EditorCustomTerrainUI : EditorCustomUI, IEditorDependent
    {
        public static EditorCustomTerrainUI Instance { get; private set; }

        public override int ContainerOffsetX => 0;
        public override int ContainerOffsetY => 0;
        public override float ContainerPositionScaleX => 1f;
        public override int ContainerSizeOffsetX => -20;
        public override int ContainerSizeOffsetY => -20;
        public override float ContainerSizeScaleX => 1f;
        public override float ContainerSizeScaleY => 1f;

        [Element(0)]
        private static ISleekButton PresetButton1;

        private static ISleekButton PresetButton1Add;

        [Element(1)]
        private static ISleekButton PresetButton2;

        private static ISleekButton PresetButton2Add;

        [Element(2)]
        private static ISleekButton PresetButton3;

        private static ISleekButton PresetButton3Add;

        public EditorCustomTerrainUI() : base()
        {
            Instance = this;

            PresetButton1 = Glazier.Get().CreateButton();
            PresetButton1.PositionOffset_X = 600;
            PresetButton1.PositionOffset_Y = -115;
            PresetButton1.PositionScale_X = 0.5f;
            PresetButton1.PositionScale_Y = 0.5f;
            PresetButton1.SizeOffset_X = 200;
            PresetButton1.SizeOffset_Y = 30;
            PresetButton1.Text = "Preset 1";
            PresetButton1.TooltipText = "Terrain brush preset. Right-click to set.";
            PresetButton1.OnClicked += OnClickedPreset1;
            Container.AddChild(PresetButton1);

            PresetButton2 = Glazier.Get().CreateButton();
            PresetButton2.PositionOffset_X = 600;
            PresetButton2.PositionOffset_Y = 45;
            PresetButton2.PositionScale_X = 0.5f;
            PresetButton2.PositionScale_Y = 0.5f;
            PresetButton2.SizeOffset_X = 200;
            PresetButton2.SizeOffset_Y = 30;
            PresetButton2.Text = "Preset 2";
            PresetButton2.TooltipText = "Terrain brush preset. Right-click to set.";
            PresetButton2.OnClicked += OnClickedPreset2;
            Container.AddChild(PresetButton2);

            PresetButton3 = Glazier.Get().CreateButton();
            PresetButton3.PositionOffset_X = 600;
            PresetButton3.PositionOffset_Y = 45;
            PresetButton3.PositionScale_X = 0.5f;
            PresetButton3.PositionScale_Y = 0.5f;
            PresetButton3.SizeOffset_X = 200;
            PresetButton3.SizeOffset_Y = 30;
            PresetButton3.Text = "Preset 3";
            PresetButton3.TooltipText = "Terrain brush preset. Right-click to set.";
            PresetButton3.OnClicked += OnClickedPreset3;
            Container.AddChild(PresetButton3);

            EditorCustomUIAutoSpacer spacer = new(this, 0, 40);

            PresetButton1Add = Glazier.Get().CreateButton();
            PresetButton1Add.CopyTransformFrom(PresetButton1);
            PresetButton1Add.SizeOffset_X = 30;
            PresetButton1Add.SizeOffset_Y = 30;
            PresetButton1Add.PositionOffset_X = 660;
            PresetButton1Add.Text = "+";
            PresetButton1Add.TooltipText = "Set the brush.";
            PresetButton1Add.OnClicked += OnRightClickedPreset1;
            Container.AddChild(PresetButton1Add);

            PresetButton2Add = Glazier.Get().CreateButton();
            PresetButton2Add.CopyTransformFrom(PresetButton2);
            PresetButton2Add.SizeOffset_X = 30;
            PresetButton2Add.SizeOffset_Y = 30;
            PresetButton2Add.PositionOffset_X = 660;
            PresetButton2Add.Text = "+";
            PresetButton2Add.TooltipText = "Set the brush.";
            PresetButton2Add.OnClicked += OnRightClickedPreset2;
            Container.AddChild(PresetButton2Add);

            PresetButton3Add = Glazier.Get().CreateButton();
            PresetButton3Add.CopyTransformFrom(PresetButton3);
            PresetButton3Add.SizeOffset_X = 30;
            PresetButton3Add.SizeOffset_Y = 30;
            PresetButton3Add.PositionOffset_X = 660;
            PresetButton3Add.Text = "+";
            PresetButton3Add.TooltipText = "Set the brush.";
            PresetButton3Add.OnClicked += OnRightClickedPreset3;
            Container.AddChild(PresetButton3Add);

        }

        private void OnRightClickedPreset1(ISleekElement button)
        {
            TerrainHeightPresetManager.Instance.CopyTo(0);
        }

        private void OnRightClickedPreset2(ISleekElement button)
        {
            TerrainHeightPresetManager.Instance.CopyTo(1);
        }

        private void OnRightClickedPreset3(ISleekElement button)
        {
            TerrainHeightPresetManager.Instance.CopyTo(2);
        }

        private void OnClickedPreset1(ISleekElement button)
        {
            TerrainHeightPresetManager.Instance.CopyFrom(0);
        }

        private void OnClickedPreset2(ISleekElement button)
        {
            TerrainHeightPresetManager.Instance.CopyFrom(1);
        }

        private void OnClickedPreset3(ISleekElement button)
        {
            TerrainHeightPresetManager.Instance.CopyFrom(2);
        }
    }

    [HarmonyPatch]
    class EditorTerrainHeightUIOpenPatch
    {
        static MethodBase TargetMethod()
        {
            // use reflections (with or without AccessTools) to return the MethodInfo of the original method
            var type = AccessTools.TypeByName("SDG.Unturned.EditorTerrainHeightUI");
            return AccessTools.Method(type, "Open");
        }

        [HarmonyPostfix]
        static void openPostfix()
        {
            EditorCustomTerrainUI.Instance.Open();
        }
    }

    [HarmonyPatch]
    class EditorTerrainHeightUIClosePatch
    {
        static MethodBase TargetMethod()
        {
            // use reflections (with or without AccessTools) to return the MethodInfo of the original method
            var type = AccessTools.TypeByName("SDG.Unturned.EditorTerrainHeightUI");
            return AccessTools.Method(type, "Close");
        }

        [HarmonyPostfix]
        static void closePostfix()
        {
            EditorCustomTerrainUI.Instance.Close();
        }
    }
}
