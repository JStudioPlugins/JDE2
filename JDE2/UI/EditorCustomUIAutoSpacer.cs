using JDE2.Utils;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace JDE2.UI
{
    public class EditorCustomUIAutoSpacer
    {
        public EditorCustomUI UI { get; set; }

        private float _x;

        private float _y;

        public float IncrementX { get; set; }

        public float IncrementY { get; set; }

        public EditorCustomUIAutoSpacer(EditorCustomUI ui, float incrementX, float incrementY)
        {
            IncrementX = incrementX;
            IncrementY = incrementY;
            UI = ui;

            Apply();
        }

        public void Apply()
        {
            var elements = UI.GetType().GetFields(ReflectionTool.ReflectionFlags)
                .Where(x => Attribute.IsDefined(x, typeof(Element)) && typeof(ISleekElement).IsAssignableFrom(x.FieldType))
                .OrderBy(x => ((Element)Attribute.GetCustomAttributes(x, typeof(Element))[0]).Index).ToList();

            for (int i = 0; i < elements.Count; i++)
            {
                FieldInfo element = elements[i];
                ISleekElement obj = (ISleekElement)(element.IsStatic ? element.GetValue(null) : element.GetValue(UI));
                var att = element.GetCustomAttributes(typeof(Element)).OrderBy(x => ((Element)x).Index).ToList();
                if (i == 0)
                {
                    _x = obj.PositionOffset_X;
                    _y = obj.PositionOffset_Y;
                }
                for (int j = 0; j < att.Count; j++)
                {
                    if (i == 0 && j == 0) continue;
                    _x += IncrementX;
                    _y += IncrementY;
                    obj.PositionOffset_X = _x;
                    obj.PositionOffset_Y = _y;
                }
            }
        }
    }

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true,
                   Inherited = true)]
    public class Element(int index) : Attribute
    {
        public int Index = index;
    }
}
