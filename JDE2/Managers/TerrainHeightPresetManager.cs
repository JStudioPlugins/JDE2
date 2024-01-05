using JDE2.SituationDependent;
using JDE2.Utils;
using SDG.Framework.Devkit;
using SDG.Framework.Devkit.Tools;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JDE2.Managers
{
    public class TerrainHeightPresetManager : IEditorDependent
    {
        public const byte SAVEDATA_VERSION = 1;

        public static TerrainHeightPresetManager Instance { get; private set; }

        public Preset[] Presets;

        public TerrainHeightPresetManager()
        {
            Instance = this;
            Load();
            DirtyManager.saved += Save;
            Level.onLevelExited += LevelExit;
        }

        public void LevelExit()
        {
            DirtyManager.saved -= Save;
            Level.onLevelExited -= LevelExit;
            Instance = null;
        }

        public void CopyTo(int preset)
        {
            Presets[preset].Mode = TerrainEditor.heightmapMode;
            Presets[preset].SmoothMethod = DevkitLandscapeToolHeightmapOptions.instance.smoothMethod;
            Presets[preset].FlattenMethod = DevkitLandscapeToolHeightmapOptions.instance.flattenMethod;
            Presets[preset].BrushRadius = DevkitLandscapeToolHeightmapOptions.instance.brushRadius;
            Presets[preset].BrushFalloff = DevkitLandscapeToolHeightmapOptions.instance.brushFalloff;
            Presets[preset].BrushStrength = EditorTerrainHeightUITool.TerrainEditor.heightmapBrushStrength;
            Presets[preset].FlattenTarget = DevkitLandscapeToolHeightmapOptions.instance.flattenTarget;
            Presets[preset].MaxPreviewSamples = DevkitLandscapeToolHeightmapOptions.instance.maxPreviewSamples;
        }

        public void CopyFrom(int preset)
        {
            TerrainEditor.heightmapMode = Presets[preset].Mode;
            DevkitLandscapeToolHeightmapOptions.instance.smoothMethod = Presets[preset].SmoothMethod;
            DevkitLandscapeToolHeightmapOptions.instance.flattenMethod = Presets[preset].FlattenMethod;
            DevkitLandscapeToolHeightmapOptions.instance.brushRadius = Presets[preset].BrushRadius;
            DevkitLandscapeToolHeightmapOptions.instance.brushFalloff = Presets[preset].BrushFalloff;
            EditorTerrainHeightUITool.TerrainEditor.heightmapBrushStrength = Presets[preset].BrushStrength;
            DevkitLandscapeToolHeightmapOptions.instance.flattenTarget = Presets[preset].FlattenTarget;
            DevkitLandscapeToolHeightmapOptions.instance.maxPreviewSamples = Presets[preset].MaxPreviewSamples;

            EditorTerrainHeightUITool.ModeButton.state = (int)Presets[preset].Mode;
            EditorTerrainHeightUITool.SmoothMethodButton.state = (int)Presets[preset].SmoothMethod;
            EditorTerrainHeightUITool.FlattenMethodButton.state = (int)Presets[preset].FlattenMethod;
            EditorTerrainHeightUITool.BrushRadiusField.Value = Presets[preset].BrushRadius;
            EditorTerrainHeightUITool.BrushFalloffField.Value = Presets[preset].BrushFalloff;
            EditorTerrainHeightUITool.BrushStrengthField.Value = Presets[preset].BrushStrength;
            EditorTerrainHeightUITool.FlattenTargetField.Value = Presets[preset].FlattenTarget;
            EditorTerrainHeightUITool.MaxPreviewSamplesField.Value = Presets[preset].MaxPreviewSamples;
        }

        public void Save()
        {
            var block = new Block();
            block.writeByte(SAVEDATA_VERSION);
            block.writeByte((byte)Presets.Length);
            for (int i = 0; i < Presets.Length; i++)
            {
                block.writeByte((byte)Presets[i].Mode);
                block.writeByte((byte)Presets[i].SmoothMethod);
                block.writeByte((byte)Presets[i].FlattenMethod);
                block.writeSingle(Presets[i].BrushRadius);
                block.writeSingle(Presets[i].BrushFalloff);
                block.writeSingle(Presets[i].BrushStrength);
                block.writeSingle(Presets[i].FlattenTarget);
                block.writeSingle(Presets[i].MaxPreviewSamples);
            }
            ReadWrite.writeBlock(Level.info.path + "/JDE2/TerrainHeightPresets.dat", useCloud: false, usePath: false, block);
        }

        public void Load()
        {
            Presets = new Preset[] { new Preset(), new Preset(), new Preset()};

            if (ReadWrite.fileExists(Level.info.path + "/JDE2/TerrainHeightPresets.dat", false, false))
            {
                var block = ReadWrite.readBlock(Level.info.path + "/JDE2/TerrainHeightPresets.dat", false, false, 0);
                block.readByte(); //Savedata version.
                byte length = block.readByte();
                for (int i = 0; i < length; i++)
                {
                    Presets[i].Mode = (TerrainEditor.EDevkitLandscapeToolHeightmapMode)block.readByte();
                    Presets[i].SmoothMethod = (EDevkitLandscapeToolHeightmapSmoothMethod)block.readByte();
                    Presets[i].FlattenMethod = (EDevkitLandscapeToolHeightmapFlattenMethod)block.readByte();
                    Presets[i].BrushRadius = block.readSingle();
                    Presets[i].BrushFalloff = block.readSingle();
                    Presets[i].BrushStrength = block.readSingle();
                    Presets[i].FlattenTarget = block.readSingle();
                    Presets[i].MaxPreviewSamples = block.readUInt32();
                }
            }
        }

        public class Preset()
        {
            public TerrainEditor.EDevkitLandscapeToolHeightmapMode Mode;
            public float BrushRadius;
            public float BrushFalloff;
            public float BrushStrength;
            public EDevkitLandscapeToolHeightmapSmoothMethod SmoothMethod;
            public float FlattenTarget;
            public EDevkitLandscapeToolHeightmapFlattenMethod FlattenMethod;
            public uint MaxPreviewSamples;
        }
    }
}
