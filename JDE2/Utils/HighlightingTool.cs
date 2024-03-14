using SDG.Unturned;
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JDE2.SituationDependent;

namespace JDE2.Utils
{
    public class HighlightingTool : MonoBehaviour
    {
        private static HighlightingTool _instance;

        public Color SelectedColor;

        public Dictionary<Transform, Color> Highlights;

        public ushort HighlightId;

        public static HighlightingTool Get() => _instance;

        public HighlightingTool()
        {
            _instance = this;

            Highlights = new();
        }

        public List<EditorSelection> EditorSelection
        {
            get => (List<EditorSelection>)ReflectionTool.GetField<EditorObjects>("selection").GetValue(null);

            set => ReflectionTool.GetField<EditorObjects>("selection").SetValue(null, value);
        }

        public static Color ColorFromIndex(int index)
        {
            if (index == 0)
            {
                return Palette.COLOR_R;
            }
            else if (index == 1)
            {
                return Palette.COLOR_O;
            }
            else if (index == 2)
            {
                return Palette.COLOR_Y;
            }
            else if (index == 3)
            {
                return Palette.COLOR_O;
            }
            else if (index == 4)
            {
                return Palette.COLOR_B;
            }
            else if (index == 5)
            {
                return new Color(75f, 0, 130f);
            }
            else if (index == 6)
            {
                return Palette.COLOR_P;
            }
            else
            {
                return Palette.COLOR_R;
            }
        }

        public void AddObjectIdHighlights()
        {
            foreach (var list in LevelObjects.objects)
            {
                foreach (var obj in list)
                {
                    if (obj.id == HighlightId)
                        AddHighlight(obj.transform, SelectedColor);
                }
            }
        }

        public void AddObjectSelectionHighlights()
        {
            foreach (var selection in EditorSelection)
            {
                AddHighlight(selection.transform, SelectedColor);
            }
        }

        public void ClearHighlights()
        {
            foreach (var pair in Highlights)
                HighlighterTool.unhighlight(pair.Key);
            Highlights.Clear();
        }

        public void AddHighlight(Transform transform, Color color)
        {
            if (Highlights.ContainsKey(transform))
                return;

            HighlighterTool.highlight(transform, color);
            Highlights.Add(transform, color);
        }

        public void RemoveHighlight(Transform transform)
        {
            if (!Highlights.ContainsKey(transform))
                return;

            Highlights.Remove(transform);
            HighlighterTool.unhighlight(transform);
        }

        internal void Update()
        {
            foreach (var pair in Highlights)
            {
                HighlighterTool.highlight(pair.Key, pair.Value);
            }
        }
    }
}
