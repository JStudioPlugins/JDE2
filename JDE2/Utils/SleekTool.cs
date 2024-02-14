using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JDE2.Utils
{
    public static class SleekTool
    {
        public static List<ISleekElement> GetChildElements(this ISleekElement element)
        {
            var list = new List<ISleekElement>();

            var cont = true;
            var index = 0;
            while (cont)
            {
                try
                {
                    element.GetChildAtIndex(index);
                    list.Add(element);
                    index++;
                }
                catch (Exception _)
                {
                    cont = false;
                }
            }

            return list;
        }

        public static ISleekElement GetHighestYElement(this ICollection<ISleekElement> collection)
        {
            ISleekElement element = null;
            

            foreach (var item in collection)
            {
                if (element == null)
                    element = item;
                else if (item.PositionOffset_Y > element.PositionOffset_Y)
                    element = item;
            }

            return element;
        }
    }
}
