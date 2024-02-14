using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JDE2.Options
{
    public interface IOption
    {
        /// <summary>
        /// Toggle: 50f
        /// Slider: 30f
        /// Button/Field: 40f
        /// Color Picker w Alpha: 160f
        /// Color Picker: 130f
        /// </summary>
        public float Spacing { get; }

        public ISleekElement GetElement();
    }
}
