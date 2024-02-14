using JDE2.Options;
using JDE2.SituationDependent;
using JDE2.Utils;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JDE2.Managers
{
    public class OptionManager : IJDEDependent
    {
        internal static List<IOption> _options = new();

        public static IReadOnlyList<IOption> Options
        {
            get { return _options.AsReadOnly(); }
        }

        public OptionManager() 
        {
            var types = ReflectionTool.GetTypesFromInterface<IOption>(Main.Instance.Assembly);

            if (PluginManager.Enabled)
            {
                foreach (var plugin in PluginManager.Instance.Plugins)
                    types.AddRange(ReflectionTool.GetTypesFromInterface<IOption>(plugin));
            }

            foreach (var type in types)
            {
                try
                {
                    var instance = (IOption)Activator.CreateInstance(type);

                    _options.Add(instance);
                }
                catch (Exception ex)
                {
                    LoggingManager.LogException(ex, $"ENCOUNTERED ERROR WHEN CREATING INSTANCE OF IOption {{{{cyan}}}}{type.FullName}{{{{_}}}}!");
                }
            }
        }

        public static void SetupOptions(ISleekScrollView optionsBox, out List<ISleekElement> activeElements)
        {
            var active = new List<ISleekElement>();
            var num = optionsBox.GetChildElements().GetHighestYElement().PositionOffset_Y;
            num += 180f;

            foreach (var option in _options)
            {
                var element = option.GetElement();
                element.PositionOffset_Y = num;
                optionsBox.AddChild(element);
                active.Add(element);
                num += option.Spacing;
            }

            activeElements = active;
        }

        
    }
}
