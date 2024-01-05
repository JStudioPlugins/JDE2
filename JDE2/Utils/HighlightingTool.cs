using SDG.Unturned;
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JDE2.Utils
{
    public class HighlightingTool : MonoBehaviour
    {
        private static HighlightingTool _instance;

        public Color SelectedColor;

        public Dictionary<Transform, Color> Highlights;

        public ushort HighlightId;

        public static HighlightingTool Get()
        {
            if (_instance == null)
                _instance = Main.BackingObj.AddComponent<HighlightingTool>();
            return _instance;
        }

        public static void CleanUp()
        {
            if (_instance == null)
                return;
            _instance.ClearHighlights();
            Destroy(_instance);
            _instance = null;
        }

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
                return Color.red;
            }
            else if (index == 1)
            {
                return Palette.COLOR_O;
            }
            else if (index == 2)
            {
                return Color.yellow;
            }
            else if (index == 3)
            {
                return Color.green;
            }
            else if (index == 4)
            {
                return Color.blue;
            }
            else if (index == 5)
            {
                return new Color(75f, 0, 130f);
            }
            else if (index == 6)
            {
                return new Color(128f, 0, 225f);
            }
            else
            {
                return Color.red;
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
