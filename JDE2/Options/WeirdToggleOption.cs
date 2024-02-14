using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JDE2.Options
{
    public class WeirdToggleOption : IOption
    {
        public float Spacing => 50f;

        public ISleekElement GetElement()
        {
            var debugToggle = Glazier.Get().CreateToggle();
            debugToggle.SizeOffset_X = 40f;
            debugToggle.SizeOffset_Y = 40f;
            debugToggle.AddLabel("Weird Toggle", ESleekSide.RIGHT);
            return debugToggle;
        }
    }
}
